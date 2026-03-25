using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private InputManager input;

    private float sensitivity = 30f;
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

        float mouseX = direction.x * sensitivity * Time.deltaTime;
        float mouseY = direction.y * sensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}