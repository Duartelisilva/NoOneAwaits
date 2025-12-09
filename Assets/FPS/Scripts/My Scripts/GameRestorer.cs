using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;
using SojaExiles;

public class GameRestorer : MonoBehaviour
{   
    [Header("Optional UI")]
    public CanvasGroup badEndingGroup; // Assign the CanvasGroup from the Bad Ending UI


    [Header("References")]
    public Transform restorePoint;               // Assign in Inspector (target position)
    public Image fadeImage;                      // UI Image for fade (black image full screen)
    public float fadeDuration = 3f;
    public BadEndingScreen bes;

    private PlayerCharacterController playerController;
    private GameObject playerCamera;
    private PlayerInputHandler inputHandler;
    public GameObject imagecanvas;
    private bool restarting=false;
    public BadEndingScreen badEndingScreen;
    public bool cantbeextra=false;
    public bool reenabledoll = false;
    public PlayerLocationTracker locationTracker;
    public GameObject dollstartposition;

    [Header("Restore Variables")]
    public WanderAndChase dollwalk;
    public GameObject doll;
    public NotesVisibilityManager nvm;
    public InteractableDestroy finalkey;
    public SigilSelfInteract sigil;
    public extrakey extrakey;
    public ObjectVisibilityManager livingroomcollider;
    public GameObject badendingtext;

    public GameObject objectiveUI;
    public GameObject pauseUI;
    public GameObject crosshair;
    public GameObject finalkeyobject;
    public FirstEventTrigger fet;
    public DBLockedDoor dbdoor;
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerCharacterController>();
        playerCamera = Camera.main.gameObject;
        inputHandler = FindFirstObjectByType<PlayerInputHandler>();

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // transparent at start
    }

    public void RestorePlayer()
    {
        StartCoroutine(RestoreRoutine());
    }

    IEnumerator RestoreRoutine()
    {   
        if(restarting)
        yield break;

        restarting = true;
        imagecanvas.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));

        if (badEndingGroup != null)
        {
            badEndingGroup.alpha = 0f;
            badEndingGroup.interactable = false;
            badEndingGroup.blocksRaycasts = false;
        }

        if (playerController != null && restorePoint != null)
        {
            CharacterController cc = playerController.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            playerController.transform.position = restorePoint.position;
            playerController.transform.rotation = restorePoint.rotation;

            if (Camera.main != null)
                Camera.main.transform.localRotation = Quaternion.identity;

            if (cc) cc.enabled = true;

            // ✅ Re-enable movement and input
            playerController.enabled = true;
            if (inputHandler != null)
                inputHandler.enabled = true;
        }
        

        closethedoors();

        restorevariables();
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(Fade(1f, 0f));
        imagecanvas.SetActive(false);
        badEndingScreen.HideBadEnding();


        cantbeextra = true;

        locationTracker.playerIsIn = PlayerLocationTracker.PlayerLocationState.DBBedroom;

        reenabledoll = true;
        restarting = false;
    }

    IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            float alpha = Mathf.Lerp(from, to, t);
            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }
    public void restorevariables()
    {
        dollwalk.contactTriggered = false;
        finalkey.keypickedup = false;
        sigil.interactionTriggered = false;
        finalkeyobject.SetActive(false);
        badendingtext.SetActive(false);
        bes.mainMenuButton.gameObject.SetActive(false);
        bes.mainMenuButton.interactable = false;

        doll.transform.position = dollstartposition.transform.position;
        doll.transform.rotation = dollstartposition.transform.rotation;
        doll.SetActive(false);
        nvm.ResetAllNotes();
        objectiveUI.SetActive(true);
        pauseUI.SetActive(true);
        crosshair.SetActive(true);
    }

    public void closethedoors()
    {

        StartCoroutine(dbdoor.silentclosing());
        
        fet.CloseSilentlyIfOpen();

        ClosetopencloseDoor[] closets = Object.FindObjectsByType<ClosetopencloseDoor>(FindObjectsSortMode.None);
        foreach (var closet in closets)
        {
            closet.CloseSilentlyIfOpen();
        }

        Drawer_Pull_X[] drawers = Object.FindObjectsByType<Drawer_Pull_X>(FindObjectsSortMode.None);
        foreach (var drawer in drawers)
        {
            drawer.CloseSilentlyIfOpen();
        }

        opencloseDoor[] doors2 = Object.FindObjectsByType<opencloseDoor>(FindObjectsSortMode.None);
        foreach (var door in doors2)
        {
            door.CloseSilentlyIfOpen();
        }

        opencloseDoor1[] doors = Object.FindObjectsByType<opencloseDoor1>(FindObjectsSortMode.None);
        foreach (var door in doors)
        {
            door.CloseSilentlyIfOpen();
        }
    }



}