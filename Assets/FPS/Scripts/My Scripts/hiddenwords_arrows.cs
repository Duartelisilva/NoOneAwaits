using UnityEngine;
using System.Collections;

public class RevealOnFlashlight2 : MonoBehaviour
{
    public Light flashlight;
    public string hiddenTag = "hiddenwords_arrows";
    public int maxObjects = 5;
    public float revealDistance = 5f;
    public float revealSpeed = 3f;

    private Renderer[] hiddenObjects;
    private float[] currentReveal;
    private bool[] revealedOnce;
    private AudioSource[] audioSources;
    private AudioClip revealClip;

    public ObjectVisibilityManager visibilityManager; // assign in inspector

    void Start()
    {
        GameObject[] hiddenGOs = GameObject.FindGameObjectsWithTag(hiddenTag);
        int count = Mathf.Min(hiddenGOs.Length, maxObjects);

        hiddenObjects = new Renderer[count];
        currentReveal = new float[count];
        revealedOnce = new bool[count];
        audioSources = new AudioSource[count];

        revealClip = Resources.Load<AudioClip>("SpecialEffects/arrowsound");

        for (int i = 0; i < count; i++)
        {
            hiddenObjects[i] = hiddenGOs[i].GetComponent<Renderer>();
            if (hiddenObjects[i] != null)
            {
                hiddenObjects[i].material.SetFloat("_RevealStrength", 0f);
                audioSources[i] = hiddenGOs[i].AddComponent<AudioSource>();
                audioSources[i].playOnAwake = false;
                audioSources[i].spatialBlend = 1f;
                audioSources[i].clip = revealClip;
            }
            revealedOnce[i] = false;
        }
    }

    void Update()
    {
        if (visibilityManager != null && visibilityManager.hasInteracted)
        {
            // Fade out all and block further revealing
            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                if (hiddenObjects[i] == null) continue;

                currentReveal[i] = Mathf.MoveTowards(currentReveal[i], 0f, revealSpeed * Time.deltaTime);
                hiddenObjects[i].material.SetFloat("_RevealStrength", currentReveal[i]);
                revealedOnce[i] = false;
            }
            return;
        }

        int hitIndex = -1;

        if (flashlight != null &&
            flashlight.enabled &&
            flashlight.gameObject.activeInHierarchy &&
            Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out RaycastHit hit, revealDistance))
        {
            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                if (hiddenObjects[i] != null && hit.collider.gameObject == hiddenObjects[i].gameObject)
                {
                    hitIndex = i;
                    break;
                }
            }
        }

        for (int i = 0; i < hiddenObjects.Length; i++)
        {
            if (hiddenObjects[i] == null) continue;

            if (i == hitIndex)
            {
                float prevValue = currentReveal[i];
                currentReveal[i] = Mathf.MoveTowards(currentReveal[i], 1f, revealSpeed * Time.deltaTime);

                if (!audioSources[i].isPlaying && prevValue < 0.01f && currentReveal[i] > 0.01f)
                {
                    audioSources[i].PlayOneShot(revealClip, 2f);
                }

                revealedOnce[i] = true;
            }
            else
            {
                if (hitIndex != -1 && revealedOnce[i])
                {
                    currentReveal[i] = Mathf.MoveTowards(currentReveal[i], 0f, revealSpeed * Time.deltaTime);
                    if (currentReveal[i] <= 0f)
                        revealedOnce[i] = false;
                }
            }

            hiddenObjects[i].material.SetFloat("_RevealStrength", currentReveal[i]);
        }
    }

    public void ForceHideArrows()
    {
        StartCoroutine(HideArrowsAfterDelay(0.5f));
    }

    private IEnumerator HideArrowsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < hiddenObjects.Length; i++)
        {
            if (hiddenObjects[i] != null)
            {
                currentReveal[i] = 0f;
                hiddenObjects[i].material.SetFloat("_RevealStrength", 0f);
                revealedOnce[i] = false;
            }
        }
    }
}
