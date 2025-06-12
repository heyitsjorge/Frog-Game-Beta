using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BossWeapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int meleeDamage = 4;
    public int rangedDamage = 10;

    public Vector3 attackOffset;

    public float meleeRange = 3f;
    public float rangedRange = 10f;
    public LayerMask attackMask;

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
    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, meleeRange);
    }




}

