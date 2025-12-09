using UnityEngine;

public class SkyboxTintController : MonoBehaviour
{
    private Material skyboxMaterial;

    void Start()
    {
        skyboxMaterial = RenderSettings.skybox;
        SetTintColor(new Color32(0x80, 0x80, 0x80, 0xFF));  // #808080
    }

    public void SetTintColorRed()
    {
        SetTintColor(new Color32(0xE6, 0x10, 0x10, 0xFF));  // #E61010
    }

    private void SetTintColor(Color color)
    {
        if (skyboxMaterial.HasProperty("_Tint"))
        {
            skyboxMaterial.SetColor("_Tint", color);
        }
    }
}
