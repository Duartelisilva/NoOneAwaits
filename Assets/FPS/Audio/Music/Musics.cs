using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LoopingAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip startClip;
    public float fadeDuration = 2f;
    [Range(0f, 1f)] public float intensity = 1f;

    private float currentVolumeTarget = 1f;
    private Coroutine currentFade;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        if (startClip != null)
            PlayAudio(startClip);
    }

    void Update()
    {
        // Smoothly apply changes to intensity if it differs from current volume target
        if (audioSource.isPlaying && !Mathf.Approximately(currentVolumeTarget, intensity))
        {
            if (currentFade != null)
                StopCoroutine(currentFade);

            currentFade = StartCoroutine(FadeVolume(audioSource.volume, intensity, fadeDuration));
            currentVolumeTarget = intensity;
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if (audioSource.clip != clip)
            audioSource.clip = clip;

        if (currentFade != null)
            StopCoroutine(currentFade);

        audioSource.Play();
        currentFade = StartCoroutine(FadeVolume(0f, intensity, fadeDuration));
        currentVolumeTarget = intensity;
    }

    public void StopAudio()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeVolume(audioSource.volume, 0f, fadeDuration, () => audioSource.Stop()));
        currentVolumeTarget = 0f;
    }

    public void SetIntensity(float newIntensity)
    {
        intensity = Mathf.Clamp01(newIntensity);
    }

    private IEnumerator FadeVolume(float from, float to, float duration, System.Action onComplete = null)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        audioSource.volume = to;
        onComplete?.Invoke();
    }

    public IEnumerator FadeOutAndRestartClip(float fadeOutBeforeEndSeconds)
    {
        while (audioSource.isPlaying)
        {
            if (audioSource.time >= audioSource.clip.length - fadeOutBeforeEndSeconds)
            {
                yield return FadeVolume(audioSource.volume, 0f, fadeDuration);
                audioSource.time = 0f;
                yield return FadeVolume(0f, intensity, fadeDuration);
            }
            yield return null;
        }
    }
}
