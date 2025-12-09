using UnityEngine;
using System.Collections;

public class KidsBedroomEntryMusic : MonoBehaviour
{
    public PlayerLocationTracker locationTracker;
    public LoopingAudioController audioController;

    public AudioClip kidsBedroomSong;        // original background song
    public AudioClip dollInteractionSong;    // new song for doll interaction

    public float delayBeforeNewSong = 2f;

    private bool hasEnteredKidsBedroom = false;
    private bool transitionStarted = false;
    public Lockdown ld;
    void Start()
    {
        if (locationTracker == null)
            locationTracker = FindFirstObjectByType<PlayerLocationTracker>();

        if (audioController == null)
            audioController = FindFirstObjectByType<LoopingAudioController>();
    }

    void Update()
    {   
        if (hasEnteredKidsBedroom || ld.lockdownActive)
            return;

        if (locationTracker != null && locationTracker.playerIsIn == PlayerLocationTracker.PlayerLocationState.KidsBedroom)
        {
            hasEnteredKidsBedroom = true;
            StartCoroutine(HandleMusicTransition());
        }
    }

    private IEnumerator HandleMusicTransition()
    {
        // Fade out current music
        audioController.StopAudio();

        // Wait for fade out + specified delay
        yield return new WaitForSeconds(audioController.fadeDuration + delayBeforeNewSong);

        // Play original kids bedroom song with fade-in
        audioController.SetIntensity(0.1f);
        audioController.PlayAudio(kidsBedroomSong);

        float fadeInDuration = 5f;
        float t = 0f;

        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            audioController.SetIntensity(Mathf.Lerp(0.1f, 1f, t / fadeInDuration));
            yield return null;
        }

        audioController.SetIntensity(1f);
    }

    public void StartMusicTransition()
    {
        if (!transitionStarted)
        {
            transitionStarted = true;
            StartCoroutine(HandleMusicTransition());
        }
    }

    // Call this to immediately play the doll interaction song with fade-in over 2 seconds
    public void PlayDollInteractionSong()
    {
        StopAllCoroutines();
        StartCoroutine(PlayNewSongWithFadeIn(dollInteractionSong, 2f));
    }

    private IEnumerator PlayNewSongWithFadeIn(AudioClip clip, float fadeInDuration)
    {
        audioController.StopAudio();

        // Wait for fade out to complete
        yield return new WaitForSeconds(audioController.fadeDuration);

        audioController.SetIntensity(0f);
        audioController.PlayAudio(clip);

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            audioController.SetIntensity(Mathf.Lerp(0f, 1f, t / fadeInDuration));
            yield return null;
        }
        audioController.SetIntensity(1f);
    }
}
