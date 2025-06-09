using UnityEngine;

public class RedBeetleMovement : Enemy
{
    public AudioClip deathSound;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;

    public float speed = 2f;
    private bool movingRight = false;
    private bool isDying = false;

    public float damageRadius = 0.5f;
    public float explodeRadius = 0.7f;
    [SerializeField] private LayerMask playerLayer;

    private Collider2D playerCollider;

    public float damageCooldown = 1f;
    private float damageTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
            }
        }
    }

    void Update()
    {
        if (isDying) return;

        rb.linearVelocity = new Vector2(movingRight ? speed : -speed, rb.linearVelocity.y);
        sr.flipX = movingRight;

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }

        if (damageTimer <= 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, playerLayer);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    FrogPhysics player = hit.GetComponent<FrogPhysics>();
                    if (player != null)
                    {
                        player.OnHit(1);
                        damageTimer = damageCooldown;
                        break;
                    }
                }
            }
        }
    }

    public override void CollisionHelper(Collider2D collision)
    {
        if (isDying) return;

        if (collision.CompareTag("EdgeTrigger"))
        {
            movingRight = !movingRight;
        }
    }

    public override void OnHit(float damage)
    {
        if (isDying) return;

        base.OnHit(damage); 

        if (health <= 0) 
        {
            OnDeath();
        }
    }

    public override void OnDeath()
    {
        if (isDying) return;

        isDying = true;
        animator.SetTrigger("isDead");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        //explode
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
        if (hit.CompareTag("Player"))
        {
            FrogPhysics player = hit.GetComponent<FrogPhysics>();
            if (player != null)
            {
                player.OnHit(1);
            }
        }
        }

        Invoke("DestroyBeetle", 0.5f);
    }

    public void DestroyBeetle()
    {
        Destroy(transform.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
