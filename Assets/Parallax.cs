using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;

    Vector2 startPos;

    float startZ; 

    Vector2 travel => (Vector2)cam.transform.position - startPos;

    float distanceFromSubject => transform.position.z - subject.position.z;
    
    float clippingPlane => cam.transform.position.z + (distanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane);

    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

    void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z; 
    }

    void Update()
    {
        Vector2 newPos = startPos + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);

    }
}
