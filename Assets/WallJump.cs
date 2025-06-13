using UnityEngine;
using System.Collections;

public class WallJump : MonoBehaviour
{
    [Header("Wall Jump Settings")]
    public float wallJumpForce = 14f;     // Vertical force of wall jump
    public float wallJumpPush = 8f;       // Horizontal force away from wall
    public float wallJumpCooldown = 0.1f; // Prevents multiple wall jumps instantly

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public Animator animator;
    private bool isTouchingWall = false;
    private bool isGrounded = false;
    public float wallJumpTimer = 0f;       // local cooldown for repeated jumps
    private int wallSide = 0;              // -1 = wall on left, 1 = wall on right

    private FrogPhysics frogPhysics;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        frogPhysics = GetComponent<FrogPhysics>();
    }

    private bool wallJumpRequested = false;

    void Update()
{
    // Count down local cooldown
    wallJumpTimer -= Time.deltaTime;

    if (isTouchingWall && !isGrounded)
    {
        animator.SetBool("isClinging", true);
        animator.SetBool("isSheething", false);
    }
    else
    {
        animator.SetBool("isClinging", false);
    }
    // Flag the wall-jump request when conditions are met
    if (Input.GetButtonDown("Jump") && isTouchingWall && !isGrounded && wallJumpTimer <= 0f)
    {
        Debug.Log("Wall Jump Requested - Wall Side: " + wallSide);
        wallJumpRequested = true;
    }
}

    void FixedUpdate()
{    
    // Execute the wall jump here for consistent physics timing
    if (wallJumpRequested)
    {
        wallJumpRequested = false;
        PerformWallJump();
    }
    else if (wallJumpRequested == false)
    {
        // Only log this occasionally to avoid spam
        if (Time.fixedTime % 1f < Time.fixedDeltaTime)
        {
            // Debug.Log("No wall jump requested this frame");
        }
    }
}

    void PerformWallJump()
{

    animator.SetBool("isWallJumping", true);
    wallJumpTimer = wallJumpCooldown;  // reset local cooldown

    // Calculate the jump direction - this should push AWAY from the wall
    float jumpDirection = wallSide * wallJumpPush;
    // Apply diagonal velocity immediately
    Vector2 wallJumpVelocity = new Vector2(jumpDirection, wallJumpForce);    
    rb.linearVelocity = wallJumpVelocity;
    
    if (frogPhysics != null)
    {
        // Lock horizontal movement for a short window
        frogPhysics.isWallJumping = true;
        frogPhysics.wallJumpTimer = frogPhysics.wallJumpDuration;
        frogPhysics.isGrounded = false; // Ensure grounded state is false during wall jump
        frogPhysics.ResetDash();

        // Suppress gravity briefly for cleaner arc
        frogPhysics.suppressGravity = true;
        frogPhysics.suppressGravityTime = 0.1f;

        StartCoroutine(DisableWallTouchTemporarily());
    }

    // Flip sprite to face away from wall
    // if (wallSide != 0)
    // {
    //     bool shouldFlipX = wallSide < 0; // If wall is on left (wallSide = -1), flip to face right
    //     sr.flipX = shouldFlipX;
    // }

    // Check velocity on next physics frame
    StartCoroutine(DebugVelocityNextFrame());
}

    private System.Collections.IEnumerator DebugVelocityNextFrame()
    {
        yield return new WaitForFixedUpdate();
        Debug.Log($"Velocity after one frame: {rb.linearVelocity}");
    }

    private IEnumerator DisableWallTouchTemporarily()
    {
        isTouchingWall = false;
        animator.SetBool("isClinging", false);
        yield return new WaitForSeconds(0.2f);
        // Wall contact will be re-enabled by collision detection
    }

    void OnCollisionStay2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Wall") && !isGrounded)
    {
        Debug.Log($"=== WALL COLLISION DETECTED ===");
        Debug.Log($"Player position: {transform.position}");
        Debug.Log($"Wall position: {collision.transform.position}");
        
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.Log($"Contact normal: {contact.normal}");
            Debug.Log($"Contact normal.x: {contact.normal.x}");
            Debug.Log($"Abs(normal.x): {Mathf.Abs(contact.normal.x)}");
            
            // Detect only vertical wall surfaces
            if (Mathf.Abs(contact.normal.x) > 0.7f)
            {
                int newWallSide = contact.normal.x > 0 ? -1 : 1; // Left wall = -1, Right wall = 1
                Debug.Log($"Wall side calculation: normal.x={contact.normal.x} > 0 ? -1 : 1 = {newWallSide}");
                
                // Explanation of wall side logic:
                // - If normal.x > 0, the wall surface normal points RIGHT, meaning we're touching the LEFT side of the wall
                // - If normal.x < 0, the wall surface normal points LEFT, meaning we're touching the RIGHT side of the wall
                Debug.Log($"Wall interpretation: {(newWallSide == -1 ? "LEFT wall (jump RIGHT)" : "RIGHT wall (jump LEFT)")}");
                
                wallSide = newWallSide;

                if (!isTouchingWall)
                {
                    isTouchingWall = true;
                    animator.SetBool("isClinging", true);
                    animator.SetTrigger("startCling");
                }

                Debug.Log($"Final wallSide: {wallSide}");
                Debug.Log($"=== END WALL COLLISION ===");
                return;
            }
        }
    }
}

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("isClinging", false);
            isTouchingWall = false;
            wallSide = 0;
        }
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        if (grounded)
        {
            animator.SetBool("isClinging", false);
            isTouchingWall = false;
        }
    }
}
