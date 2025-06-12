using UnityEngine;
using System.Collections;

public class BossBrain : MonoBehaviour
{
    public enum BossState { Idle, Walking, Dash, MeleeAttack, Dodge }
    public BossState currentState = BossState.Idle;

    public Transform player;
    public float detectionRange = 10f; //yama will chase once player is within this range
    public float walkSpeed = 3f;
    private bool attackOnCooldown = false; // Prevents multiple attacks in quick succession

    private bool actionLock = false;

    private Animator animator;
    private Rigidbody2D rb;

    //dash vars
    public float dashCooldown = 6f;
    public float dashRange = 4f;      // Player must be within this distance to dash
    public float dashSpeed = 8f;      // How fast Yama dashes
    public float dashDuration = 0.5f; // How long the dash lasts

    private float dashCooldownTimer;
    private bool isDashing = false;
    private float dashTimer = 0f;

    //dodge vars
    private bool hasDodged = false;
    public float dodgeJumpForce = 9f; // Force applied when dodging

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    void IdleUpdate()
    {
        // Play idle animation if not already playing
        animator.Play("Boss Idle");
        // Could add idle logic here
        rb.velocity = Vector2.zero; // Stop movement
    }

    void WalkingUpdate()
    {
        animator.Play("Boss Move");
        Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;
        rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y);


        float facing = player.position.x > transform.position.x ? -1 : 1;
        transform.localScale = new Vector3(3.5f * facing, 3.5f, 1);

    }

    void DashUpdate()
    {
        if (!isDashing)
        {
            animator.Play("Boss Dash");
            isDashing = true;
            dashTimer = dashDuration;

            float facing = player.position.x > transform.position.x ? -1 : 1;
            transform.localScale = new Vector3(3.5f * facing, 3.5f, 1);
        }
        float dashDirection = player.position.x > transform.position.x ? 1 : -1;

        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
            rb.velocity = Vector2.zero;
            dashCooldownTimer = dashCooldown;
            ChangeState(BossState.Walking);
        }
    }


    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                IdleUpdate();
                break;
            case BossState.Walking:
                WalkingUpdate();
                break;
            case BossState.Dash:
                DashUpdate();
                break;
            case BossState.MeleeAttack:
                MeleeAttackUpdate();
                break;
            case BossState.Dodge:
                DodgeUpdate();
                break;

        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        dashCooldownTimer -= Time.deltaTime;
        if (currentState != BossState.Dash && distanceToPlayer <= dashRange)
        {
            // Guaranteed dash if timer is up
            if (dashCooldownTimer <= 0)
            {
                ChangeState(BossState.Dash);
                return;
            }

            // Random dash chance (e.g., 10% chance per second when player is close)
            if (Random.value < 0.01f) // ~1% chance per frame (~60% per second at 60fps)
            {
                ChangeState(BossState.Dash);
                return;
            }
        }
        if (currentState == BossState.Walking && distanceToPlayer < 2.5f && !actionLock)
        {
            // Only trigger if not already attacking or dodging
            if (!attackOnCooldown)
            {
                float r = Random.value;
                if (r < 0.05f)
                {
                    ChangeState(BossState.MeleeAttack);
                    attackOnCooldown = true;
                    return;
                }
                else if (r < 0.02f) // Slightly higher to avoid both at once
                {
                    ChangeState(BossState.Dodge);
                    attackOnCooldown = true;
                    return;
                }
            }
        }
    }


    void ChangeState(BossState newState)
    {
    // Only allow state change to attack/dodge if not busy,
    // or always allow switching to Idle, Walking, Dash
        if (actionLock && 
            (newState == BossState.MeleeAttack || newState == BossState.Dodge))
        {
            return; // Ignore attempts to re-trigger attack/dodge
        }

        currentState = newState;

        if (newState != BossState.Dash) isDashing = false;
        if (animator == null) return;

        switch (newState)
        {
            case BossState.MeleeAttack:
                actionLock = true; // Block further actions
                animator.ResetTrigger("isMeleeAttacking");
                animator.SetTrigger("isMeleeAttacking");
                StartCoroutine(EndAttackAfter(0.8f)); // Duration as needed
                break;
            case BossState.Dodge:
                actionLock = true; // Block further actions
                animator.ResetTrigger("isDodging");
                animator.SetTrigger("isDodging");
                StartCoroutine(EndAttackAfter(0.6f)); // Duration as needed
                break;
                // Add other attack states if needed
        }
    }

    

    public void OnDeath()
    {
        Debug.Log("Yama has been defeated!");

        // Play death animation (if you have one)
        if (animator != null)
        {
            animator.Play("Death"); // Make sure you have a "Death" animation, or just skip this line for now
        }

        // Disable boss movement/attacks
        this.enabled = false; // disables the BossBrain script

        // Optional: Disable Yama's collider to prevent further interaction
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        //add some extra polish here like sounds or particle affects on death.

        // Optional: Destroy Yama after a delay
        Destroy(gameObject, 2f); // Destroys Yama after 2 seconds

        // Optional: Trigger scene transition, victory UI, etc.
        // SceneManager.LoadScene("WinScene");
    }

    void MeleeAttackUpdate()
    {
        rb.linearVelocity = Vector2.zero; // Stop movement during melee attack
    
    }

    void DodgeUpdate()
    {
        if (!hasDodged)
        {
            rb.velocity = new Vector2(rb.velocity.x, dodgeJumpForce); // Preserve X, boost Y
            hasDodged = true;
        }       
    }

    IEnumerator EndAttackAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        actionLock = false;// Reset cooldown after the attack
        hasDodged = false; // Reset dodge state
        ChangeState(BossState.Idle);
    }

}
