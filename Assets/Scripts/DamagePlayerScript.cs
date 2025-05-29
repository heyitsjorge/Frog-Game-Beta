using UnityEngine;

public class DamagePlayerScript : MonoBehaviour
{
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
}
