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
    }
}
