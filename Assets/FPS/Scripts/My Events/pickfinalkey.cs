using UnityEngine;
using System.Collections;

public class InteractableDestroy : MonoBehaviour
{
    public GameObject keyObject;
    public AudioSource audioSource;
    public AudioClip interactSound;
    public float interactDistance = 1f;
    public KeyCode interactKey = KeyCode.E;
    public Transform playerCamera;

    public bool keypickedup = false;

    public ObjectiveUI objectiveUI;           // Assign in Inspector
    public DialogueSystem dialogueSystem;     // Assign in Inspector
    public GameObject objectkeyaux;
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main?.transform;

        if (audioSource == null)
            Debug.LogWarning("AudioSource is not assigned.");

        if (keyObject == null)
            Debug.LogWarning("KeyObject is not assigned.");
    }

    void Update()
    {
        if (keypickedup || playerCamera == null || keyObject == null) 
            return;

        // Always measure distance between player camera and the key
        float distanceToKey = Vector3.Distance(playerCamera.position, keyObject.transform.position);

        if (distanceToKey <= interactDistance && Input.GetKeyDown(interactKey) && objectkeyaux.activeSelf)
        {
            // Optional: also check if the player is actually looking at the key
            Vector3 directionToKey = (keyObject.transform.position - playerCamera.position).normalized;
            if (Vector3.Dot(playerCamera.forward, directionToKey) > 0.5f) // 0.5 ~ 60° field of view
            {
                keypickedup = true;

                if (audioSource != null && interactSound != null)
                {
                    keyObject.SetActive(false);
                    audioSource.PlayOneShot(interactSound);
                }

                if (objectiveUI != null)
                    objectiveUI.SetObjective("leave the house");

                if (dialogueSystem != null)
                    dialogueSystem.ShowPassiveMessage("Let's get the hell out of here");
            }
        }
    }

    IEnumerator DestroyKeyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (keyObject != null)
            keyObject.SetActive(false);
    }
}
