using System.Collections;
using UnityEngine;

public class FirstEventTrigger : MonoBehaviour
{
    public Transform otherDoor;
    public float eventOpenAngle = 45f;
    public float interactOpenAngle = 90f;
    public float openDuration = 1f;

    private bool eventCompleted = false;
    private bool doorOpen = false;
    public bool keyPickedUp = false;
    public LivingRoomTrigger livingRoomTrigger;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    public AudioSource audioSource;
    public AudioClip doorSound;

    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;
    public Lockdown ld;
    private bool doorClosed = true;

    private float lockedMessageCooldown = 0f;
    private DialogueSystem dialogueSystem;
    private bool semiopen = false;
    void Start()
    {
        closedRotation = otherDoor.rotation;
        openRotation = Quaternion.Euler(otherDoor.eulerAngles + new Vector3(0, interactOpenAngle, 0));
        doorClosed = true;

        doorOpenClip = Resources.Load<AudioClip>("DrawersAndDoors/dooropen");
        doorCloseClip = Resources.Load<AudioClip>("DrawersAndDoors/doorclose");

        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
    }

    void Update()
    {
        if (lockedMessageCooldown > 0f)
            lockedMessageCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 4f))
            {
                if (hit.collider.transform == otherDoor)
                {
                    if (eventCompleted && keyPickedUp && livingRoomTrigger.eventcompleted && doorClosed || ld.lockdownActive)
                    {
                        ToggleDoor();
                    }
                    else if (lockedMessageCooldown <= 0f && !semiopen && !ld.lockdownActive)
                    {
                        if (dialogueSystem != null)
                        {
                            dialogueSystem.ShowPassiveMessage("It's locked...");
                            lockedMessageCooldown = 5f;
                        }
                    }
                }
            }
        }
    }

    public void TriggerEvent()
    {   
        semiopen = true;
        StartCoroutine(TriggerEventWithDelay());
    }

    private IEnumerator TriggerEventWithDelay()
    {
        yield return new WaitForSeconds(2f);
        audioSource.PlayOneShot(doorSound);
        StartCoroutine(OpenOtherDoorByEvent());
    }

    IEnumerator OpenOtherDoorByEvent()
    {
        Quaternion startRot = otherDoor.rotation;
        Quaternion endRot = Quaternion.Euler(otherDoor.eulerAngles + new Vector3(0, eventOpenAngle, 0));
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            otherDoor.rotation = Quaternion.Slerp(startRot, endRot, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        otherDoor.rotation = endRot;
        openRotation = Quaternion.Euler(closedRotation.eulerAngles + new Vector3(0, interactOpenAngle, 0));
        doorOpen = true;
        eventCompleted = true;
    }

    void ToggleDoor()
    {
        StopAllCoroutines();

        if (doorOpen)
        {
            StartCoroutine(PlayCloseSoundAfterDelay());
            StartCoroutine(RotateDoor(closedRotation));
        }
        else
        {
            if (audioSource != null && doorOpenClip != null)
            {
                audioSource.clip = doorOpenClip;
                audioSource.time = 0.2f;
                audioSource.Play();
            }

            StartCoroutine(RotateDoor(openRotation));
        }

        doorOpen = !doorOpen;
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        Quaternion startRot = otherDoor.rotation;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            otherDoor.rotation = Quaternion.Slerp(startRot, targetRotation, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        otherDoor.rotation = targetRotation;
    }

    public void OnKeyPickedUp()
    {
        keyPickedUp = true;
    }

    IEnumerator CloseOtherDoorAfterEvent()
    {
        doorClosed = false;
        Quaternion startRot = otherDoor.rotation;
        Quaternion endRot = closedRotation;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            otherDoor.rotation = Quaternion.Slerp(startRot, endRot, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        otherDoor.rotation = endRot;
        doorOpen = false;
        doorClosed = true;
    }

    public void OnPlayerEnterLivingRoom()
    {
        if (keyPickedUp)
        {
            StopAllCoroutines();
            if (doorOpen)
            {
                StartCoroutine(CloseOtherDoorAfterEvent());
            }
            else
            {
                doorClosed = true;
            }
            doorOpen = false;
        }
    }

    IEnumerator PlayCloseSoundAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (audioSource != null && doorCloseClip != null)
        {
            audioSource.clip = doorCloseClip;
            audioSource.Play();
        }
    }

    public void CloseSilentlyIfOpen()
    {
        if (doorOpen)
        {
            StartCoroutine(RotateDoor(closedRotation));
            doorOpen = false;
        }
    }
}
