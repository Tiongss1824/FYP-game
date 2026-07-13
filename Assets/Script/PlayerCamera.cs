using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 300f; // Increased slightly as GetAxisRaw feels different
    [Range(0f, 0.2f)] public float lookSmoothTime = 0.05f; // Controls how "buttery" the movement is

    [Header("Camera Limits")]
    public float minViewAngle = -25f; // How far up you can look
    public float maxViewAngle = 35f;  // How far down you can look

    [Header("References")]
    public Transform playerBody;
    public Transform cameraTarget;

    // Track our target rotations
    private float targetXRotation = 0f;
    private float targetYRotation = 0f;

    // Track our current smoothed rotations
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    // Velocity references required by SmoothDamp
    private float xRotationVelocity;
    private float yRotationVelocity;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize target rotations to current physical rotations to prevent snapping on start
        targetXRotation = cameraTarget.localEulerAngles.x;
        targetYRotation = playerBody.eulerAngles.y;

        currentXRotation = targetXRotation;
        currentYRotation = targetYRotation;
    }

    void LateUpdate()
    {
        // 1. Get raw input. GetAxisRaw removes Unity's artificial input smoothing, 
        // which causes input lag. We want to apply our own mathematical smoothing instead.
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Calculate the desired target rotations
        targetXRotation -= mouseY;
        targetXRotation = Mathf.Clamp(targetXRotation, minViewAngle, maxViewAngle);

        targetYRotation += mouseX;

        // 3. Apply SmoothDamp for professional, buttery-smooth transitions
        currentXRotation = Mathf.SmoothDamp(currentXRotation, targetXRotation, ref xRotationVelocity, lookSmoothTime);
        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, targetYRotation, ref yRotationVelocity, lookSmoothTime);

        // 4. Apply the final smoothed rotations to the transforms
        cameraTarget.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }
}