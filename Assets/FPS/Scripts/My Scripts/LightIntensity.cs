using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightIntensityController : MonoBehaviour
{
    [Range(0f, 10f)]
    public float intensity = 10f;

    private float transitionTime = 3f;
    private float timer = 0f;
    private bool isTransitioning = false;

    private List<Light> targetLights = new List<Light>();
    private Dictionary<Light, Color> originalColors = new Dictionary<Light, Color>();
    private Color targetColor = Color.white;
    private Dictionary<Light, Color> startColors = new Dictionary<Light, Color>();

    void Start()
    {
        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        Color initialColor = new Color(0.85f, 0.78f, 0.55f); // Custom base color

        foreach (var light in allLights)
        {
            if (light.gameObject.tag == "Untagged")
            {
                light.color = initialColor;
                targetLights.Add(light);
                originalColors[light] = initialColor;
            }
        }

        targetColor = initialColor;
        SetAllLightsIntensity(intensity);
    }

    void Update()
    {
        SetAllLightsIntensity(intensity);

        if (isTransitioning)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / transitionTime);

            foreach (Light light in targetLights)
            {
                light.color = Color.Lerp(startColors[light], targetColor, t);
            }

            if (t >= 1f)
                isTransitioning = false;
        }
    }

    public void SetRedLevel(float redPercent)
    {
        timer = 0f;
        isTransitioning = true;

        startColors.Clear();
        foreach (var light in targetLights)
        {
            startColors[light] = light.color;
        }

        float redValue = Mathf.Clamp01(redPercent / 100f);
        targetColor = new Color(
            Mathf.Lerp(originalColors[targetLights[0]].r, 0.8745f, redValue),
            Mathf.Lerp(originalColors[targetLights[0]].g, 0.1569f, redValue),
            Mathf.Lerp(originalColors[targetLights[0]].b, 0.1569f, redValue)
        );
    }

    public void SetAllLightsIntensity(float intensity)
    {
        foreach (var light in targetLights)
        {
            if (light == null) continue;

            light.intensity = intensity;
            light.enabled = intensity > 0.5f;
        }
    }

    public void FadeIntensityToZero(float duration)
    {
        StartCoroutine(FadeToZeroCoroutine(duration));
    }

    private IEnumerator FadeToZeroCoroutine(float duration)
    {
        float start = intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            intensity = Mathf.Lerp(start, 0f, elapsed / duration);
            yield return null;
        }

        intensity = 0f;
    }
}
