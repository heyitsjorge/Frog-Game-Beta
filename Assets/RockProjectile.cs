using UnityEngine;
public class RockProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private LayerMask hitMask;

    // Call this immediately after Instantiate
    public void Initialize(Vector2 dir, float spd, int dmg, LayerMask mask)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        hitMask = mask;
    }

    void Update()
    {
        // Move manually each frame
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only collide with layers in the hitMask
        // Damage the player
        var frog = collision.GetComponent<FrogPhysics>();
        if (frog != null)
        {
            frog.OnHit(damage);
            Destroy(gameObject);
            return;
        }

    }
}