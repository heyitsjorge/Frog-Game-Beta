
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class FrogPhysics : MonoBehaviour
{
    private WallJump wallJump;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public Animator animator;
    private float moveInput = 0f;

    public float jumpVelocity = 15f;
    public float gravityUp = 30f;
    public float gravityDown = 60f;
    public float jumpCutMultiplier = 2f;

    public float moveSpeed = 8f;
    public float airControl = 0.5f;

    public float coyoteTime = 0.1f;
    private float coyoteTimer = 0f;

    public float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;
    private bool jumpInputHeld;

    private bool isJumpingUp = false;

    private bool hasJumped = false;
    private float jumpStartY = 0f;
    public float maxJumpHeight = 5f;
    public float apexVelocityThreshold = 1f; // Threshold to consider the apex of the jump
    public float apexExtraGravity = 120f; // Velocity at which the player is considered at the apex of the jump

    // Repurposed: using these to match new WallJump logic
    public bool isWallJumping = false;
    public float wallJumpTimer = 0f;
    public float wallJumpDuration = 0.15f;

    public bool suppressGravity = false;
    public float suppressGravityTime = 0f;

    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask ground;

    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float groundDashCoolDown = 0.5f;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float groundDashCooldownTimer = 0f;

    public float attackDuration = 0.75f;
    public float firstAttackDuration = 0.75f;
    public float secondAttackDuration = 0.66f;
    private bool isAttacking = false;
    private bool isSecondAttackSet = false;

    // Kunai
    public GameObject kunaiPrefab;
    public Transform kunaiSpawnPoint;
    public float kunaiSpeed = 15f;
    public float throwCooldownTime = 0f;
    private bool isThrowing = false;
    private float throwDuration = 0.25f;
    private float throwDurationLeft = 0.25f;
    private float throwCooldown = 0f;

    public Transform respawnPoint;
    public float maxHealth = 3;
    public float health = 3;
    private bool isDying = false;

    [SerializeField] private Collider2D weaponFirstAttackFirstCollider1;
    [SerializeField] private Collider2D weaponFirstAttackFirstCollider2;
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider1;
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider2;
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider3;
    [SerializeField] private Collider2D weaponSecondAttackFirstCollider1;
    [SerializeField] private Collider2D weaponSecondAttackSecondCollider1;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        wallJump = GetComponent<WallJump>();

        DisableFirstAttackWeaponCollider();
        DisableSecondAttackWeaponCollider();
    }

    void Update()
    {
        if (isDying) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetFloat("FallSpeed", rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
        if (isGrounded)
        {
            isJumpingUp = false;
            canDash = true;
            hasJumped = false;
            coyoteTimer = coyoteTime;
            groundDashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        jumpInputHeld = Input.GetButton("Jump");

        if (jumpBufferTimer > 0 && coyoteTimer > 0 && !hasJumped)
        {
            Jump();
            jumpBufferTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isThrowing)
        {
            if (!isAttacking && !isSecondAttackSet)
            {
                animator.SetBool("isAttacking", true);
                isAttacking = true;
            }
            else
            {
                isSecondAttackSet = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !isAttacking && throwCooldown <= 0)
        {
            animator.SetBool("isThrowing", true);
            animator.SetTrigger("startThrow");
            throwCooldown = throwCooldownTime;
            throwDurationLeft = throwDuration;
            isThrowing = true;
        }

        if (rb.linearVelocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isWallJumping", false);
        }

        if (moveInput != 0)
        {
            bool prevFlipX = sr.flipX;
            sr.flipX = moveInput > 0;
            if (prevFlipX != sr.flipX)
            {
                FlipWeaponColliders();
                FlipKunaiSpawnPoint();
            }
        }

        if (wallJump != null)
            wallJump.SetGrounded(isGrounded);

        if (suppressGravity)
        {
            suppressGravityTime -= Time.deltaTime;
            if (suppressGravityTime <= 0f)
            {
                suppressGravity = false;
            }
        }

        // Count down the wall-jump lock timer
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
                isWallJumping = false;
        }
    }

    void FixedUpdate()
    {
        if (isWallJumping && wallJumpTimer > 0f)
        {
            Debug.Log($"FixedUpdate START - isWallJumping: {isWallJumping}, wallJumpTimer: {wallJumpTimer:F2}, RB Velocity: {rb.linearVelocity}");
        }

        if (isAttacking)
        {
            attackDuration -= Time.fixedDeltaTime;
            if (attackDuration <= 0f)
            {
                animator.SetBool("isAttacking", false);
                isAttacking = false;
                attackDuration = firstAttackDuration;

                if (isSecondAttackSet)
                {
                    attackDuration = secondAttackDuration;
                }
                else
                {
                    animator.SetBool("isSheething", true);
                    DisableFirstAttackWeaponCollider();
                }
            }
        }

        if (isSecondAttackSet && !isAttacking)
        {
            animator.SetBool("isSecondAttacking", true);
            attackDuration -= Time.fixedDeltaTime;

            if (attackDuration <= 0f)
            {
                animator.SetBool("isSecondAttacking", false);
                isSecondAttackSet = false;
                attackDuration = secondAttackDuration;
                DisableFirstAttackWeaponCollider();
                DisableSecondAttackWeaponCollider();
            }
        }

        if (isThrowing)
        {
            throwDurationLeft -= Time.fixedDeltaTime;
            if (throwDurationLeft <= 0f)
            {
                isThrowing = false;
                throwDurationLeft = throwDuration;
                animator.SetBool("isThrowing", false);
            }
        }

        if (isDashing)
        {
            if (isWallJumping) Debug.Log("Wall jumping but also dashing!");
            DisableFirstAttackWeaponCollider();
            DisableSecondAttackWeaponCollider();

            float dashDirection = sr.flipX ? 1 : -1;
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                animator.SetBool("isDashing", false);
            }

            return;
        }

        if (isWallJumping && wallJumpTimer > 0f)
        {
            Debug.Log($"Before ApplyCustomGravity - RB Velocity: {rb.linearVelocity}");
        }

        ApplyCustomGravity();
        if (isWallJumping && wallJumpTimer > 0f)
        {
            Debug.Log($"Before HandleHorizontalMovement - RB Velocity: {rb.linearVelocity}");
        }
        HandleHorizontalMovement();
        if (isWallJumping && wallJumpTimer > 0f)
        {
            Debug.Log($"FixedUpdate END - RB Velocity: {rb.linearVelocity}");
        }

    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        animator.SetBool("isJumping", true);
        animator.SetBool("isSheething", false);

        // Track jump start
        isJumpingUp = true;
        hasJumped = true;
        jumpStartY = transform.position.y;
    }

    void ApplyCustomGravity()
    {
        if (suppressGravity) return;

        float gravityStrength = 0f;
        float currentJumpHeight = transform.position.y - jumpStartY;

        // Preserve horizontal velocity if within wall-jump window
        float preservedHorizontalVelocity = rb.linearVelocity.x;

        if (rb.linearVelocity.y > 0)
        {
            if (!jumpInputHeld || currentJumpHeight >= maxJumpHeight)
            {
                rb.linearVelocity = new Vector2(preservedHorizontalVelocity, 0);
                gravityStrength = gravityDown;
                isJumpingUp = false;
            }
            else
            {
                gravityStrength = gravityUp;
            }
        }
        else
        {
            gravityStrength = gravityDown;
            isJumpingUp = false;
        }

        rb.linearVelocity += Vector2.down * gravityStrength * Time.fixedDeltaTime;

        // Reapply preserved X if still wall-jumping
        if (isWallJumping && wallJumpTimer > 0f)
        {
            rb.linearVelocity = new Vector2(preservedHorizontalVelocity, Mathf.Max(rb.linearVelocity.y, -gravityDown * 0.75f));
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -gravityDown * 0.75f));
        }
    }

    void HandleHorizontalMovement()
    {
        // CRITICAL: Exit immediately if wall jumping - don't touch horizontal velocity at all
        if (isWallJumping && wallJumpTimer > 0f)
        {
            Debug.Log($"[HandleX] WALL JUMP ACTIVE - Skipping all horizontal movement (timer: {wallJumpTimer:F2})");
            return; // Exit the entire function
        }

        // Normal horizontal movement (only when NOT wall jumping)
        float control = isGrounded ? 1f : airControl;
        Vector2 oldVel = rb.linearVelocity;

        // Apply movement input
        if (moveInput != 0)
        {
            float newX = moveInput * moveSpeed * control;
            rb.linearVelocity = new Vector2(newX, oldVel.y);

            Debug.Log($"[HandleX] Movement applied: oldX={oldVel.x:F2} → newX={newX:F2}, input={moveInput}");
        }
        // Apply air resistance when airborne and no input
        else if (!isGrounded && Mathf.Abs(oldVel.x) > 0.1f)
        {
            rb.linearVelocity = new Vector2(oldVel.x * 0.98f, oldVel.y);
            Debug.Log($"[HandleX] Air resistance applied: {oldVel.x:F2} → {rb.linearVelocity.x:F2}");
        }
        // Stop horizontal movement when grounded and no input
        else if (isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, oldVel.y);
        }
    }

    void TryDash()
    {
        if (isDashing || isAttacking || isSecondAttackSet || isThrowing) return;

        if (isGrounded && groundDashCooldownTimer <= 0f)
        {
            StartDash();
            groundDashCooldownTimer = groundDashCoolDown;
        }
        else if (!isGrounded && canDash)
        {
            StartDash();
            canDash = false;
        }
    }

    public void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        animator.SetBool("isDashing", true);
        animator.SetBool("isSheething", false);
    }

    public void ResetDash()
    {
        canDash = true;
    }

    public void OnHit(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        animator.SetTrigger("isDying");
        animator.SetBool("isDead", true);
        isDying = true;
        rb.bodyType = RigidbodyType2D.Static;
        DisableFirstAttackWeaponCollider();
        DisableSecondAttackWeaponCollider();

        StartCoroutine(RespawnAfterDelay(2f));
    }
    private System.Collections.IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main Menu");
    }

    public void EnableFirstAttackFirstHitboxCollider()
    {
        weaponFirstAttackFirstCollider1.enabled = true;
        weaponFirstAttackFirstCollider2.enabled = true;
    }

    public void EnableFirstAttackSecondHitboxCollider()
    {
        weaponFirstAttackSecondCollider1.enabled = true;
        weaponFirstAttackSecondCollider2.enabled = true;
        weaponFirstAttackSecondCollider3.enabled = true;
    }

    public void DisableFirstAttackWeaponCollider()
    {
        weaponFirstAttackFirstCollider1.enabled = false;
        weaponFirstAttackFirstCollider2.enabled = false;
        weaponFirstAttackSecondCollider1.enabled = false;
        weaponFirstAttackSecondCollider2.enabled = false;
        weaponFirstAttackSecondCollider3.enabled = false;
    }

    public void EnableSecondAttackFirstHitboxCollider()
    {
        weaponSecondAttackFirstCollider1.enabled = true;
    }

    public void EnableSecondAttackSecondHitboxCollider()
    {
        weaponSecondAttackSecondCollider1.enabled = true;
    }

    public void DisableSecondAttackWeaponCollider()
    {
        weaponSecondAttackFirstCollider1.enabled = false;
        weaponSecondAttackSecondCollider1.enabled = false;
    }

    public void Sheeth()
    {
        animator.SetBool("isSheething", false);
    }

    private void FlipWeaponColliders()
    {
        Vector3 weaponPosition;
        Vector3 weaponRotation;

        void Flip(Collider2D collider)
        {
            weaponPosition = collider.transform.localPosition;
            weaponRotation = collider.transform.localEulerAngles;
            weaponPosition.x = -weaponPosition.x;
            weaponRotation.z = -weaponRotation.z;
            collider.transform.localPosition = weaponPosition;
            collider.transform.localEulerAngles = weaponRotation;
        }

        Flip(weaponFirstAttackFirstCollider1);
        Flip(weaponFirstAttackFirstCollider2);
        Flip(weaponFirstAttackSecondCollider1);
        Flip(weaponFirstAttackSecondCollider2);
        Flip(weaponFirstAttackSecondCollider3);
        Flip(weaponSecondAttackFirstCollider1);
        Flip(weaponSecondAttackSecondCollider1);
    }

    private void FlipKunaiSpawnPoint()
    {
        Vector3 spawnPosition = kunaiSpawnPoint.localPosition;
        spawnPosition.x = -spawnPosition.x;
        kunaiSpawnPoint.localPosition = spawnPosition;
    }

    public void ThrowKunai()
    {
        GameObject go = Instantiate(kunaiPrefab, kunaiSpawnPoint.position, kunaiSpawnPoint.rotation);
        var proj = go.GetComponent<KunaiProjectile>();
        proj.InititializeProjectile(sr.flipX ? Vector2.right : Vector2.left, go.GetComponent<Rigidbody2D>(), go.GetComponent<SpriteRenderer>());
    }
    
    private IEnumerator RespawnAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    // Move Jiri to the respawn point
    transform.position = respawnPoint.position;
    
    // Reset health and dying state
    health = maxHealth;
    isDying = false;

    // Reset animator state
    animator.SetBool("isDead", false);
    animator.ResetTrigger("isDying");

    // Reactivate controls/physics
    rb.bodyType = RigidbodyType2D.Dynamic;

    // Optional: Add invincibility frames, particle effects, or UI feedback here!
}

}
