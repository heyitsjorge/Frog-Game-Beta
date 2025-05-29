using UnityEngine;

public class DestroyBeetleGoo : MonoBehaviour
{
    [SerializeField] private float delay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float animTime = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        
        Destroy(gameObject, animTime + delay - 0.001f);
    }
}

