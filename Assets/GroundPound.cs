using UnityEngine;

public class GroundPound : MonoBehaviour
{
    public GameObject GroundPoundEffectsPrefab;
    public float poundForce = -10f;
    public float damageRadius = 5f;
    public LayerMask enemyLayer;

    private FrogPhysics frogPhysics;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isGroundPounding = false;
    private bool poundTriggeredInAir = false;

    void Start()
    {
        frogPhysics = GetComponent<FrogPhysics>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
    
        if (!isGroundPounding && !frogPhysics.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartGroundPound();
            }
        }
        if (frogPhysics.isGrounded && isGroundPounding)
        {
            FinishGroundPound();
        }
    }

    void StartGroundPound()
    {
        isGroundPounding = true;
        poundTriggeredInAir = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, poundForce); // smash attack

        if (animator != null)
        {
            animator.SetTrigger("isGroundPounding");
        }
    }

    void FinishGroundPound()
    {
        PerformGroundPoundImpact();
        isGroundPounding = false;
        poundTriggeredInAir = false;
    }

    void PerformGroundPoundImpact()
    {

        if (GroundPoundEffectsPrefab != null)
        {
        Instantiate(GroundPoundEffectsPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);

        foreach (Collider2D enemy in enemiesHit)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null)
            {
                e.OnHit(5);
            }
        }

        Debug.Log("Ground pound impact!");
    }

    public void GroundPoundImpact()
    {
        if (!frogPhysics.isGrounded)
    {
        Debug.LogWarning("Tried to apply ground pound impact before landing.");
        return;
    }

        PerformGroundPoundImpact();
        isGroundPounding = false;
        poundTriggeredInAir = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
