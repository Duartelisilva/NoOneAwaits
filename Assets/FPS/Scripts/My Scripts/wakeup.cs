using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using System.Collections;

public class Wakeup : MonoBehaviour
{
    public PlayerCharacterController playerController;
    public CanvasGroup blackCanvas;

    PlayerInputHandler inputHandler;
    Transform camTransform;
    DialogueSystem dialogue;
    public ObjectiveUI objectiveUI;
    public GameObject fadecanvas;

    Vector3 wakeupPosition = new Vector3(0f, 0.2f, 0f);
    Vector3 wakeupRotation = new Vector3(-90f, 0f, 0f);
    Vector3 standingPosition;
    Vector3 standingRotation = Vector3.zero;
    float riseDuration = 2f;

    bool lockPosition = true;

    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerCharacterController not assigned.");
            return;
        }

        camTransform = playerController.PlayerCamera.transform;
        if (camTransform == null)
        {
            Debug.LogError("PlayerCamera not assigned in PlayerCharacterController.");
            return;
        }

        standingPosition = Vector3.up * playerController.CapsuleHeightStanding * playerController.CameraHeightRatio;

        camTransform.localPosition = wakeupPosition;
        camTransform.localEulerAngles = wakeupRotation;

        inputHandler = playerController.GetComponent<PlayerInputHandler>();
        if (inputHandler != null)
            inputHandler.enabled = false;

        dialogue = FindFirstObjectByType<DialogueSystem>();

        if (dialogue != null)
            dialogue.DisablePlayerMovement(true);

        StartCoroutine(InitialWakeupSequence());
    }

    void LateUpdate()
    {
        if (lockPosition && camTransform != null)
        {
            camTransform.localPosition = wakeupPosition;
            camTransform.localEulerAngles = wakeupRotation;
        }
    }

    IEnumerator InitialWakeupSequence()
    {
        yield return new WaitForSeconds(2f);

        if (dialogue != null)
        {
            dialogue.OnDialogueComplete += BeginFadeOut;
            dialogue.StartDialogue(new string[] { "(Press E to continue)" });
        }
    }

    void BeginFadeOut()
    {
        dialogue.OnDialogueComplete -= BeginFadeOut;
        StartCoroutine(FadeCanvasRoutine());
    }

    IEnumerator FadeCanvasRoutine()
    {
        float duration = 2f;
        float t = 0f;

        if (blackCanvas == null)
        {
            Debug.LogWarning("Black canvas not assigned.");
            yield break;
        }

        while (t < duration)
        {
            blackCanvas.alpha = Mathf.Lerp(1f, 0f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        blackCanvas.alpha = 0f;
        blackCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (dialogue != null)
        {
            dialogue.OnDialogueComplete += StartGetUp;
            dialogue.StartDialogue(new string[] { "Wh-what happened... Where am I?" });
        }
    }

    void StartGetUp()
    {
        dialogue.OnDialogueComplete -= StartGetUp;
        StartCoroutine(RaiseCamera());
    }

    IEnumerator RaiseCamera()
    {
        lockPosition = false;

        Vector3 startPos = camTransform.localPosition;
        Vector3 startRot = camTransform.localEulerAngles;
        Vector3 targetRot = new Vector3(0f, startRot.y, startRot.z);
        float t = 0f;

        while (t < riseDuration)
        {
            camTransform.localPosition = Vector3.Lerp(startPos, standingPosition, t / riseDuration);

            float newX = Mathf.LerpAngle(startRot.x, targetRot.x, t / riseDuration);
            camTransform.localEulerAngles = new Vector3(newX, startRot.y, startRot.z);

            t += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = standingPosition;
        camTransform.localEulerAngles = standingRotation;

        if (dialogue != null)
        {
            dialogue.OnDialogueComplete += FinalMessageComplete;
            dialogue.StartDialogue(new string[] { "I feel a weird presence... I need to get out of here." });
        }
    }

    void FinalMessageComplete()
    {   fadecanvas.SetActive(false);
        objectiveUI?.SetObjective("leave the house");
        dialogue.OnDialogueComplete -= FinalMessageComplete;
        dialogue.gamebegin = false;
        dialogue.DisablePlayerMovement(false);

        if (inputHandler != null)
            inputHandler.enabled = true;

        Destroy(this);
    }
}
