using UnityEngine;
using System.Collections;

public class thankyou : MonoBehaviour
{
    public GameObject interactableObject;
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    public Camera playerCamera;
    public DialogueSystem dialogueSystem;
    public PARTY discoLightController;
    public dancingdolls dd;
    public GameObject daughterpicture;

    [Header("Music Settings")]
    public LoopingAudioController loopingAudioController;
    public AudioClip newSong;

    private bool isMonitoring = false;
    public float fadeOutBeforeEndSeconds = 12f;
    private bool isDialogueRunning = false;
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {   if(isDialogueRunning)
        return;

        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {   
                if (hit.collider.gameObject == interactableObject)
                {   

                    if (daughterpicture.activeInHierarchy) 
                    {   
                        isDialogueRunning = true;
                        dialogueSystem.OnDialogueComplete += () => isDialogueRunning = false;
                        dialogueSystem.StartDialogue(new string[] { "That photo of a girl...", "I should check it first." });
                        return;
                    }


                    else if (dialogueSystem != null)
                        dialogueSystem.StartDialogue(new string[] { "Developer notes:", "This game was developed by me alone,",
                        "this is the first game I have ever made,", "it was done as a side project just for fun.",
                        "Hopefully you had as much fun playing as I had making it :)", "Thank you for playing!" });

                    if (loopingAudioController != null && newSong != null)
                    {
                        StartCoroutine(FadeOutOldAndPlayNew());

                        if (!isMonitoring)
                        {
                            isMonitoring = true;
                            StartCoroutine(loopingAudioController.FadeOutAndRestartClip(fadeOutBeforeEndSeconds));
                        }
                    }

                    if (discoLightController != null)
                        discoLightController.StartDiscoMode();

                    if (dd != null)
                        dd.EnableAllDancingDolls();

                    Destroy(interactableObject);
                }
            }
        }
    }

    private IEnumerator FadeOutOldAndPlayNew()
    {
        loopingAudioController.StopAudio();
        yield return new WaitForSeconds(loopingAudioController.fadeDuration);
        loopingAudioController.PlayAudio(newSong);
    }
}
