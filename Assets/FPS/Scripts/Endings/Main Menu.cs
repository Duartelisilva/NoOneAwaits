using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject left;       // assign first canvas
    public GameObject right;      // assign second canvas
    public GameObject title;      // assign title object

    public CanvasGroup fadeCanvasGroup;   // full screen black panel with CanvasGroup (alpha 0 to 1)
    public TextMeshProUGUI loadingText;   // "Loading" text, initially invisible (alpha 0)

    public AudioClip playAudioClip;        // assign play sound
    public AudioClip quitAudioClip;        // assign quit sound
    public AudioMixerGroup audioMixerGroup; // assign AudioMixerGroup
    private bool buttonnomore=false;
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    public HardModeButtonController hbc;
    public Button myButton;
    public GameObject Aboutbutton;
    void Start()
    {
        loadingText.alpha = 0;
        fadeCanvasGroup.alpha = 0;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        left.SetActive(true);
        right.SetActive(true);
        title.SetActive(true);
    }

    public void StartGame()
    {   if(buttonnomore)
        return;

        buttonnomore=true;
        myButton.interactable = false;
        hbc.StartGame();
        StartCoroutine(PlaySoundThenStart(playAudioClip));
    }

    public void QuitGame()
    {   
        myButton.interactable = false;
        StartCoroutine(PlaySoundThenQuit(quitAudioClip));
    }

    private IEnumerator PlaySoundThenStart(AudioClip clip)
    {
        // 1) Fade out UI first
        yield return StartCoroutine(FadeOutUI());
        // 2) Show loading text (fade in)
        yield return StartCoroutine(FadeText(loadingText, 0f, 1f, fadeDuration));
        // 3) Play your “play” sound
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
        // 4) (NEW) Warm up all shaders so next scene never shows fallback
        Shader.WarmupAllShaders();
        yield return null; // let one frame pass so UI can update
        // 5) Begin async loading
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;
        // 6) Wait until scene is ready (~90%)
        while (operation.progress < 0.9f)
            yield return null;
        // 7) Fade out loading text
        yield return StartCoroutine(FadeText(loadingText, 1f, 0f, fadeDuration));
        // 8) Fade screen to black
        yield return StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 0f, 1f, fadeDuration));
        // 9) Finally activate the scene
        operation.allowSceneActivation = true;
    }

    private IEnumerator PlaySoundThenQuit(AudioClip clip)
    {
        if (buttonnomore)
            yield break;

        buttonnomore = true;

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            // Wait until audio finishes playing
            while (audioSource.isPlaying)
                yield return null;
        }

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }


    private IEnumerator StartGameRoutine()
    {
        yield return StartCoroutine(FadeText(loadingText, 0f, 1f, fadeDuration));

        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeText(loadingText, 1f, 0f, fadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 0f, 1f, fadeDuration));

        operation.allowSceneActivation = true;
    }

    private IEnumerator FadeOutUI()
    {
        left.SetActive(false);
        right.SetActive(false);
        title.SetActive(false);
        Aboutbutton.SetActive(false);
        yield break;
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        text.alpha = endAlpha;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }
}
