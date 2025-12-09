using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BadEndingScreen : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI endingText;
    public float fadeDuration = 2f;
    public Button mainMenuButton;
    public CanvasGroup badEndingCanvasGroup;

    [Header("Player References")]
    public string playerTag = "Player";
    public string playerMovementScriptName = "PlayerCharacterController";
    public string playerLookScriptName = "PlayerInputHandler";
    public Transform playerCamera;
    public AudioSource playerAudioSource;
    public AudioClip playerScreamClip;
    public GameObject options;
    [Header("Audio")]
    public AudioClip hitFloorClip;
    private Coroutine keepCursorCoroutine;

    void Start()
    {
        if (badEndingCanvasGroup != null)
        {
            badEndingCanvasGroup.alpha = 0f;
            badEndingCanvasGroup.interactable = false;
            badEndingCanvasGroup.blocksRaycasts = false;
            badEndingCanvasGroup.gameObject.SetActive(true);
        }

        if (endingText != null)
            endingText.gameObject.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(false);
    }

    public void ShowBadEnding()
    {
        options.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisablePlayerMovement();

        StartCoroutine(TriggerBadEndingSequence());
        keepCursorCoroutine = StartCoroutine(KeepCursorVisible());
    }

    private IEnumerator KeepCursorVisible()
    {
        while (true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            yield return null;
        }
    }

    void DisablePlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            var movement = player.GetComponent(playerMovementScriptName);
            if (movement != null)
                (movement as MonoBehaviour).enabled = false;

            var look = player.GetComponent(playerLookScriptName);
            if (look != null)
                (look as MonoBehaviour).enabled = false;
        }
    }

    private IEnumerator TriggerBadEndingSequence()
    {
        Coroutine fallRoutine = StartCoroutine(FallCamera());

        if (playerAudioSource != null && playerScreamClip != null)
        {
            playerAudioSource.clip = playerScreamClip;
            playerAudioSource.volume = 1f;
            playerAudioSource.Play();
            StartCoroutine(FadeOutScream(playerAudioSource, 1f, 1f));
        }

        yield return fallRoutine;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeToBlackThenShowText());
    }

    private IEnumerator FallCamera()
    {
        if (playerCamera == null)
            yield break;

        Quaternion startRot = playerCamera.rotation;

        Vector3 leftDirection = -playerCamera.right;
        float rayDistance = 1f;
        bool hitWallOnLeft = Physics.Raycast(playerCamera.position, leftDirection, rayDistance);

        float fallAngle = hitWallOnLeft ? -80f : 80f;
        Quaternion fallRot = startRot * Quaternion.Euler(0f, 0f, fallAngle);

        float duration = 0.6f;
        float t = 0f;
        bool playedSound = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            playerCamera.rotation = Quaternion.Slerp(startRot, fallRot, t / duration);

            if (!playedSound && t / duration >= 0.6f)
            {
                if (playerAudioSource != null && hitFloorClip != null)
                    playerAudioSource.PlayOneShot(hitFloorClip);
                playedSound = true;
            }

            yield return null;
        }
    }

    private IEnumerator FadeToBlackThenShowText()
    {
        float t = 0f;
        Color panelColor = fadePanel.color;
        panelColor.a = 0;
        fadePanel.color = panelColor;

        endingText.gameObject.SetActive(false);

        // Fade in black panel
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelColor.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadePanel.color = panelColor;
            yield return null;
        }

        // Fade in canvas
        if (badEndingCanvasGroup != null)
        {
            badEndingCanvasGroup.alpha = 0f;
            badEndingCanvasGroup.gameObject.SetActive(true);

            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                badEndingCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }

            badEndingCanvasGroup.interactable = true;
            badEndingCanvasGroup.blocksRaycasts = true;
        }

        // Fade in text
        endingText.gameObject.SetActive(true);
        endingText.alpha = 0;
        t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            endingText.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            mainMenuButton.interactable = true;
        }
    }

    private IEnumerator FadeOutScream(AudioSource source, float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = source.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    public void HideBadEnding()
    {
        if (keepCursorCoroutine != null)
            StopCoroutine(keepCursorCoroutine);

        Cursor.lockState = CursorLockMode.Locked;  // or your game's default lock state
        Cursor.visible = false;

        // Re-enable player movement/input here if needed
    }
}
