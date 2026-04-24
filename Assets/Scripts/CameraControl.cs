using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 3f, -10f);

    [Header("Look At Offset")]
    public Vector3 lookAtOffset = new Vector3(0f, 0.5f, 4f);

    [Header("Smoothing")]
    public float positionSmoothSpeed = 8f;
    public float rotationSmoothSpeed = 8f;

    private PlayerControl playerControl;

    void Start()
    {
        if (target != null)
            playerControl = target.GetComponent<PlayerControl>();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion shipRotation = target.rotation;

        if (playerControl != null)
            shipRotation = playerControl.ShipRotation;

        Vector3 desiredPosition = target.position + shipRotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmoothSpeed * Time.deltaTime
        );

        Vector3 lookTarget = target.position + shipRotation * lookAtOffset;

        Quaternion desiredRotation = Quaternion.LookRotation(
            lookTarget - transform.position,
            shipRotation * Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}
