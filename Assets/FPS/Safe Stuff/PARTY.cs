using UnityEngine;
using System.Collections.Generic;

public class PARTY : MonoBehaviour
{
    public float intensity = 20f;
    public float transitionTime = 1f;

    private List<Light> targetLights = new List<Light>();
    private List<Color> discoColors = new List<Color>
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.magenta,
        Color.cyan,
        Color.yellow,
        new Color(1f, 0.5f, 0f),    // Orange
        new Color(0.5f, 0f, 1f)     // Purple
    };

    private int currentColorIndex = 0;
    private int nextColorIndex = 1;
    private float timer = 0f;
    private bool isDiscoActive = false;

    private Color currentColor;  // store current fading-from color

    void Update()
    {
        if (!isDiscoActive) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / transitionTime);

        Color next = discoColors[nextColorIndex];

        foreach (Light light in targetLights)
        {
            if (light != null)
            {
                light.color = Color.Lerp(currentColor, next, t);
                light.intensity = intensity;
            }
        }

        if (t >= 1f)
        {
            timer = 0f;
            currentColor = next;
            currentColorIndex = nextColorIndex;
            nextColorIndex = (nextColorIndex + 1) % discoColors.Count;
        }
    }

    public void StartDiscoMode()
    {
        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        targetLights.Clear();

        foreach (var light in allLights)
        {
            if (light.gameObject.tag == "Untagged")
            {
                targetLights.Add(light);
                light.intensity = intensity;
            }
        }

        // Initialize currentColor from the first light's current color, or default to first disco color
        if (targetLights.Count > 0)
            currentColor = targetLights[0].color;
        else
            currentColor = discoColors[0];

        currentColorIndex = 0;
        nextColorIndex = 1;
        timer = 0f;
        isDiscoActive = true;
    }

    public void StopDiscoMode()
    {
        isDiscoActive = false;
    }
}
