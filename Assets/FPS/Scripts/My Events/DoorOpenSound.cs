using UnityEngine;
using SojaExiles;

public class DoorSoundOnEvent : MonoBehaviour
{
    public FirstEventTrigger2 eventTrigger;  // Assign the event trigger object here
    public AudioClip doorSound;              // Assign the door sound clip here
    private AudioSource audioSource;
    private bool eventSubscribed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (eventTrigger != null && !eventSubscribed)
        {
            eventTrigger.OnEventTriggered += PlayDoorSound;
            eventSubscribed = true;
        }
    }

    void OnDestroy()
    {
        if (eventTrigger != null && eventSubscribed)
        {   
            eventTrigger.OnEventTriggered -= PlayDoorSound;
        }
    }

    void PlayDoorSound()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
                    Debug.Log("Door sound played.");
        }
    }
}
