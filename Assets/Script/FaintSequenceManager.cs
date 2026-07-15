using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using StarterAssets;

public class FaintSequenceManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image blackoutScreen;

    [Header("Player & Camera")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private Transform playerCamera;

    [Header("Teleport Target")]
    [SerializeField] private Transform merchantRoomSpawnPoint;

    // This is the public button the Old Man will push!
    public void StartFaintingSequence()
    {
        StartCoroutine(MasterTimeline());
    }

    private IEnumerator MasterTimeline()
    {
        // 1. Freeze the player
        playerController.enabled = false;

        // 2. Play the fall and fade to black
        yield return StartCoroutine(Phase1_FallAndFade());

        // 3. Teleport while the screen is completely black
        Phase2_TeleportBehindScenes();

        // 4. Wait for 2 seconds in the dark
        yield return new WaitForSeconds(2f);

        // 5. Wake up and clear the screen
        yield return StartCoroutine(Phase3_WakeUpAndClear());

        // Phase 4 has been completely deleted! You will wake up peacefully now.
        playerController.enabled = true;
    }

    private IEnumerator Phase1_FallAndFade()
    {
        float elapsed = 0f;
        float duration = 2f;

        Quaternion originalCamRot = playerCamera.localRotation;
        Quaternion fallCamRot = Quaternion.Euler(originalCamRot.eulerAngles.x, originalCamRot.eulerAngles.y, 75f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Fade to black
            Color c = blackoutScreen.color;
            c.a = Mathf.Clamp01(elapsed / duration);
            blackoutScreen.color = c;

            // Tilt camera
            playerCamera.localRotation = Quaternion.Slerp(originalCamRot, fallCamRot, elapsed / duration);

            yield return null;
        }
    }

    private void Phase2_TeleportBehindScenes()
    {
        CharacterController cc = playerController.GetComponent<CharacterController>();
        cc.enabled = false;

        playerController.transform.position = merchantRoomSpawnPoint.position;
        playerController.transform.rotation = merchantRoomSpawnPoint.rotation;

        cc.enabled = true;

        // Reset the camera tilt back to straight ahead
        playerCamera.localRotation = Quaternion.identity;
    }

    private IEnumerator Phase3_WakeUpAndClear()
    {
        float elapsed = 0f;
        float duration = 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Color c = blackoutScreen.color;
            c.a = Mathf.Clamp01(1f - (elapsed / duration));
            blackoutScreen.color = c;

            yield return null;
        }
    }
}