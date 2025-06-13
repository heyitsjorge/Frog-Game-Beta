using UnityEngine;

public class BeetleSpawner : MonoBehaviour
{
    [SerializeField] float spawningDelay = 20;
    [SerializeField] bool spawnFacingRight = true;
    [SerializeField] BeetleMovementController beetle;
    [SerializeField] float spawnedMoveSpeed = 3;
    private float tempDelay;
    void Start()
    {
        tempDelay = spawningDelay;
        beetle.facingRight = spawnFacingRight;
        beetle.moveSpeed = spawnedMoveSpeed;
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
