using UnityEngine;

public class BossBrain : MonoBehaviour
{
    public enum BossState { Idle, Walking, Dash}
    public BossState currentState = BossState.Idle;

    public Transform player;
    public float detectionRange = 10f; //yama will chase once player is within this range
    public float walkSpeed = 3f;

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

        if (currentState == BossState.Idle && distanceToPlayer < detectionRange)
        {
            ChangeState(BossState.Walking);
        }
        else if (currentState == BossState.Walking && distanceToPlayer >= detectionRange)
        {
            ChangeState(BossState.Idle);
        }

    }


    void ChangeState(BossState newState)
    {
        currentState = newState;
        if (newState != BossState.Dash)
        {
            isDashing = false;
        }
    }
}
