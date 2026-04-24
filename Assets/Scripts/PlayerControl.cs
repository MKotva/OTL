using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 20f;
    public float maxSpeed = 18f;
    public float damping = 2f;

    [Header("Rotation")]
    public float angularAcceleration = 220f;
    public float maxAngularSpeed = 120f;
    public float angularDamping = 3f;

    private float yaw;
    private float pitch;

    private float yawVelocity;
    private float pitchVelocity;

    private Vector3 velocity;

    public Quaternion ShipRotation
    {
        get { return Quaternion.Euler(pitch, yaw, 0f); }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    void Start()
    {
        Vector3 startRotation = transform.eulerAngles;

        pitch = startRotation.x;
        yaw = startRotation.y;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        float forwardInput = 0f;
        float yawInput = 0f;
        float pitchInput = 0f;

        if (keyboard.wKey.isPressed)
            forwardInput = 1f;

        if (keyboard.sKey.isPressed)
            forwardInput = -1f;

        if (keyboard.aKey.isPressed)
            yawInput = -1f;

        if (keyboard.dKey.isPressed)
            yawInput = 1f;

        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
            pitchInput = -1f;

        if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
            pitchInput = 1f;

        // Rotation inertia
        yawVelocity += yawInput * angularAcceleration * Time.deltaTime;
        pitchVelocity += pitchInput * angularAcceleration * Time.deltaTime;

        yawVelocity = Mathf.Clamp(yawVelocity, -maxAngularSpeed, maxAngularSpeed);
        pitchVelocity = Mathf.Clamp(pitchVelocity, -maxAngularSpeed, maxAngularSpeed);

        // Slow rotation down when no rotation key is pressed
        if (Mathf.Abs(yawInput) < 0.01f)
            yawVelocity = Mathf.Lerp(yawVelocity, 0f, angularDamping * Time.deltaTime);

        if (Mathf.Abs(pitchInput) < 0.01f)
            pitchVelocity = Mathf.Lerp(pitchVelocity, 0f, angularDamping * Time.deltaTime);

        yaw += yawVelocity * Time.deltaTime;
        pitch += pitchVelocity * Time.deltaTime;

        transform.rotation = ShipRotation;

        // Movement inertia
        Vector3 accelerationDirection = ShipRotation * Vector3.forward * forwardInput;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (Mathf.Abs(forwardInput) < 0.01f)
            velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }
}
