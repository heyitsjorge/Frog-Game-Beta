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
        Debug.Log("Kunai hit: " + collision.name);

        bool didHit = false;

        // 1) Hit generic Enemy?
        var enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnHit(1);
            didHit = true;
        }

        // 2) Hit Yama (boss)?
        var boss = collision.GetComponentInParent<Boss_Health>();
        if (boss != null)
        {
            boss.OnHit(1);
            didHit = true;
        }

        if (didHit)
            Destroy(gameObject);
    }
}
