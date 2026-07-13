using UnityEngine;
using TMPro; // <--- NEW: Allows us to change the text letters!

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    [Header("Settings")]
    [SerializeField] private float pickUpDistance = 5f;

    [Header("UI Elements")]
    [SerializeField] private GameObject pickUpPromptUI; // The UI object we turn on/off
    [SerializeField] private TextMeshProUGUI pickUpText; // <--- NEW: The actual text component!

    private ObjectGrabbable objectGrabbable;

    private void Update()
    {
        if (objectGrabbable == null)
        {
            // Shoot the raycast
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, pickUpDistance, pickUpLayerMask))
            {
                // 1. Did we hit an ITEM?
                if (hit.transform.TryGetComponent(out ObjectGrabbable grabbableTarget))
                {
                    pickUpText.text = "Press [E] to Grab"; // CHANGE TEXT
                    pickUpPromptUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        objectGrabbable = grabbableTarget;
                        objectGrabbable.Grab(objectGrabPointTransform);
                        pickUpPromptUI.SetActive(false);
                    }
                }
                // 2. Did we hit a DOOR?
                else if (hit.transform.TryGetComponent(out DoorController doorTarget))
                {
                    pickUpText.text = "Press [E] to Open"; // CHANGE TEXT
                    pickUpPromptUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        doorTarget.ToggleDoor();
                    }
                }
                // 3. Did we hit an NPC?
                else if (hit.transform.TryGetComponent(out NPCtalk npcTarget))
                {
                    pickUpText.text = "Press [F] to Talk"; // CHANGE TEXT
                    pickUpPromptUI.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.F)) // Listens for F!
                    {
                        npcTarget.Interact();
                    }
                }
                // 4. We hit a normal wall/floor
                else
                {
                    pickUpPromptUI.SetActive(false);
                }
            }
            else
            {
                // We look at the empty sky
                pickUpPromptUI.SetActive(false);
            }
        }
        else
        {
            // We are holding something, give dropping instructions
            pickUpPromptUI.SetActive(true);
            pickUpText.text = "Press [E] to Drop"; // Optional: Tell them how to drop!

            if (Input.GetKeyDown(KeyCode.E))
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
                pickUpPromptUI.SetActive(false);
            }
        }
    }
}