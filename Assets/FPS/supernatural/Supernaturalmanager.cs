using UnityEngine;
using System.Collections;

public class SupernaturalManager : MonoBehaviour
{
    public GameObject glitchOverlay; // RawImage GameObject with glitch shader
    public Transform player; // assign your player here
    public float glitchDuration = 0.25f; // How long the glitch shows
    public float fadeDistance = 1f;

    int playerYID;

    void Start()
    {
        glitchOverlay.SetActive(false);
        playerYID = Shader.PropertyToID("_PlayerY");
    }

    void Update()
    {
        if (glitchOverlay.activeSelf)
        {
            // Update shader with player Y position and fade distance
            var graphic = glitchOverlay.GetComponent<UnityEngine.UI.Graphic>();
            if (graphic != null && graphic.material != null)
            {
                graphic.material.SetFloat(playerYID, player.position.y);
                graphic.material.SetFloat("_FadeDistance", fadeDistance);
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(TriggerGlitch());
        }
    }

    IEnumerator TriggerGlitch()
    {
        if (glitchOverlay != null)
        {
            glitchOverlay.SetActive(true);
            yield return new WaitForSeconds(glitchDuration);
            glitchOverlay.SetActive(false);
        }
    }
}
