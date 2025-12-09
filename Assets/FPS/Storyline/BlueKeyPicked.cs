using UnityEngine;

public class KeyPickupMessageTrigger : MonoBehaviour
{
    public KeyPickup keyPickup;                 // Reference to the KeyPickup script
    public DialogueSystem dialogueSystem;       // Reference to DialogueSystem
    public ObjectiveUI objectiveUI;             // Reference to ObjectiveUI

    private bool messageShown = false;
    private bool objectiveSet = false;

    void Update()
    {
        if (!messageShown && keyPickup != null && dialogueSystem != null)
        {
            if (IsEventHappened())
            {
                messageShown = true;
                dialogueSystem.StartDialogue(new string[] { "Maybe this key will open that other door..." });
            }
        }

        if (messageShown && !objectiveSet && dialogueSystem != null && objectiveUI != null)
        {
            if (!dialogueSystem.DialogueIsActive)
            {
                objectiveUI.SetObjective("use the key on the other door");
                objectiveSet = true;
            }
        }
    }

    bool IsEventHappened()
    {
        var field = typeof(KeyPickup).GetField("eventhappened", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null && (bool)field.GetValue(keyPickup);
    }
}
