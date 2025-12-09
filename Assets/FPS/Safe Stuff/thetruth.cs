using UnityEngine;

public class SimpleInteraction : MonoBehaviour
{
    public GameObject objectToDisable;  // Assign the interactable object here
    public DialogueSystem dialogueSystem;  // Assign your DialogueSystem instance here
    public GameObject pictureframe1;
    public GameObject pictureframe2;
    private string[] dialogueLines = new string[]
    {
    "Every night, I return to this place...",
    "I remember the silence that followed her absence.",
    "She was only twelve...",
    "And I still don’t know how to let her go.",
    "When I leave this house..." ,
    "This dream...",
    "There won’t be anyone on the other side."
    };

    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.gameObject == objectToDisable)
                {
                    objectToDisable.SetActive(false);
                    if (dialogueSystem != null)
                    {
                        dialogueSystem.StartDialogue(dialogueLines);
                    }

                    pictureframe2.SetActive(true);
                    pictureframe1.SetActive(false);
                }
            }
        }

        // Debug: Trigger lockdown directly with Y key
    }
}
