using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextGradient : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TextMeshProUGUI[] textObjects;

    public RectTransform topRef;
    public RectTransform midRef;
    public RectTransform bottomRef;

    public Color topColor = new Color32(0x91, 0x00, 0x7A, 255);     // #91007A (Purple-ish)
    public Color midColor = new Color32(0x0B, 0x73, 0x00, 255);     // #0B7300 (Dark Green)
    public Color bottomColor = new Color32(0x95, 0x00, 0x00, 255);  // #950000 (Dark Red)

    void Update()
    {
        foreach (var tmp in textObjects)
        {
            ApplyGradient(tmp);
        }
    }

    void ApplyGradient(TextMeshProUGUI tmp)
    {
        tmp.ForceMeshUpdate();

        if (tmp.textInfo.characterCount == 0)
            return;

        var textInfo = tmp.textInfo;
        RectTransform viewport = scrollRect.viewport;

        float topY = viewport.InverseTransformPoint(topRef.position).y;
        float midY = viewport.InverseTransformPoint(midRef.position).y;
        float bottomY = viewport.InverseTransformPoint(bottomRef.position).y;

        Vector3 textViewportPos = viewport.InverseTransformPoint(tmp.rectTransform.position);

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            var vertices = meshInfo.vertices;
            var colors = meshInfo.colors32;

            if (colors.Length != vertices.Length)
                colors = new Color32[vertices.Length];

            for (int j = 0; j < vertices.Length; j += 4)
            {
                float avgY = (vertices[j].y + vertices[j + 1].y + vertices[j + 2].y + vertices[j + 3].y) / 4f;
                float vertexViewportY = avgY + textViewportPos.y;

                Color32 color;

                if (vertexViewportY >= midY)
                {
                    // Between top and mid
                    float t = Mathf.InverseLerp(midY, topY, vertexViewportY);
                    color = Color32.Lerp(midColor, topColor, t);
                }
                else
                {
                    // Between bottom and mid
                    float t = Mathf.InverseLerp(bottomY, midY, vertexViewportY);
                    color = Color32.Lerp(bottomColor, midColor, t);
                }

                colors[j] = color;
                colors[j + 1] = color;
                colors[j + 2] = color;
                colors[j + 3] = color;
            }

            meshInfo.colors32 = colors;
            textInfo.meshInfo[i] = meshInfo;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
