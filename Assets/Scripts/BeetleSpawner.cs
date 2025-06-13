using UnityEngine;

public class BeetleSpawner : MonoBehaviour
{
    [SerializeField] float spawningDelay = 20;
    [SerializeField] bool spawnFacingRight = true;
    [SerializeField] BeetleMovementController beetle;
    private float tempDelay;
    void Start()
    {
        tempDelay = spawningDelay;
        beetle.facingRight = spawnFacingRight;
    }
    void FixedUpdate()
    {
        tempDelay -= 1;
        if (tempDelay <= 0)
        {
            tempDelay = spawningDelay;
            Instantiate(beetle,
                gameObject.transform.position,
                gameObject.transform.localRotation);
        }
    }
}
