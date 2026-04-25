using UnityEngine;

public class AsteroidMotionController : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        ApplyMotion();
    }

    public void ApplyMotion()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
            return;

        rb.useGravity = false;


        rb.linearVelocity = velocity;
        rb.angularVelocity = rotationAxis.normalized * rotationSpeed * Mathf.Deg2Rad;
    }

    void Update()
    {
        if (rb != null)
            return;

        transform.position += velocity * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }
}
