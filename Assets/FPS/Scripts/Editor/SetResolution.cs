using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CanvasResolutionSetter : MonoBehaviour
{
    [MenuItem("Tools/Set All Canvases to 1920x1080")]
    static void SetCanvasResolutions()
    {
        foreach (CanvasScaler scaler in FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None))
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            EditorUtility.SetDirty(scaler);
        }
    }
}
