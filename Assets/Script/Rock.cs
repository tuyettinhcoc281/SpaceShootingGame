using UnityEngine;

public class Rock : MonoBehaviour
{
    private bool RotateDirection = false;
    private float rotateSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotateSpeed = Random.Range(0.5f, 2.5f);
        RotateDirection = Random.Range(0, 2) == 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (RotateDirection)
        {
            transform.Rotate(Vector3.forward * rotateSpeed);
        }
    }
}
