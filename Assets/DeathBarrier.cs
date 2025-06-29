using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FrogPhysics player = collision.GetComponent<FrogPhysics>();
            if (player != null)
            {
                player.OnDeath(); // Triggers your existing death method
            }
        }
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                Destroy(enemy); // trigger existing enemy death method
            }
        }
    }
}
