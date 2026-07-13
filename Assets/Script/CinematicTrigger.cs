using System.Collections;
using UnityEngine;
using StarterAssets;

public class CinematicTrigger : MonoBehaviour
{
    [Header("Player Settings")]
    public FirstPersonController playerController;
    [Tooltip("Drag the PlayerCameraRoot from inside the PlayerCapsule here")]
    public Transform playerCameraRoot; // <--- NEW: Controls the Up/Down tilt

    [Header("NPC Settings")]
    [Tooltip("Drag the new FaceTarget object here")]
    public Transform npcFaceLocation; // <--- NEW: The exact spot between the eyes
    public NPCtalk npcScript;

    [Header("Settings")]
    public float turnSpeed = 1.0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
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

        // The BODY only turns left and right (Keep Y at 0)
        Vector3 bodyDirection = directionToFace;
        bodyDirection.y = 0;
        Quaternion targetBodyRotation = Quaternion.LookRotation(bodyDirection);

        // The CAMERA tilts to look exactly at the face
        Quaternion targetCameraRotation = Quaternion.LookRotation(directionToFace);

        // 4. Smoothly rotate both the body and the head over time
        while (timeElapsed < turnSpeed)
        {
            playerController.transform.rotation = Quaternion.Slerp(startBodyRotation, targetBodyRotation, timeElapsed / turnSpeed);
            playerCameraRoot.rotation = Quaternion.Slerp(startCameraRotation, targetCameraRotation, timeElapsed / turnSpeed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the exact final angle to be safe
        playerController.transform.rotation = targetBodyRotation;
        playerCameraRoot.rotation = targetCameraRotation;

        // 5. Start talking!
        npcScript.Interact();
    }
}