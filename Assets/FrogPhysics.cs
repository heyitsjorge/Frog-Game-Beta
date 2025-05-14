/*using UnityEngine;

public class FrogPhysics : MonoBehaviour
{
    private WallJump wallJump; //reference to the wall jump script
    private Rigidbody2D rb; //rigidbody of the frog
    private SpriteRenderer sr; //sprite renderer of the frog

    public Animator animator; //animator of the frog
    private float moveInput = 0f; //input from the player
//Jump Variables
    public float jumpVelocity= 10f; //uparwd velocity
    public float gravityUp = 30f;  //upward gravity (slower than down)
    public float gravityDown = 50f; //downward gravity (faster than up)
    public float jumpCutMultiplier = 0.5f; //how much to cut the jump velocity when the player releases the jump button

//Movement Variables
    public float moveSpeed = 8f; //speed of the frog
    public float airControl = 0.5f; //how much control the player has in the air

//Coyote Time Variables
    public float coyoteTime = 0.1f; //how long the player can jump after leaving the ground
    public float coyoteTimer = 0f; //counter for coyote time

//Jump Variables
    public float jumpBufferTime = 0.1f; //how long the player can press the jump button before they hit the ground
    public float jumpBufferTimer = 0f; //counter for jump buffer time
    private bool jumpInputHeld;

//Ground Check Variables
    private bool isGrounded; //is the player on the ground
    public Transform groundCheck; //transform to check if the player is on the ground
    public float groundCheckRadius = 0.2f; //radius of the ground check
    public LayerMask ground; //layer of the ground

//Dash Variables
    public float dashSpeed = 20f; //speed of the dash
    public float dashDuration = 0.15f; //duration of the dash
    public float groundDashCoolDown = 0.5f; //cooldown of the dash

    private bool canDash = true; //can the player dash
    private bool isDashing = false; //is the player dashing
    private float dashTimer = 0f; //time left in the dash
    private float groundDashCooldownTimer = 0f; //time left in the dash cooldown

//Attack Variables
    public float attackDuration = 0.75f; //duration of the attack
    public float firstAttackDuration = 0.75f; //duration of the first attack
    public float secondAttackDuration = 0.66f; //duration of the second attack
    private bool isAttacking = false; //is the player attacking'
    private bool isSecondAttackSet = false; //is the second attack set

// Health Variables
    public float health = 3;
    private bool isDying = false;

    [SerializeField] private Collider2D weaponFirstAttackFirstCollider1; //first attack collider
    [SerializeField] private Collider2D weaponFirstAttackFirstCollider2; //first attack collider
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider1; //second attack collider
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider2; //second attack collider
    [SerializeField] private Collider2D weaponFirstAttackSecondCollider3; //second attack collider
    [SerializeField] private Collider2D weaponSecondAttackFirstCollider1; //first attack collider
    [SerializeField] private Collider2D weaponSecondAttackSecondCollider1; //second attack collider



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //set gravity scale to 0 so we can control the gravity ourselves
        wallJump = GetComponent<WallJump>(); //get the wall jump script

        // Disable all attack colliders at the start
        weaponFirstAttackFirstCollider1.enabled = false;
        weaponFirstAttackFirstCollider2.enabled = false;
        weaponFirstAttackSecondCollider1.enabled = false;
        weaponFirstAttackSecondCollider2.enabled = false;
        weaponFirstAttackSecondCollider3.enabled = false;
        weaponSecondAttackFirstCollider1.enabled = false;
        weaponSecondAttackSecondCollider1.enabled = false;
    
    }

    // Update is called once per frame
    void Update()
    {
        if(isDying)return;
        moveInput = Input.GetAxisRaw("Horizontal"); //get input from player
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetFloat("FallSpeed", rb.linearVelocity.y);
        //Coyote Timea
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ground);
        if (isGrounded)
        {
            canDash = true; //reset dash
            coyoteTimer = coyoteTime; //reset coyote timer
            groundDashCooldownTimer -= Time.deltaTime; //decrease dash cooldown timer
        }
        else
        {
            coyoteTimer -= Time.deltaTime; //decrease coyote timer
        }
        //Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime; //reset jump buffer timer
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime; //decrease jump buffer timer
        }
        //Jump cutting
        jumpInputHeld = Input.GetButton("Jump");
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            Jump();
            jumpBufferTimer = 0f; //reset jump buffer timer
        }
        //dash input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }

        //Attack input
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            if(!isAttacking && !isSecondAttackSet)
            {
                animator.SetBool("isAttacking", true);
                isAttacking = true;
            }
            else
            {
                isSecondAttackSet = true;
            }
            
        }
        //falling
        if (rb.linearVelocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
        }

        //fix direction of the sprite
        if (moveInput != 0)
        {
            sr.flipX = moveInput > 0;
        }
        if (wallJump != null)
        {
        wallJump.SetGrounded(isGrounded);
        }
    }
        
    void FixedUpdate(){
        if(isAttacking)
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
                } else{
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

        if (isDashing)
        {
        float dashDirection = sr.flipX ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        dashTimer -= Time.fixedDeltaTime;
        {
            // Clean up sword hitboxes:
            DisableFirstAttackWeaponCollider();
            DisableSecondAttackWeaponCollider();

            float dashDirection = sr.flipX ? 1 : -1;
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
            dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)

        //Attack input
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            if(!isAttacking && !isSecondAttackSet)
            {
                animator.SetBool("isAttacking", true);
                isAttacking = true;
            }
            else
            {
                isSecondAttackSet = true;
            }
            
        }
        //falling
        if (rb.linearVelocity.y <= 0)
        {
            animator.SetBool("isJumping", false);
        }

    }
        
    void FixedUpdate(){
        if(isAttacking)
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
                } else{
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

        if (isDashing)
        {
            // Clean up sword hitboxes:
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

            return; // Skip other movement while dashing
        }

        ApplyCustomGravity();
        HandleHorizontalMovement();
    }

    void Jump(){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity); //set the upward velocity
        animator.SetBool("isJumping", true); //set the jump animation
    }

    void ApplyCustomGravity(){
        //shorten jump is player releases to early
        if (rb.linearVelocity.y > 0 && !jumpInputHeld)
        {
            rb.linearVelocity += Vector2.down *gravityUp * jumpCutMultiplier * Time.fixedDeltaTime; //apply upward gravity
        }
        //regular jump
        else if (rb.linearVelocity.y > 0)
        {
            rb.velocity += Vector2.down * gravityUp * Time.fixedDeltaTime; //apply upward gravity
        }
        //falling down
        else  {
            rb.linearVelocity += Vector2.down * gravityDown * Time.fixedDeltaTime; //apply downward gravity
        }
    }

    void HandleHorizontalMovement(){

        float control = isGrounded ? 1f : airControl; //if the player is on the ground, give them full control, otherwise give them less control
        rb.linearVelocity = new Vector2(moveInput * moveSpeed * control, rb.linearVelocity.y); //set the horizontal velocity
        //flip the sprite based on the direction
        // Flip sprite based on direction
        if (moveInput != 0)
        {
            bool prevFlipX = sr.flipX;
            sr.flipX = moveInput > 0;
            if (prevFlipX != sr.flipX)
            {
                FlipWeaponColliders();
            }
        }   
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void TryDash()
    {   
        // Can't dash while already dashing or during cooldown on ground or while attacking
        if (isDashing || isAttacking || isSecondAttackSet) return;

        if (isGrounded && groundDashCooldownTimer <= 0f)
        {
            StartDash();
            groundDashCooldownTimer = groundDashCoolDown; // Cooldown starts
        }
        else if (!isGrounded && canDash)
        {
            StartDash();
            canDash = false; // Use up air dash
        }
    }
    public void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        animator.SetBool("isDashing", true); // Set dashing animation

        // Optional: Add a visual or sound effect here
    }
    public void OnHit(float damage)
    {
        health -= damage;
        //TODO: Add hit animation
        // animator.SetTrigger("isHit");
        if(health <= 0)
        {
            OnDeath();
        }
    }

    // Optional: Add a visual or sound effect here
 public void ResetDash()
{
    canDash = true;
}
    public void OnDeath()
    {
        animator.SetTrigger("isDying");
        animator.SetBool("isDead", true);
        isDying = true;
        rb.bodyType = RigidbodyType2D.Static;
        DisableFirstAttackWeaponCollider();
        DisableSecondAttackWeaponCollider();
    }
    public void EnableFirstAttackFirstHitboxCollider()
    {
        // Debug.Log("First Attack Hitbox Enabled");
        weaponFirstAttackFirstCollider1.enabled = true;
        weaponFirstAttackFirstCollider2.enabled = true;
    }
    public void EnableFirstAttackSecondHitboxCollider()
    {
        // Debug.Log("Second Attack Hitbox Enabled");
        weaponFirstAttackSecondCollider1.enabled = true;
        weaponFirstAttackSecondCollider2.enabled = true;
        weaponFirstAttackSecondCollider3.enabled = true;
    }

    public void DisableFirstAttackWeaponCollider()
    {
        // Debug.Log("First Attack Hitbox Disabled");
        
        weaponFirstAttackFirstCollider1.enabled = false;
        weaponFirstAttackFirstCollider2.enabled = false;
        weaponFirstAttackSecondCollider1.enabled = false;
        weaponFirstAttackSecondCollider2.enabled = false;
        weaponFirstAttackSecondCollider3.enabled = false;
    }

    public void EnableSecondAttackFirstHitboxCollider()
    {
        // Debug.Log("Second Attack Hitbox Enabled");
        weaponSecondAttackFirstCollider1.enabled = true;
    }
    
    public void EnableSecondAttackSecondHitboxCollider()
    {
        // Debug.Log("Second Attack Hitbox Enabled");
        weaponSecondAttackSecondCollider1.enabled = true;
    }

    public void DisableSecondAttackWeaponCollider()
    {
        // Debug.Log("Second Attack Hitbox Disabled");
        weaponSecondAttackFirstCollider1.enabled = false;
        weaponSecondAttackSecondCollider1.enabled = false;
    }

    private void FlipWeaponColliders()
    {
        Vector3 weaponPosition;
        Vector3 weaponRotation;

        // 1 1 1
        weaponPosition = weaponFirstAttackFirstCollider1.transform.localPosition;
        weaponRotation = weaponFirstAttackFirstCollider1.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponFirstAttackFirstCollider1.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponFirstAttackFirstCollider1.transform.localEulerAngles = weaponRotation;
        // 1 1 2
        weaponPosition = weaponFirstAttackFirstCollider2.transform.localPosition;
        weaponRotation = weaponFirstAttackFirstCollider2.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponFirstAttackFirstCollider2.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponFirstAttackFirstCollider2.transform.localEulerAngles = weaponRotation;

        // 1 2 1
        weaponPosition = weaponFirstAttackSecondCollider1.transform.localPosition;
        weaponRotation = weaponFirstAttackSecondCollider1.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponFirstAttackSecondCollider1.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponFirstAttackSecondCollider1.transform.localEulerAngles = weaponRotation;
        // 1 2 2
        weaponPosition = weaponFirstAttackSecondCollider2.transform.localPosition;
        weaponRotation = weaponFirstAttackSecondCollider2.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponFirstAttackSecondCollider2.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponFirstAttackSecondCollider2.transform.localEulerAngles = weaponRotation;
        // 1 2 3
        weaponPosition = weaponFirstAttackSecondCollider3.transform.localPosition;
        weaponRotation = weaponFirstAttackSecondCollider3.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponFirstAttackSecondCollider3.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponFirstAttackSecondCollider3.transform.localEulerAngles = weaponRotation;

        // 2 1 1
        weaponPosition = weaponSecondAttackFirstCollider1.transform.localPosition;
        weaponRotation = weaponSecondAttackFirstCollider1.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponSecondAttackFirstCollider1.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponSecondAttackFirstCollider1.transform.localEulerAngles = weaponRotation;

        // 2 2 1
        weaponPosition = weaponSecondAttackSecondCollider1.transform.localPosition;
        weaponRotation = weaponSecondAttackSecondCollider1.transform.localEulerAngles;
        weaponPosition.x = -weaponPosition.x;
        weaponSecondAttackSecondCollider1.transform.localPosition = weaponPosition;
        weaponRotation.z = -weaponRotation.z;
        weaponSecondAttackSecondCollider1.transform.localEulerAngles = weaponRotation;
    }

    public void Sheeth(){
        animator.SetBool("isSheething", false);

    }

}}
*/
using UnityEngine;

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

        if (Input.GetKeyDown(KeyCode.Mouse0))
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
            }
        }

        if (wallJump != null)
        {
            wallJump.SetGrounded(isGrounded);
        }
    }

    void FixedUpdate()
    {
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
        if (isDashing || isAttacking || isSecondAttackSet) return;

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
}

