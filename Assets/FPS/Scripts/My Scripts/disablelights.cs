using UnityEngine;

public class EmissionDisabler : MonoBehaviour
{
    public Material[] materialsToDisableEmission;

    void Start()
    {
        foreach (var mat in materialsToDisableEmission)
        {
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white);
            }
        }
    }

    public void disablelights()
    {
        foreach (var mat in materialsToDisableEmission)
        {
            if (mat != null && mat.IsKeywordEnabled("_EMISSION"))
            {
                mat.DisableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
