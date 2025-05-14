using UnityEngine;

public class JiriMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;

    private bool isDashing = false;
    private bool isGrounded = false;
    private float dashTime;
    private Vector2 dashDirection;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDashing) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveX * moveSpeed;
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (moveX != 0)
            sr.flipX = moveX > 0;

        // Jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash(moveX));
        }
        Debug.Log("Grounded: " + isGrounded);


    }

    private System.Collections.IEnumerator Dash(float direction)
    {
        isDashing = true;
        dashTime = dashDuration;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * dashSpeed, 0);

        while (dashTime > 0)
        {
            dashTime -= Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
