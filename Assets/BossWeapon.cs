using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BossWeapon : MonoBehaviour
{
    public Transform playerTransform;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int meleeDamage = 4;

    public Vector3 attackOffset;

    public float meleeRange = 3f;
   
    public LayerMask attackMask;

    //dash attack variables
    public bool isDashing = false;
    public float dashAttackRange = 6f;
    public float dashDamage = 3f;
    public float dashSpeed = 10f;
    public float dashHitBoxRadius = 1.5f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 3f;
    public float dashCooldownTimer = 0f;

    //Dodging varaibles
    public bool isDodging = false;
    //ranged attack variables
    public int rangedDamage = 10;
    public float rangedRange = 10f;

    public GameObject rockPrefab;
    public Transform rockSpawnPoint;
    public float      projectileSpeed = 8f;
    public float      rangedCooldown  = 2f;
    public float     rangedCooldownTimer = 0f;

    public LayerMask rangedMask;

    public void meleeAttack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, meleeRange, attackMask);
        if (colInfo != null)
        {
            if (colInfo.CompareTag("Player"))
            {
                FrogPhysics playerFrog = colInfo.GetComponent<FrogPhysics>();
                if (playerFrog != null)
                {
                    playerFrog.OnHit(meleeDamage); // This will apply damage and handle death etc.
                }

            }

        }

    }

    public void TickDashCooldown()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    public void dashMovement()
{
    // Only start if not already dashing and cooldown is up
    if (isDashing || dashCooldownTimer > 0f) return;
    StartCoroutine(DoDash());
}

private IEnumerator DoDash()
{
    isDashing = true;
    dashCooldownTimer = dashCooldown;

    float elapsed = 0f;
    // Calculate direction once up front
    float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);

    // Run in FixedUpdate context for consistent movement
    while (elapsed < dashDuration)
    {
        // Move the transform directly
        transform.position += new Vector3(dirX * dashSpeed * Time.fixedDeltaTime, 0f, 0f);

        elapsed += Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }

    isDashing = false;
}


    public void dashAttack()
    {
        // Same hitbox check, separate from movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Collider2D hit = Physics2D.OverlapCircle(rb.position, dashHitBoxRadius, attackMask);
        if (hit != null && hit.CompareTag("Player"))
        {
            FrogPhysics frog = hit.GetComponent<FrogPhysics>();
            if (frog != null) frog.OnHit(dashDamage);
        }
    }
    public void StopDash()
    {
        isDashing = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void Update()
    {
        // tick your ranged cooldown back toward zero
        if (rangedCooldownTimer > 0f)
            rangedCooldownTimer -= Time.deltaTime;
    }

    public void RangedAttack()
    {
        // Only fire if off cooldown and prefab assigned
        if (rangedCooldownTimer > 0f || rockPrefab == null || playerTransform == null)
            return;

        // Determine spawn position
        Vector3 spawnPos = rockSpawnPoint != null
            ? rockSpawnPoint.position
            : transform.position;

        // Compute direction towards player
        Vector2 dir = (playerTransform.position - spawnPos).normalized;

        // Instantiate and initialize projectile
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
        var proj = rock.GetComponent<RockProjectile>();
        if (proj != null)
        {
            proj.Initialize(dir, projectileSpeed, rangedDamage, rangedMask);
        }
        else
        {
            // Fallback: direct Rigidbody2D velocity
            var rb = rock.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = dir * projectileSpeed;
        }

        // Restart cooldown
        rangedCooldownTimer = rangedCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, meleeRange);

        // Dash hitbox
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dashHitBoxRadius);
    }


}

