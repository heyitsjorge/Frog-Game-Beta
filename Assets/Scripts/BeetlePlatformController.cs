using UnityEngine;

public class BeetlePlatformController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private bool active;
    private bool playerInTrigger = false;
    [SerializeField] private BoxCollider2D platform;
    [SerializeField] private BoxCollider2D playerDetector;
    [SerializeField] private GameObject beetle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger.");
            // Perform actions when the player enters the trigger.
            playerInTrigger = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger.");
            // Perform actions when the player enters the trigger.
            playerInTrigger = false;
        }
    }
    void Start()
    {
        active = player.transform.position.y > gameObject.transform.position.y;
        platform = gameObject.GetComponent<BoxCollider2D>();
        platform.enabled = active;

    }
    // Update is called once per frame
    void Update()
    {
        if (!active && player.transform.position.y - 1.5 > gameObject.transform.position.y)
        {
            platform.enabled = true;
            active = true;
            Debug.Log("activating");
        }
        else if (active && player.transform.position.y - 1.5 < gameObject.transform.position.y)
        {
            platform.enabled = false;
            active = false;
            Debug.Log("deactivating");
        }

        if (active && playerInTrigger && Input.GetAxisRaw("Horizontal") == 0)
        {
            // Vector2 tempVelocity = new Vector2(beetle.transform.GetComponent<Rigidbody2D>().linearVelocity.x, player.transform.GetComponent<Rigidbody2D>().linearVelocity.y);
            // player.transform.GetComponent<Rigidbody2D>().linearVelocity = tempVelocity;
            Vector3 tempPosition = new Vector3(beetle.transform.position.x,
                                        player.transform.position.y, 
                                        player.transform.position.z);
            player.transform.position = tempPosition;
            Debug.Log("fixing player's position");
        }
    }
}
