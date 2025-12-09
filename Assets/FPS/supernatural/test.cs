using UnityEngine;
using UnityEngine.UI;

public class UndergroundShaderController : MonoBehaviour
{
    public Transform player;
    public Graphic targetGraphic; // Image, RawImage, or Text component
    public float fadeDistance = 1f;

    int playerYID;

    void Start()
    {
        playerYID = Shader.PropertyToID("_PlayerY");
    }

    void Update()
    {
        float y = player.position.y;
        if (targetGraphic != null && targetGraphic.material != null)
        {
            targetGraphic.material.SetFloat(playerYID, y);
            targetGraphic.material.SetFloat("_FadeDistance", fadeDistance);
        }
    }
}
