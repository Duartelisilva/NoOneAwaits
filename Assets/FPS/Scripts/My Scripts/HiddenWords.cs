using UnityEngine;

public class RevealOnFlashlight : MonoBehaviour
{
    public Light flashlight;                 // Assign your flashlight here
    public string hiddenTag = "hiddenwords"; // Tag for objects with hidden text
    public float revealDistance = 5f;        // Max distance to reveal
    public float revealSpeed = 3f;           // How fast text appears/disappears

    private Renderer[] hiddenObjects;
    private float[] currentReveal;

    void Start()
    {
        // Find all objects with the tag "hiddenwords"
        GameObject[] hiddenGOs = GameObject.FindGameObjectsWithTag(hiddenTag);
        hiddenObjects = new Renderer[hiddenGOs.Length];
        for (int i = 0; i < hiddenGOs.Length; i++)
        {
            hiddenObjects[i] = hiddenGOs[i].GetComponent<Renderer>();
        }

        currentReveal = new float[hiddenObjects.Length];
    }

    void Update()
    {
        int hitIndex = -1;

        // Only proceed if flashlight is enabled and active
        if (flashlight != null &&
            flashlight.enabled &&
            flashlight.gameObject.activeInHierarchy &&
            Physics.Raycast(flashlight.transform.position, flashlight.transform.forward, out RaycastHit hit, revealDistance))
        {
            for (int i = 0; i < hiddenObjects.Length; i++)
            {
                if (hit.collider.gameObject == hiddenObjects[i].gameObject)
                {
                    hitIndex = i;
                    break;
                }
            }
        }

        for (int i = 0; i < hiddenObjects.Length; i++)
        {
            float target = (i == hitIndex) ? 1f : 0f;
            currentReveal[i] = Mathf.MoveTowards(currentReveal[i], target, revealSpeed * Time.deltaTime);

            Material mat = hiddenObjects[i].material;
            mat.SetFloat("_RevealStrength", currentReveal[i]);
        }
    }
}
