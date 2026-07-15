using System.Collections;
using UnityEngine;
using StarterAssets;

public class CinematicTrigger : MonoBehaviour
{
    [Header("Player Settings")]
    public FirstPersonController playerController;
    public Transform playerCameraRoot;

    [Header("NPC Settings")]
    public Transform npcFaceLocation;

    // --- CHANGED: Now it takes ANY game object instead of just NPCtalk ---
    [Tooltip("Drag the 3D model of the NPC here")]
    public GameObject npcCharacter;

    [Header("Settings")]
    public float turnSpeed = 1.0f;

    private bool hasTriggered = false;

    // This still works if you step into a box!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCutsceneManually();
        }
    }

    // --- NEW: A public button so the Basket can start the cutscene! ---
    public void StartCutsceneManually()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        // 1. Freeze the player
        playerController.enabled = false;

        float timeElapsed = 0;

        // 2. Save where the player is currently looking
        Quaternion startBodyRotation = playerController.transform.rotation;
        Quaternion startCameraRotation = playerCameraRoot.rotation;

        // 3. Calculate the math to look directly at the face
        Vector3 directionToFace = npcFaceLocation.position - playerCameraRoot.position;

        Vector3 bodyDirection = directionToFace;
        bodyDirection.y = 0;
        Quaternion targetBodyRotation = Quaternion.LookRotation(bodyDirection);

        Quaternion targetCameraRotation = Quaternion.LookRotation(directionToFace);

        // 4. Smoothly rotate
        while (timeElapsed < turnSpeed)
        {
            playerController.transform.rotation = Quaternion.Slerp(startBodyRotation, targetBodyRotation, timeElapsed / turnSpeed);
            playerCameraRoot.rotation = Quaternion.Slerp(startCameraRotation, targetCameraRotation, timeElapsed / turnSpeed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerController.transform.rotation = targetBodyRotation;
        playerCameraRoot.rotation = targetCameraRotation;

        // 5. --- UPGRADED: Ask the NPC for its contract and press Interact! ---
        if (npcCharacter != null && npcCharacter.TryGetComponent(out IInteractable interactableNPC))
        {
            interactableNPC.OnInteract();
        }
    }
}