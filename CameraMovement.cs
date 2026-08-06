using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private InputManager input;

    private float sensitivity = 0.15f;
    private float xRot;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        Look();
    }

    private void Look()
    {
        Vector2 direction = input.GetCameraRotation();

        float mouseX = direction.x * sensitivity;
        float mouseY = direction.y * sensitivity;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerBody.MoveRotation(playerBody.rotation * Quaternion.Euler(0f, mouseX, 0f));
    }
}
