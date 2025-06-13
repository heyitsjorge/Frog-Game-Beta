using UnityEngine;

public class Boss_Health : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float maxHP = 20f;
    public float currHP;
    void Awake()
    {
        currHP = maxHP;
    }
    public void OnHit(float damage)
    {
        currHP -= damage;
        if (currHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("Boss has been defeated!");
        // Here you can add logic for what happens when the boss dies
        Destroy(gameObject); // 
    }

}
