using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Health: " + health);
    }
}
