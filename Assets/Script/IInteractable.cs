using UnityEngine;

// Notice it says "interface" instead of "class", and it does NOT use MonoBehaviour!
public interface IInteractable
{
    // Every interactable object must be able to provide text for the screen
    string GetInteractPrompt();

    // Every interactable object must have an action when pressed
    void OnInteract();
}