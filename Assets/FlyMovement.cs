using UnityEngine;

public class FlyMovement : MonoBehaviour
{
    private Rigidbody2D rb; //rigidbody of the fly
    private SpriteRenderer sr; //sprite renderer of the fly

    public float speed;
    private GameObject player;
    private bool Chase = false;
    public float detectionRadius = 5f; // Radius within which the fly will chase the player

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        else
        {
            // Move the fly in a random direction
            float moveX = Random.Range(-1f, 1f);
            float moveY = Random.Range(-1f, 1f);
            Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
            rb.linearVelocity = moveDirection * speed;
            // Flip sprite based on direction
            if (moveX != 0)
                sr.flipX = moveX > 0;
            // Check if the fly is close to the player
            if (detectionRadius > Vector2.Distance(transform.position, player.transform.position))
            {
                Chase = true;
                ChasePlayer(); // Start chasing the player
            }
        
    }
    void ChasePlayer()
    {
        detectionRadius = 10f; // increase the distance hell chase you for
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
}
