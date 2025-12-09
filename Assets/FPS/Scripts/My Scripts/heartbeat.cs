using UnityEngine;
using System.Collections;

public class heartbeat : MonoBehaviour
{
    public Transform doll;
    public float nearDistance = 8f;
    public float reallyCloseDistance = 4f;

    public AudioSource audio1;  // heartbeat for near
    public AudioSource audio2;  // heartbeat for really close

    public Material redLinesMaterial;  // assign the material with the second shader here

    private Coroutine fadeCoroutine;
    private enum ProximityState { Far, Near, ReallyClose }
    private ProximityState currentState = ProximityState.Far;

    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject canvas3;

    public WanderAndChase dollchase;

    void Start()
    {
        SetShaderVisibility(0f);
    }

    void Update()
    {
        if(canvas1.activeSelf || canvas2.activeSelf || canvas3.activeSelf)
        {
            StartCoroutine(FadeOutBoth());
            SetShaderVisibility(0f);
            return;
        }

        if (doll == null || !doll.gameObject.activeInHierarchy || audio1 == null || audio2 == null || redLinesMaterial == null)
            return;

        float distance = Vector3.Distance(transform.position, doll.position);
        ProximityState newState;

        if (distance <= reallyCloseDistance)
            newState = ProximityState.ReallyClose;
        else if (distance <= nearDistance)
            newState = ProximityState.Near;
        else
            newState = ProximityState.Far;

        if (newState != currentState)
        {
            Debug.Log($"Proximity changed: {currentState} → {newState}");
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            switch (newState)
            {
                case ProximityState.ReallyClose:
                    fadeCoroutine = StartCoroutine(SwitchAudio(audio1, audio2));
                    break;
                case ProximityState.Near:
                    fadeCoroutine = StartCoroutine(SwitchAudio(audio2, audio1));
                    break;
                case ProximityState.Far:
                    fadeCoroutine = StartCoroutine(FadeOutBoth());
                    break;
            }

            currentState = newState;
        }

        // Calculate visibility alpha based on distance: 0 at far, 1 at really close
        float visibility = 0f;
        if (distance <= reallyCloseDistance)
            visibility = 1f;
        else if (distance <= nearDistance)
            visibility = Mathf.InverseLerp(nearDistance, reallyCloseDistance, distance) / 2;
        
        if(dollchase.chasing)
        SetShaderVisibility(visibility);
        else SetShaderVisibility(0);
    }

    Color baseColor = new Color(1f, 0.2f, 0.2f, 0.7f); // original line color
    private float currentAlpha = 0f;
    private float alphaChangeSpeed = 1f / 3f; // 3 seconds to fully change

    void SetShaderVisibility(float targetAlpha)
    {
        if (redLinesMaterial != null)
        {
            // Smoothly move currentAlpha toward targetAlpha
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaChangeSpeed * Time.deltaTime);

            Color col = baseColor;
            col.a *= currentAlpha;
            redLinesMaterial.SetColor("_LineColor", col);
        }
    }


    IEnumerator SwitchAudio(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        Debug.Log($"Switching: {fadeOutSource.name} → {fadeInSource.name}");
        fadeOutSource.volume = 1f;
        yield return StartCoroutine(FadeOut(fadeOutSource, 0.5f));
        fadeOutSource.Stop();

        fadeInSource.volume = 0f;
        fadeInSource.Play();
        yield return StartCoroutine(FadeIn(fadeInSource, 0.5f));
        fadeInSource.volume = 1f;
    }

    IEnumerator FadeOutBoth()
    {
        Debug.Log("Fading out both audios");
        if (audio1.isPlaying)
        {
            yield return StartCoroutine(FadeOut(audio1, 0.5f));
            audio1.Stop();
        }

        if (audio2.isPlaying)
        {
            yield return StartCoroutine(FadeOut(audio2, 0.5f));
            audio2.Stop();
        }
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        Debug.Log($"Fading out {source.name}");
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        Debug.Log($"Fading in {source.name}");
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        source.volume = 1f;
    }
}
