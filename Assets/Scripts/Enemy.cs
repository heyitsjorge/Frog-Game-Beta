using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 5;
    public float moveSpeed = 5;
    [SerializeField] float damage = 1;
    [SerializeField] bool doesContactDamage;


    [SerializeField] GameObject deathAnimationObject;

    public void Update()
    {
        // we'll put stuff in here that has to be called for every enemy.
        // then call base.Update to use it
        // just to reduce reused code
    } 

    public void OnHit(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            onDeath();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (doesContactDamage && collision.CompareTag("Player"))
        {
            Debug.Log("Hit " + collision.name);
            FrogPhysics player = collision.GetComponent<FrogPhysics>();
            if (player != null)
            {
                player.OnHit(damage);
            }
        }
    }
    public virtual void onDeath()
    {
        Instantiate(deathAnimationObject, gameObject.transform.position, gameObject.transform.localRotation);
        Destroy(gameObject);
    }
}
