using System.Collections;
using UnityEngine;
using SojaExiles;

public class FinalDoor : MonoBehaviour
{
    public opencloseDoor door;                // Assign in Inspector
    public InteractableDestroy keyScript;    // Assign in Inspector
    public EndingScreen endingScreen;        // Assign in Inspector
    public DialogueSystem dialogueSystem;    // Assign in Inspector
    public GameObject objectToDisable;       // Assign in Inspector

    public float interactDistance = 3f;
    public float maxLookAngle = 90f;
    public Transform player;
    Camera playerCamera;
    public GameObject crosshair;
    public GameObject objectivesUI;
    public extrakey extrakey;
    public ExtraScreen extraScreen;
    public GameRestorer GameRestorer;
    private int hard = 0;

    void Start()
    {
        playerCamera = Camera.main;
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        if (keyScript == null)
            keyScript = FindFirstObjectByType<InteractableDestroy>();

        if (door != null && door.openandclose != null)
            door.openandclose.enabled = false;
        
        hard = PlayerPrefs.GetInt("HardModeUnlocked", 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 camPos = playerCamera.transform.position;
            Vector3 camForward = playerCamera.transform.forward;

            Ray ray = new Ray(camPos, camForward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == door.transform)
                {
                    Vector3 directionToDoor = (door.transform.position - player.position).normalized;
                    float angle = Vector3.Angle(player.forward, directionToDoor);
                    if (angle < maxLookAngle)
                    {
                        if (keyScript.keypickedup || extrakey.extrakeypicked)
                        {
                            if (objectToDisable != null)
                                objectToDisable.SetActive(false);
                            crosshair.SetActive(false);
                            objectivesUI.SetActive(false);

                            if(keyScript.keypickedup)
                            {
                                HardModeButtonController.UnlockHardMode();
                                endingScreen.ShowGoodEnding();
                            }
                                
                            else if(extrakey.extrakeypicked && !GameRestorer.cantbeextra)
                                extraScreen.ShowExtraEnding();

                            enabled = false;
                        }
                        else
                        {
                            if (dialogueSystem != null)
                                dialogueSystem.ShowPassiveMessage("It's locked");
                        }
                    }
                }
            }
        }
    }
}
