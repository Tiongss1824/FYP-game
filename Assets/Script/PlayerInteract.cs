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
        // 1. Are we currently holding an object? (Like a vegetable)
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
            // Look for the Interface (NPCs and Doors)
            if (hit.transform.TryGetComponent(out IInteractable interactableObject))
            {
                pickUpText.text = interactableObject.GetInteractPrompt();
                pickUpPromptUI.SetActive(true);

                // --- SMART KEY SORTING ---
                // If it is an NPC, demand the 'F' key to talk
                if (interactableObject is NpcTalk || interactableObject is MerchantNpc)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        interactableObject.OnInteract();
                    }
                }
                // If it is anything else (like a Door), demand the 'E' key to interact
                else
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactableObject.OnInteract();
                    }
                }
            }
            // Look for the Grabbable item (Vegetables)
            else if (hit.transform.TryGetComponent(out ObjectGrabbable grabbableTarget))
            {
                pickUpText.text = "Press [E] to Grab";
                pickUpPromptUI.SetActive(true);

                // Grabbing strictly uses the 'E' key
                if (Input.GetKeyDown(KeyCode.E))
                {
                    objectGrabbable = grabbableTarget;
                    objectGrabbable.Grab(objectGrabPointTransform);
                    pickUpPromptUI.SetActive(false);
                }
            }
            else
            {
                pickUpPromptUI.SetActive(false);
            }
        }
        else
        {
            pickUpPromptUI.SetActive(false);
        }
    }
}