using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 180f;

    void Update()
    {
        // Up / Down arrows = move forward / backward
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            moveInput = 1f;

        if (Input.GetKey(KeyCode.DownArrow))
            moveInput = -1f;

        // Left / Right arrows = rotate ship
        float rotationInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            rotationInput = 1f;

        if (Input.GetKey(KeyCode.RightArrow))
            rotationInput = -1f;

        // Move spaceship forward/backward
        transform.position += transform.forward * moveInput * moveSpeed * Time.deltaTime;

        // Rotate spaceship left/right
        transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }
}
