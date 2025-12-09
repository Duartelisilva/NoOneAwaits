using UnityEngine;

public class FlashlightSettings : MonoBehaviour
{
    public Light flashlightLight; // Assign the child Light in Inspector

    void Awake()
    {
        if (flashlightLight == null)
            flashlightLight = GetComponentInChildren<Light>(true); // true includes inactive

        if (GameSettings.isHardMode)
        {
            flashlightLight.innerSpotAngle = 0f;
            flashlightLight.spotAngle = 72f;
            flashlightLight.intensity = 2f;
            flashlightLight.range = 5f;
        }
        else
        {
            flashlightLight.innerSpotAngle = 30f;
            flashlightLight.spotAngle = 72f;
            flashlightLight.intensity = 4f;
            flashlightLight.range = 20f;
        }
    }
}
