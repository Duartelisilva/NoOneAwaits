using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingScreen : MonoBehaviour
{
    public Image fadePanel;
    public TextMeshProUGUI endingText;
    public float fadeDuration = 2f;

    public string playerLookScriptName = "YourLookScript";
    public string playerTag = "Player";
    public string playerMovementScriptName = "YourMovementScript";

    public Button mainMenuButton; // Assign in the Inspector
    public CanvasGroup mainCanvasGroup; // Parent of all UI elements
    public GameObject options;
    void Start()
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.alpha = 0f;
            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;
        }

        if (endingText != null)
            endingText.gameObject.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.gameObject.SetActive(false);
    }

    public void ShowGoodEnding()
    {   
        options.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisablePlayerMovement();

        StartCoroutine(FadeInCanvasThenBlack());
        StartCoroutine(KeepCursorVisible());
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

    private IEnumerator FadeInCanvasThenBlack()
    {
        float t = 0f;

        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.alpha = 0f;
            mainCanvasGroup.gameObject.SetActive(true);

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                mainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }

            mainCanvasGroup.interactable = true;
            mainCanvasGroup.blocksRaycasts = true;
        }

        yield return StartCoroutine(FadeToBlackThenShowText());
    }

    private IEnumerator FadeToBlackThenShowText()
    {
        float t = 0f;
        Color panelColor = fadePanel.color;
        panelColor.a = 0;
        fadePanel.color = panelColor;

        if (endingText != null)
            endingText.gameObject.SetActive(false);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelColor.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = panelColor;
            yield return null;
        }

        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);
            endingText.alpha = 0f;
        }

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (endingText != null)
                endingText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(true);
            mainMenuButton.interactable = true;
        }
    }
}
