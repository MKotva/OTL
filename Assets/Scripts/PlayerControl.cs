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

    [Header("Engine Visuals")]
    [SerializeField] private EngineThrustControler[] engineThrustVisuals;

    private Vector3 velocity;

    private Quaternion shipRotation;

    private float yawVelocity;
    private float pitchVelocity;

    public Quaternion ShipRotation
    {
        get { return shipRotation; }
    }

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    void Start()
    {
        shipRotation = transform.rotation;
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

        // A / D = rotate left / right
        if (keyboard.aKey.isPressed)
            yawInput = -1f;

        if (keyboard.dKey.isPressed)
            yawInput = 1f;

        // Shift / Ctrl = rotate nose up / down
        if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
            pitchInput = -1f;

        if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
            pitchInput = 1f;

        // Engine thrust visuals
        float thrustAmount = forwardInput > 0f ? 1f : 0f;

        for (int i = 0; i < engineThrustVisuals.Length; i++)
        {
            if (engineThrustVisuals[i] != null)
                engineThrustVisuals[i].SetThrust(thrustAmount);
        }

        // Rotation inertia
        yawVelocity += yawInput * angularAcceleration * Time.deltaTime;
        pitchVelocity += pitchInput * angularAcceleration * Time.deltaTime;

        yawVelocity = Mathf.Clamp(yawVelocity, -maxAngularSpeed, maxAngularSpeed);
        pitchVelocity = Mathf.Clamp(pitchVelocity, -maxAngularSpeed, maxAngularSpeed);

        if (Mathf.Abs(yawInput) < 0.01f)
            yawVelocity = Mathf.Lerp(yawVelocity, 0f, angularDamping * Time.deltaTime);

        if (Mathf.Abs(pitchInput) < 0.01f)
            pitchVelocity = Mathf.Lerp(pitchVelocity, 0f, angularDamping * Time.deltaTime);

        float yawDelta = yawVelocity * Time.deltaTime;
        float pitchDelta = pitchVelocity * Time.deltaTime;

        // Rotate around the ship's LOCAL axes
        shipRotation = shipRotation * Quaternion.AngleAxis(yawDelta, Vector3.up);
        shipRotation = shipRotation * Quaternion.AngleAxis(pitchDelta, Vector3.right);

        transform.rotation = shipRotation;

        // Movement inertia
        Vector3 accelerationDirection = transform.forward * forwardInput;

        velocity += accelerationDirection * acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (Mathf.Abs(forwardInput) < 0.01f)
            velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }
}

