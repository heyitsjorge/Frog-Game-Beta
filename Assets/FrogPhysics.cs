using UnityEngine;

public class FrogPhysics : MonoBehaviour
{
    private WallJump wallJump; //reference to the wall jump script
    private Rigidbody2D rb; //rigidbody of the frog
    private SpriteRenderer sr; //sprite renderer of the frog
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; //set gravity scale to 0 so we can control the gravity ourselves
        wallJump = GetComponent<WallJump>(); //get the wall jump script
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); //get input from player
        //Coyote Time
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
        if (isDashing)
    {
        float dashDirection = sr.flipX ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }

        return; // Skip other movement while dashing
    }


        ApplyCustomGravity();
        HandleHorizontalMovement();
    }

    void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); //set the upward velocity
    }

    void ApplyCustomGravity(){
        //shorten jump is player realses to early
        if (rb.velocity.y > 0 && !jumpInputHeld)
        {
            rb.velocity += Vector2.down *gravityUp * jumpCutMultiplier * Time.fixedDeltaTime; //apply upward gravity
        }
        //regualr jump
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.down * gravityUp * Time.fixedDeltaTime; //apply upward gravity
        }
        //faling down
        else
        {
            rb.velocity += Vector2.down * gravityDown * Time.fixedDeltaTime; //apply downward gravity
        }
    }

    void HandleHorizontalMovement(){

        float control = isGrounded ? 1f : airControl; //if the player is on the ground, give them full control, otherwise give them less control
        rb.velocity = new Vector2(moveInput * moveSpeed * control, rb.velocity.y); //set the horizontal velocity
        //flip the sprite based on the direction
        // Flip sprite based on direction
        if (moveInput != 0)
        {
            sr.flipX = moveInput < 0;
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
    // Can't dash while already dashing or during cooldown on ground
    if (isDashing) return;

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
void StartDash()
{
    isDashing = true;
    dashTimer = dashDuration;

    // Optional: Add a visual or sound effect here
}
public void ResetDash()
{
    canDash = true;
}

}

