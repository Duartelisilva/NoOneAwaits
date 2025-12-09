using UnityEngine;
using System.Collections;
using Unity.FPS.Gameplay;

public class DollReveal : MonoBehaviour
{
    public GameObject targetObject;
    public float disableFlashlightDuration = 5f;
    public AudioSource dollAudioSource;
    public AudioClip dollSound;

    PlayerFlashlightManager flashlightManager;
    PlayerInputHandler playerInputHandler;
    PlayerCharacterController characterController;
    public LoopingAudioController loopingAudioController; // Assign in Inspector
    public RevealOnFlashlight2 disablearrows;

    bool flashlightWasOn;
    bool triggered = false;

    void Start()
    {   
        StartCoroutine(PreloadAndDisableDoll());
        targetObject.SetActive(false);

        flashlightManager   = Object.FindFirstObjectByType<PlayerFlashlightManager>();
        playerInputHandler  = Object.FindFirstObjectByType<PlayerInputHandler>();
        characterController = Object.FindFirstObjectByType<PlayerCharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        triggered = true;

        if (loopingAudioController != null)
            StartCoroutine(FadeOutMusic(2));

        // Disable flashlight
        if (flashlightManager != null)
        {
            flashlightWasOn = flashlightManager.FlashlightLight.enabled;
            flashlightManager.FlashlightLight.enabled = false;
            flashlightManager.enabled = false;
        }

        // Show doll and play laugh
        if (dollAudioSource != null && dollSound != null)

        StartCoroutine(DisableMovementAfterDelay(0.5f));
        StartCoroutine(ReenableAfterDelay());
    }



    IEnumerator DisableMovementAfterDelay(float delay)
    {
        disablearrows.ForceHideArrows();
        yield return new WaitForSeconds(delay);

        targetObject.SetActive(true);
        dollAudioSource.PlayOneShot(dollSound);
        if (playerInputHandler    != null) playerInputHandler.enabled    = false;
        if (characterController   != null) characterController.enabled   = false;
    }

    IEnumerator ReenableAfterDelay()
    {
        yield return new WaitForSeconds(disableFlashlightDuration);

        // Restore flashlight
        if (flashlightManager != null)
        {
            flashlightManager.enabled                       = true;
            flashlightManager.FlashlightLight.enabled       = flashlightWasOn;
        }

        // Restore player control & hide doll
        if (playerInputHandler    != null) playerInputHandler.enabled    = true;
        if (characterController   != null) characterController.enabled   = true;

        //targetObject.SetActive(false);
    }


    IEnumerator FadeOutMusic(float duration)
    {
        AudioSource source = loopingAudioController.audioSource;
        float startVolume = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    IEnumerator PreloadAndDisableDoll()
    {
        targetObject.SetActive(true);

        // Wait one frame so materials/shaders initialize
        yield return null;

        // Optional: wait another frame if needed
        // yield return null;

        targetObject.SetActive(false);
    }
}
