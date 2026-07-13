using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    private CharacterController controller;
    private float verticalVelocity;

    // The reference to your new Animator
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // This automatically finds the Animator on your downloaded 3D model!
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // SAFETY CHECK: Stop moving and go back to Idle if talking to Pinut
        if (controller.enabled == false)
        {
            if (animator != null)
            {
                animator.SetFloat("InputX", 0f);
                animator.SetFloat("InputY", 0f);
            }
            return;
        }

        // 1. Get raw WASD Input (-1, 0, or 1)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 2. Handle Gravity & Jumping
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        verticalVelocity += gravity * Time.deltaTime;

        // 3. Handle Horizontal Strafing Movement
        Vector3 move = transform.right * x + transform.forward * z;

        // Normalize so diagonal walking isn't too fast
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // Move the physical character
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 4. Update the Animator Blend Tree!
        if (animator != null)
        {
            // The "0.1f" smooths out the animation so it doesn't snap instantly
            animator.SetFloat("InputX", x, 0.1f, Time.deltaTime);
            animator.SetFloat("InputY", z, 0.1f, Time.deltaTime);
        }

        // 5. Apply the vertical movement (Gravity)
        controller.Move(new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }
}