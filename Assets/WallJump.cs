using UnityEngine;

public class WallJump : MonoBehaviour
{
    [Header("Wall Jump Settings")]
    public float wallJumpForce = 14f;     // Vertical force of wall jump
    public float wallJumpPush = 8f;       // Horizontal force away from wall
    public float wallJumpCooldown = 0.2f; // Prevents multiple wall jumps instantly

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public Animator animator;
    private bool isTouchingWall = false;
    private bool isGrounded = false; // You’ll update this via FrogPhysics or from a shared state
    private float wallJumpTimer = 0f;
    private int wallSide = 0; // -1 = wall on left, 1 = wall on right


    private FrogPhysics frogPhysics; // Optional: reference to reset dash

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        frogPhysics = GetComponent<FrogPhysics>(); // Only if needed for resetting dash
    }

    void Update()
    {
        wallJumpTimer -= Time.deltaTime;

        if (isTouchingWall)
        {
            animator.SetBool("isClinging", true);
            animator.SetBool("isSheething", false);
        }

        if (Input.GetButtonDown("Jump") && isTouchingWall && !isGrounded && wallJumpTimer <= 0f)
        {
            Debug.Log("Wall Jump Triggered");
            PerformWallJump();
        }
    }

    void PerformWallJump()
    {
        animator.SetBool("isWallJumping", true);
        wallJumpTimer = wallJumpCooldown;

        rb.linearVelocity = new Vector2(wallSide * wallJumpPush, wallJumpForce);

        if (frogPhysics != null)
            frogPhysics.ResetDash();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("isClinging", true);
            animator.SetTrigger("startCling");
            isTouchingWall = true;
            // Use contact normal to determine wall side
            ContactPoint2D contact = collision.GetContact(0);
            wallSide = contact.normal.x > 0 ? -1 : 1; // Wall is on left → push right
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("isClinging", false);
            isTouchingWall = false;
        }
    }

    // Call this from FrogPhysics to sync ground status
    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }
}
