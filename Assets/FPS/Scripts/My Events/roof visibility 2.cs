using UnityEngine;
using System.Collections;
using SojaExiles;
public class ObjectVisibilityManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject object1;
    public GameObject object2;
    public GameObject stairs;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    public PlayerLocationTracker locationTracker;

    [Header("Dialogue & Climb")]
    public DialogueSystem dialogueSystem;
    public GameObject player;
    public MonoBehaviour cameraController;
    public Transform stairsBottom;
    public Transform stairsTop;
    public float climbSpeed = 2f;
    public LightIntensityController lightIntensityController;
    public GameObject safe;
    public ObjectiveUI objectiveUI;

    [Header("Notes & Sigil")]
    public GameObject sigilObject;
    public Renderer sigilRenderer;
    private Color sigilBaseEmission;
    public NotesVisibilityManager NotesVisibilityManager;

    public GameObject objectToDisableAtStart;
    public GameObject objectToDisableAtStart2;
    public GameObject spellobject;

    public bool hasInteracted = false;
    public bool hasInteracted2 = false;
    private bool happenedaux = false;
    private bool _isClimbing = false;
    private bool _dialogueTriggered = false;
    private bool _canClimb = false;
    GameObject interactedDoll = null;
    public GameRestorer reenabledoll;
    public FrameInteraction FrameInteraction;
    public EmissionDisabler EmissionDisabler;
    public SkyboxTintController SkyboxTintController;
    void Start()
    {
        if (sigilObject != null)
            sigilObject.SetActive(false);

        if (sigilRenderer != null)
        {
            sigilBaseEmission = sigilRenderer.material.GetColor("_EmissionColor");
            sigilRenderer.material.EnableKeyword("_EMISSION");
            SetSigilEmission(0f);
        }

        if (objectToDisableAtStart != null)
            objectToDisableAtStart.SetActive(false);

        if (objectToDisableAtStart2 != null)
            objectToDisableAtStart2.SetActive(false);

        object1.SetActive(false);
        object2.SetActive(false);
    }

    void Update()
    {   
        
        if (_isClimbing) return;

        if (Input.GetKeyDown(interactKey))
        {
            var cam = playerCamera != null ? playerCamera : Camera.main;
            var ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out var hit, interactDistance))
            {
                if (hit.collider.CompareTag("stairs"))
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

        CheckDollInteraction();
        CheckSpellInteraction();
        CheckLivingRoomReturn();
    }

    void EnableClimb()
    {
        _canClimb = true;
        dialogueSystem.OnDialogueComplete -= EnableClimb;
    }

    IEnumerator ClimbRoutine()
    {   
        _isClimbing = true;
        safe?.SetActive(false);

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        var camTransform = (playerCamera != null ? playerCamera : Camera.main).transform;
        Quaternion frozenCamRot = camTransform.rotation;
        dialogueSystem.DisablePlayerMovement(true);
        yield return MovePlayer(stairsBottom.position, camTransform, frozenCamRot);
        yield return MovePlayer(stairsTop.position, camTransform, frozenCamRot);
        dialogueSystem.DisablePlayerMovement(false);
        if (cc != null) cc.enabled = true;
        if (cameraController != null) cameraController.enabled = true;

        object1?.SetActive(true);
        object2?.SetActive(true);

        if (lightIntensityController != null)
            lightIntensityController.FadeIntensityToZero(3f);

        objectiveUI?.SetObjective("explore the attic");
        FrameInteraction.wooshitsgone=true;
        SkyboxTintController.SetTintColorRed();
        _isClimbing = false;
    }

    IEnumerator MovePlayer(Vector3 targetPos, Transform camTransform, Quaternion frozenRot)
    {   
        var playerTransform = player.transform;
        while (Vector3.Distance(playerTransform.position, targetPos) > 0.01f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position,
                targetPos,
                climbSpeed * Time.deltaTime
            );
            camTransform.rotation = frozenRot;
            reenabledoll.closethedoors();
            yield return null;
        }
    }

    void CheckDollInteraction()
    {
        if (hasInteracted || !Input.GetKeyDown(interactKey)) return;

        var cam = playerCamera != null ? playerCamera : Camera.main;
        var ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.collider.CompareTag("doll"))
        {
            interactedDoll = hit.collider.gameObject;
            hasInteracted = true;

            if (sigilObject != null && sigilRenderer != null)
            {
                NotesVisibilityManager.SpawnInitialNotes();
                sigilObject.SetActive(true);
                SetSigilEmission(0f);
            }

            interactedDoll?.GetComponent<DollController>()?.TriggerInteraction();
        }
    }

    void CheckSpellInteraction()
    {
        if (hasInteracted2 || !Input.GetKeyDown(interactKey)) return;

        var cam = playerCamera != null ? playerCamera : Camera.main;
        var ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.collider.CompareTag("spell"))
        {
            object1?.SetActive(false);
            object2?.SetActive(false);
            stairs?.SetActive(false);

            Destroy(spellobject);
            hasInteracted2 = true;

            var barrier = GameObject.FindWithTag("barrier_attic");
            if (barrier != null)
                Destroy(barrier);

            if (dialogueSystem != null)
            {
                string[] lines = new string[]
                {
                    "She left a note...",
                    "\"Let's play a game!",
                    $"Find all {NotesVisibilityManager.notesLimit} notes scattered on the floor below.",
                    "Together, they form the spell to summon your way out of here.",
                    "The more you collect, the more will appear!!",
                    "Good luck.\""
                };

                dialogueSystem.OnDialogueComplete += () =>
                {
                    if (!happenedaux)
                    {
                        objectiveUI?.SetObjective($"collect all the notes (0/{NotesVisibilityManager.notesLimit})");
                        happenedaux = true;
                    }
                };
                EmissionDisabler.disablelights();
                dialogueSystem.StartDialogue(lines);
            }
        }
    }

    void CheckLivingRoomReturn()
    {   
        if(reenabledoll.reenabledoll && !objectToDisableAtStart.activeSelf && locationTracker.playerIsIn == PlayerLocationTracker.PlayerLocationState.LivingRoom)
        {
            objectToDisableAtStart.SetActive(true);
            reenabledoll.reenabledoll = false;
        }
        else if (!hasInteracted)
            return;

        if (locationTracker.playerIsIn == PlayerLocationTracker.PlayerLocationState.LivingRoom)
        {
            object1.SetActive(true);
            object2.SetActive(true);
            hasInteracted = false;
            objectToDisableAtStart?.SetActive(true);
        }
    }

    void SetSigilEmission(float intensity)
    {
        if (sigilRenderer != null)
            sigilRenderer.material.SetColor("_EmissionColor", sigilBaseEmission * intensity);
    }
}
