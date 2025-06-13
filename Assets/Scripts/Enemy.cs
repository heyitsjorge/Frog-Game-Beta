using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health = 5;
    public float moveSpeed = 5;
    [SerializeField] float damage = 1;
    [SerializeField] bool doesContactDamage;

    private float enemyDamagedTimer = 0;
    private float tempSpeed;
    bool damaged = false;
    private SpriteRenderer enemySprite;
    [SerializeField] Color flashColor;
    private Color tempColor;
    [SerializeField] GameObject deathAnimationObject;
    [SerializeField] bool isImmortal = false;

    public void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();
    }
    public void Update()
    {
        // we'll put stuff in here that has to be called for every enemy.
        // then call base.Update to use it
        // just to reduce reused code
        enemyDamagedTimer -= Time.deltaTime;
        if (enemyDamagedTimer < 0 && damaged)
        {
            damaged = false;
            moveSpeed = tempSpeed;
            enemySprite.color = tempColor;
        }
    } 

    public virtual void OnHit(float damage)
    {
        if (enemyDamagedTimer <= 0)
        {
            health -= damage;
            enemyDamagedTimer = 0.2f;
            damaged = true;
            tempSpeed = moveSpeed;
            moveSpeed = 0;
            tempColor = enemySprite.color;
            enemySprite.color = flashColor;
        }
        if (health <= 0 && !isImmortal)
        {
            enemySprite.color = tempColor;
            OnDeath();

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionHelper(collision);
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
    public virtual void CollisionHelper(Collider2D collision)
    {
        
    }
    public virtual void OnDeath()
    {
        Instantiate(deathAnimationObject, gameObject.transform.position, gameObject.transform.localRotation);
        Destroy(gameObject);
    }
}
