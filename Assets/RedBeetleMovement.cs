using UnityEngine;

public class RedBeetleMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;

    public float speed = 2f;
    private bool movingRight = false;  
    private bool isDying = false;
    [SerializeField] private float health = 3f;

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

        rb.linearVelocity = new Vector2(movingRight ? speed : -speed, rb.linearVelocity.y);


        sr.flipX = movingRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying) return;

        if (collision.CompareTag("EdgeTrigger"))
        {

            movingRight = !movingRight;
        }
        else if (collision.CompareTag("Player"))
        {
            Debug.Log("Beetle collided with player " + collision.name);
            FrogPhysics player = collision.GetComponent<FrogPhysics>();
            if (player != null)
            {
                player.OnHit(1);
            }
        }
    }

    public void OnHit(float damage)
    {
        if (isDying) return;

        health -= damage;

        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
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
