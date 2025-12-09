using UnityEngine;
using SojaExiles;
using System.Collections;

public class LivingRoomTrigger : MonoBehaviour
{
    public FirstEventTrigger firstEventTrigger;
    public PlayerLocationTracker locationTracker;
    public bool eventcompleted = false;
    public DBLockedDoor unlockdoor;

    public AudioSource audioSource;
    public AudioClip enterLivingRoomSound;
    public Lockdown ld;
    public DialogueSystem dialogueSystem; // Reference to DialogueSystem
    public string passiveMessage = "Jesus christ... what's happening?";

    private void OnTriggerEnter(Collider other)
    {
        if (!eventcompleted && !ld.lockdownActive)
        {
            if (other.CompareTag("Player") &&
                firstEventTrigger.keyPickedUp &&
                locationTracker.playerIsIn == PlayerLocationTracker.PlayerLocationState.LivingRoom)
            {
                firstEventTrigger.OnPlayerEnterLivingRoom();

                if (audioSource != null && enterLivingRoomSound != null)
                {
                    audioSource.PlayOneShot(enterLivingRoomSound);
                    StartCoroutine(ShowMessageAfterDelay(enterLivingRoomSound.length));
                }
                else
                {
                    ShowPassiveMessage();
                }

                unlockdoor.SetHasKey();
                eventcompleted = true;
            }
        }
    }

    IEnumerator ShowMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPassiveMessage();
    }

    void ShowPassiveMessage()
    {
        if (dialogueSystem != null)
            dialogueSystem.ShowPassiveMessage(passiveMessage);
    }
}
