using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    [Header("Settings")]
    [SerializeField] private float pickUpDistance = 5f;

    [Header("UI Elements")]
    [SerializeField] private GameObject pickUpPromptUI;
    [SerializeField] private TextMeshProUGUI pickUpText;

    private ObjectGrabbable objectGrabbable;

    private void Update()
    {
        // 1. Are we currently holding an object?
        if (objectGrabbable != null)
        {
            pickUpPromptUI.SetActive(true);
            pickUpText.text = "Press [E] to Drop";

            if (Input.GetKeyDown(KeyCode.E))
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
                pickUpPromptUI.SetActive(false);
            }
            return; // Stop running the rest of the code while holding something
        }

        // 2. We are NOT holding anything, shoot the raycast!
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            // Look for the Interface instead of a specific script!
            if (hit.transform.TryGetComponent(out IInteractable interactableObject))
            {
                // Ask the object what text it wants to display
                pickUpText.text = interactableObject.GetInteractPrompt();
                pickUpPromptUI.SetActive(true);

                // Allow either E or F to interact
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
                {
                    interactableObject.OnInteract();
                }
            }
            // Check for the Grabbable item (Since grabbing has special logic like holding/dropping)
            else if (hit.transform.TryGetComponent(out ObjectGrabbable grabbableTarget))
            {
                pickUpText.text = "Press [E] to Grab";
                pickUpPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    objectGrabbable = grabbableTarget;
                    objectGrabbable.Grab(objectGrabPointTransform);
                    pickUpPromptUI.SetActive(false);
                }
            }
            else
            {
                // Hit a normal wall
                pickUpPromptUI.SetActive(false);
            }
        }
        else
        {
            // Looking at the sky
            pickUpPromptUI.SetActive(false);
        }
    }
}