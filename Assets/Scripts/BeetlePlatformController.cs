using UnityEngine;

public class BeetlePlatformController : MonoBehaviour
{
    private GameObject player;
    private bool active = true;
    private bool playerInTrigger = false;
    [SerializeField] private BoxCollider2D platform;
    [SerializeField] private BoxCollider2D playerDetector;
    [SerializeField] private GameObject beetle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
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
    // Update is called once per frame
    void Update()
    {
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
