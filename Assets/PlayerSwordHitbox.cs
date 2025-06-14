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


        Enemy enemy = collision.GetComponent<Enemy>();
        if (collision.CompareTag("Enemy") && enemy != null)
        {
            enemy.OnHit(1);
        }

        Boss_Health boss = collision.GetComponentInParent<Boss_Health>();
        {
            if (boss != null)
            {
                boss.OnHit(1);
                Debug.Log("Boss hit for 1 damage");
            }
        }
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
