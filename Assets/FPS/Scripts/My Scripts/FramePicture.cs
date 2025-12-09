using UnityEngine;

public class FrameInteraction : MonoBehaviour
{
    [Header("Hitboxes")]
    public Collider hitbox1;
    public Collider hitbox2;
    public bool wooshitsgone=false;

    [Header("Dialogue")]
    public DialogueSystem dialogueSystem;

    private bool canInteract = true;

    private readonly string[] message1 = {
        "There’s something off about this picture… like it’s alive."
    };

    private readonly string[] message2 = {
        "This photo holds all the grief I’ve tried to bury.",
        "Memories that won’t fade, no matter how hard I try."
    };

    void Update()
    {
        if(!wooshitsgone){
            if (!canInteract || !Input.GetKeyDown(KeyCode.E)) return;

            Camera cam = Camera.main;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (hit.collider == hitbox1 && IsFacingFront(hitbox1.transform))
                {
                    TriggerDialogue(message1);
                }
                else if (hit.collider == hitbox2 && IsFacingFront(hitbox2.transform))
                {
                    TriggerDialogue(message2);
                }
            }
        }
    }

    bool IsFacingFront(Transform target)
    {
        Vector3 toPlayer = (Camera.main.transform.position - target.position).normalized;
        float signedAngle = Vector3.SignedAngle(target.forward, toPlayer, Vector3.up);
        return signedAngle >= -30f && signedAngle <= 60f;
    }

    void TriggerDialogue(string[] lines)
    {
        canInteract = false;
        dialogueSystem.OnDialogueComplete += EnableInteraction;
        dialogueSystem.StartDialogue(lines);
    }

    void EnableInteraction()
    {
        canInteract = true;
        dialogueSystem.OnDialogueComplete -= EnableInteraction;
    }
}
