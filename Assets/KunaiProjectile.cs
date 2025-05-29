using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    public void InititializeProjectile(Vector2 direction, Rigidbody2D rb, SpriteRenderer sr)
    {
        if (rb == null || sr == null)
        {
            Debug.LogError("Rigidbody2D or SpriteRenderer is not assigned.");
            return;
        }
        sr.flipX = direction.x < 0;
        rb.linearVelocity = direction * moveSpeed;
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
        Destroy(gameObject);
    }
}
