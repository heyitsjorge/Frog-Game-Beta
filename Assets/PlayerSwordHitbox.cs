using UnityEngine;

public class PlayerSwordHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit " + collision.name);
        if (collision.CompareTag("FlyEnemy"))
        {
            FlyMovement enemy = collision.GetComponent<FlyMovement>();
            enemy.OnHit(1);
        }

        else if (collision.CompareTag("MarioBeetle"))
        {
            BeetleMovementController enemy = collision.GetComponent<BeetleMovementController>();
            enemy.OnHit(1);
        }
        else if (collision.CompareTag("Red_Beetle"))
        {
            RedBeetleMovement enemy = collision.GetComponent<RedBeetleMovement>();
            enemy.OnHit(1);
        }
    }
}
