using UnityEngine;
using System.Collections;

public class MainMenuTheme : MonoBehaviour
{
    public AudioSource themeSource;
    public float fadeDuration = 3f;

    void Start()
    {
        if (themeSource != null && themeSource.clip != null)
            StartCoroutine(PlayWithFadeLoop());
    }

    IEnumerator PlayWithFadeLoop()
    {
        while (true)
        {
            themeSource.volume = 0f;
            themeSource.Play();

            // Fade in
            yield return StartCoroutine(FadeAudio(themeSource, 0f, 1f, fadeDuration));

            // Wait until 5s before the clip ends
            float waitTime = themeSource.clip.length - 5f;
            yield return new WaitForSeconds(waitTime);

            // Fade out
            yield return StartCoroutine(FadeAudio(themeSource, 1f, 0f, 5f));

            themeSource.Stop();
        }
    }

    IEnumerator FadeAudio(AudioSource source, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        source.volume = to;
    }
}
