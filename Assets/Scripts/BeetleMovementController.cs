using UnityEngine;

public class BeetleMovementController : MonoBehaviour
{

    [SerializeField] float health;
    [SerializeField] float speed;
    [SerializeField] float dropMultiplier;

    [SerializeField] Transform floorDetector;
    [SerializeField] Transform wallDetector;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private GameObject deathAnimation;

    [SerializeField] bool facingRight;


    [SerializeField] bool walkOffPlatform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 tempVelocity = gameObject.GetComponent<Rigidbody2D>().linearVelocity;

        if (Physics2D.OverlapCircle(wallDetector.position, 0.05f, wallLayer) || Physics2D.OverlapCircle(wallDetector.position, 0.05f, enemyLayer)
        && Physics2D.OverlapCircle(floorDetector.position, 0.05f, wallLayer))
        {
            FlipSprite();
        }
        else if (!walkOffPlatform && (!Physics2D.OverlapCircle(floorDetector.position, 0.05f, wallLayer)))
        {
            FlipSprite();
        }

        if (facingRight)
        {
            tempVelocity.x = transform.right.x * speed * Time.fixedDeltaTime;
        }
        else
        {
            tempVelocity.x = transform.right.x * speed * Time.fixedDeltaTime * -1;
        }
        tempVelocity.y = gameObject.GetComponent<Rigidbody2D>().linearVelocityY * dropMultiplier;


        gameObject.GetComponent<Rigidbody2D>().linearVelocity = tempVelocity;

    }

    void FlipSprite()
    {
        Vector3 temp = transform.localScale;
        temp.x = temp.x * -1;
        transform.localScale = temp;
        facingRight = !facingRight;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Hit " + collision.name);
            FrogPhysics player = collision.GetComponent<FrogPhysics>();
            if (player != null)
            {
                player.OnHit(1);
            }
        }
    }

    public void OnHit(float damage){
        health -= damage;
        if (health <= 0)
        {
            Instantiate(deathAnimation, gameObject.transform.position, gameObject.transform.localRotation);
            Destroy(gameObject);
        }
    }
}
