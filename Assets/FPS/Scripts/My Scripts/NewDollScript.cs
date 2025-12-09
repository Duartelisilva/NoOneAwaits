using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class DollAuxiliary : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlightLight;

    [Header("End Sequence Audio")]
    public AudioSource endAudioSource;
    public AudioClip endClip;
    public string mixerGroupName = "DollLaugh";

    [Header("Ambience Audio (new)")]
    public AudioSource ambienceAudioSource;
    public AudioClip ambienceClip;

    [Header("Doll Settings")]
    public GameObject dollObject;

    [Header("Additional Object")]
    public GameObject additionalObjectToToggle; // Assign in Inspector

    public AudioMixer audioMixer;

    private Color originalColor;
    private float originalIntensity;
    public float pitchaux = 1f;

    void Start()
    {
        if (flashlightLight != null)
        {
            originalColor = flashlightLight.color;
            originalIntensity = flashlightLight.intensity;
        }
        else
        {
            Debug.LogWarning("Flashlight Light not assigned on DollAuxiliary.");
        }

        if (audioMixer != null && endAudioSource != null)
        {
            var groups = audioMixer.FindMatchingGroups(mixerGroupName);
            if (groups.Length > 0)
                endAudioSource.outputAudioMixerGroup = groups[0];
            else
                Debug.LogWarning($"Mixer group '{mixerGroupName}' not found.");
        }

        // Disable additional object at start
        if (additionalObjectToToggle != null)
            additionalObjectToToggle.SetActive(false);
    }

    public void FadeFlashlightSequenceAndEnd()
    {
        if (flashlightLight != null)
            StartCoroutine(FullSequenceCoroutine());
    }

    private IEnumerator FullSequenceCoroutine()
    {
        float colorDuration = 2f;
        Color startColor = flashlightLight.color;
        float elapsed = 0f;
        while (elapsed < colorDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / colorDuration;
            flashlightLight.color = Color.Lerp(startColor, Color.red, t);
            yield return null;
        }
        flashlightLight.color = Color.red;

        float intensityDuration = 2f;
        float startIntensity = flashlightLight.intensity;
        elapsed = 0f;
        while (elapsed < intensityDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / intensityDuration;
            flashlightLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }
        flashlightLight.intensity = 0f;

        // Re-enable additional object now that darkness is reached
        if (additionalObjectToToggle != null)
            additionalObjectToToggle.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (endAudioSource != null && endClip != null)
        {
            endAudioSource.pitch = 1.45f;
            endAudioSource.PlayOneShot(endClip);
        }

        if (ambienceAudioSource != null && ambienceClip != null)
        {
            ambienceAudioSource.clip = ambienceClip;
            ambienceAudioSource.loop = true;
            ambienceAudioSource.Play();
        }

        if (endClip != null)
            yield return new WaitForSeconds(endClip.length);

        if (dollObject != null)
            dollObject.SetActive(false);

        flashlightLight.color = originalColor;
        flashlightLight.intensity = originalIntensity;
    }
}
