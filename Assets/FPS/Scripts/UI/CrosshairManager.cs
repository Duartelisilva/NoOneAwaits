using UnityEngine;
using UnityEngine.UI;

public class AlwaysVisibleCrosshair : MonoBehaviour
{
    public Image CrosshairImage;

    void Start()
    {
        if (CrosshairImage != null)
        {
            CrosshairImage.enabled = true;
        }
    }

    void Update()
    {
        if (CrosshairImage != null && !CrosshairImage.enabled)
        {
            CrosshairImage.enabled = true;
        }
    }
}
