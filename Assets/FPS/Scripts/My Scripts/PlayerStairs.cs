using System.Collections;
using UnityEngine;

public class StairsTeleport : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Stairs Points")]
    public Transform stairsBottom;
    public Transform stairsTop;

    [Header("Post‑Climb Objects")]
    public GameObject object1;
    public GameObject object2;

    [Header("Climb Settings")]
    public float climbSpeed = 2f;

    [Header("Camera")]
    public Camera playerCamera;
    public MonoBehaviour cameraController;
    public LightIntensityController lightIntensityController;

    [Header("Dialogue")]
    public DialogueSystem dialogueSystem;
    public ObjectiveUI objectiveUI;

    bool _isClimbing;
    bool _canClimb = false;
    bool _dialogueTriggered = false;

    void Start()
    {
        if (object1 != null) object1.SetActive(false);
        if (object2 != null) object2.SetActive(false);
    }

    void Update()
    {
        if (_isClimbing) return;

        if (Input.GetKeyDown(interactKey))
        {
            var cam = playerCamera != null ? playerCamera : Camera.main;
            var ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out var hit, interactDistance) && hit.collider.CompareTag("stairs"))
            {
                if (_canClimb)
                {
                    _canClimb = false;
                    StartCoroutine(ClimbRoutine());
                }
                else if (!_dialogueTriggered && dialogueSystem != null)
                {
                    _dialogueTriggered = true;
                    dialogueSystem.OnDialogueComplete += EnableClimb;
                    dialogueSystem.StartDialogue(new string[] { "These stairs seem to lead to some attic..." });
                }
            }
        }
    }

    void EnableClimb()
    {
        dialogueSystem.OnDialogueComplete -= EnableClimb;
        _canClimb = true;
    }

    IEnumerator ClimbRoutine()
    {
        _isClimbing = true;

        var player = GameObject.FindGameObjectWithTag("Player");
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        var camTransform = (playerCamera != null ? playerCamera : Camera.main).transform;
        Quaternion frozenCamRot = camTransform.rotation;

        yield return MoveAndFreezeCamera(stairsBottom.position, camTransform, frozenCamRot);
        yield return MoveAndFreezeCamera(stairsTop.position, camTransform, frozenCamRot);

        if (cc != null) cc.enabled = true;
        if (cameraController != null) cameraController.enabled = true;

        if (object1 != null) object1.SetActive(true);
        if (object2 != null) object2.SetActive(true);
        if (lightIntensityController != null)
            lightIntensityController.FadeIntensityToZero(3f);

        _isClimbing = false;
        objectiveUI.SetObjective("explore the attic");
        StartCoroutine(ShowPassiveAfterDelay(1f));
    }

    IEnumerator ShowPassiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueSystem.ShowPassiveMessage("It's so dark in here...");
    }

    IEnumerator MoveAndFreezeCamera(Vector3 targetPos, Transform camTransform, Quaternion frozenRot)
    {
        var playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        while (Vector3.Distance(playerTransform.position, targetPos) > 0.01f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position,
                targetPos,
                climbSpeed * Time.deltaTime
            );
            camTransform.rotation = frozenRot;
            yield return null;
        }
    }
}
