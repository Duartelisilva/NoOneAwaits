using UnityEngine;

public class SigilFadeInteraction : MonoBehaviour
{
    public GameObject keyObject;          // Assign your key GameObject in the Inspector
    public float fadeDuration = 2f;       // Time it takes to fade out
    public Renderer sigilRenderer;

    private Material material;
    private Color originalColor;
    private bool isFading = false;

    void Start()
    {
        // Clone the material to avoid changing the original asset
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        originalColor = material.color;

        if (keyObject != null)
            keyObject.SetActive(false); // Hide the key initially
    }

    void OnMouseDown()
    {
        if (!isFading)
            StartCoroutine(FadeAndActivate());
    }

    System.Collections.IEnumerator FadeAndActivate()
    {
        isFading = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            Color newColor = originalColor;
            newColor.a = alpha;
            material.color = newColor;
            timer += Time.deltaTime;
            yield return null;
        }

        if (keyObject != null)
            keyObject.SetActive(true); // Show the key

        Destroy(gameObject); // Destroy the sigil
    }
}
