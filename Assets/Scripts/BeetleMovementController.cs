using UnityEngine;

public class BeetleMovementController : Enemy
{
    [SerializeField] float dropMultiplier;

    [SerializeField] Transform floorDetector;
    [SerializeField] Transform wallDetector;
    [SerializeField] Transform groundedDetector;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private GameObject deathAnimation;

    public bool facingRight;


    [SerializeField] bool walkOffPlatform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        GetComponent<Rigidbody2D>().gravityScale = dropMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Vector2 tempVelocity = gameObject.GetComponent<Rigidbody2D>().linearVelocity;

        if (Physics2D.OverlapCircle(wallDetector.position, 0.05f, wallLayer)
            || Physics2D.OverlapCircle(wallDetector.position, 0.05f, enemyLayer))
        {
            Debug.Log("green koopa flip");
            FlipSprite();
        }
        else if (!walkOffPlatform && !Physics2D.OverlapCircle(floorDetector.position, 0.05f, wallLayer) && Physics2D.OverlapCircle(groundedDetector.position, 0.05f, wallLayer))
        {
            Debug.Log("red koopa flip");
            FlipSprite();
        }

        tempVelocity.x = facingRight ? moveSpeed : -moveSpeed;

        tempVelocity.y = gameObject.GetComponent<Rigidbody2D>().linearVelocityY;

        if (tempVelocity.y > 0)
        {
            tempVelocity.y = 0;
        }

        gameObject.GetComponent<Rigidbody2D>().linearVelocity = tempVelocity;

    }

    void FlipSprite()
    {
        Vector3 temp = transform.localScale;
        temp.x = temp.x * -1;
        transform.localScale = temp;
        facingRight = !facingRight;
    }
}
