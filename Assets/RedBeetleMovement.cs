using UnityEngine;

public class RedBeetleMovement : Enemy
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;

    private bool movingRight = false;  
    private bool isDying = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        movingRight = false;
        sr.flipX = false;  
    }

    void Update()
    {
        if (isDying) return;

        rb.linearVelocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);


        sr.flipX = movingRight;
    }

    public override void CollisionHelper(Collider2D collision)
    {
        if (isDying) return;

        if (collision.CompareTag("EdgeTrigger"))
        {

            movingRight = !movingRight;
        }
    }
    public override void OnDeath()
    {
        animator.SetTrigger("isDead");
        isDying = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void DestroyBeetle()
    {
        Destroy(transform.gameObject);
    }
}
