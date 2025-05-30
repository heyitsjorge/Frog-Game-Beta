
using UnityEngine;
using UnityEngine.SceneManagement;
public class FrogPhysics : MonoBehaviour
{
    private WallJump wallJump;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public Animator animator;
    private float moveInput = 0f;

    public float jumpVelocity = 10f;
    public float gravityUp = 30f;
    public float gravityDown = 50f;
    public float jumpCutMultiplier = 0.5f;

    public float moveSpeed = 8f;
    public float airControl = 0.5f;

    public float coyoteTime = 0.1f;
    private float coyoteTimer = 0f;

    public float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;
    private bool jumpInputHeld;

    private bool isGrounded;
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
            canDash = true;
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

        if (jumpBufferTimer > 0 && coyoteTimer > 0)
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
        {
            wallJump.SetGrounded(isGrounded);
        }
    }

    void FixedUpdate()
    {
        throwCooldown -= Time.fixedDeltaTime;

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

        ApplyCustomGravity();
        HandleHorizontalMovement();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        animator.SetBool("isJumping", true);
        animator.SetBool("isSheething", false);
    }

    void ApplyCustomGravity()
    {
        if (rb.linearVelocity.y > 0 && !jumpInputHeld)
        {
            rb.linearVelocity += Vector2.down * gravityUp * jumpCutMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector2.down * gravityUp * Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity += Vector2.down * gravityDown * Time.fixedDeltaTime;
        }
    }

    void HandleHorizontalMovement()
    {
        float control = isGrounded ? 1f : airControl;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed * control, rb.linearVelocity.y);
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

        StartCoroutine(LoadMainMenuAfterDelay(2f));

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
}

