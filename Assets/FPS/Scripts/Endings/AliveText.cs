using TMPro;
using UnityEngine;

public class TitleUnderlayAnimator : MonoBehaviour
{
    public TMP_Text titleText;
    public float speed = 2f;
    public float dilateMin = 0.05f;
    public float dilateMax = 0.15f;

    void Update()
    {
        if (titleText == null) return;

        Material mat = titleText.fontMaterial;

        float offsetX = Mathf.Sin(Time.time * speed) * 0.5f;
        float offsetY = Mathf.Cos(Time.time * speed) * 0.5f;
        mat.SetFloat("_UnderlayOffsetX", offsetX);
        mat.SetFloat("_UnderlayOffsetY", offsetY);

        float dilate = Mathf.Lerp(dilateMin, dilateMax, (Mathf.Sin(Time.time * speed) + 1) / 2);
        mat.SetFloat("_FaceDilate", dilate);
    }
}
