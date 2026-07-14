using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion ajarRotation; // The slightly open hint position
    private Quaternion targetRotation;

    [Tooltip("How fast the door swings open")]
    [SerializeField] private float swingSpeed = 5f;

    [Header("Level Design Hints")]
    [Tooltip("Check this to make the door start slightly open.")]
    [SerializeField] private bool startAjar = false;
    [Tooltip("How many degrees open should the hint be?")]
    [SerializeField] private float ajarAngle = 20f;

    private void Start()
    {
        // 1. Remember the perfectly closed position
        closedRotation = transform.localRotation;

        // 2. Calculate the fully open position (90 degrees)
        openRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + 90f, transform.localEulerAngles.z);

        // 3. Calculate the slightly open hint position (e.g., 20 degrees)
        ajarRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + ajarAngle, transform.localEulerAngles.z);

        // 4. Setup the starting position
        if (startAjar)
        {
            targetRotation = ajarRotation;
            // Instantly snap it to the cracked-open position so we don't see it swing when the game loads
            transform.localRotation = ajarRotation;
        }
        else
        {
            targetRotation = closedRotation;
        }
    }

    private void Update()
    {
        // Smoothly rotate the door towards the target position every frame
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swingSpeed);
    }

    public void ToggleDoor()
    {
        // Toggle the state
        isOpen = !isOpen;

        // Set the new target rotation (it will ignore the 'ajar' angle forever now)
        targetRotation = isOpen ? openRotation : closedRotation;
    }

    public string GetInteractPrompt()
    {
        return "Press [E] to Interact";
    }

    public void OnInteract()
    {
        ToggleDoor();
    }
}