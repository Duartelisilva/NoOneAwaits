using UnityEngine;
using Unity.FPS.Gameplay;

public class FlashlightRayPickup : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public Camera playerCamera;

    [Header("Flashlight Settings")]
    public PlayerFlashlightManager flashlightManager;
    //public LightIntensityController lightIntensityController;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public bool flashlightpicked;
    [Header("Target Object")]
    public GameObject objectToActivateAfterPickup; // Assign this in the inspector

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (flashlightManager != null && flashlightManager.FlashlightObject != null)
            flashlightManager.FlashlightObject.SetActive(false);

        if (objectToActivateAfterPickup != null)
            objectToActivateAfterPickup.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, interactDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {   
                    flashlightpicked = true;
                    var go = flashlightManager.FlashlightObject;
                    if (go != null)
                        go.SetActive(true);

                    if (flashlightManager.FlashlightLight != null)
                        flashlightManager.FlashlightLight.enabled = true;

                    typeof(PlayerFlashlightManager)
                        .GetField("m_IsFlashlightOn",
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance)
                        ?.SetValue(flashlightManager, true);

                    if (audioSource != null && pickupSound != null)
                        audioSource.PlayOneShot(pickupSound);

                    /*if (objectToActivateAfterPickup != null)
                        objectToActivateAfterPickup.SetActive(true);*/

                    float delay = (pickupSound != null) ? pickupSound.length : 0f;
                    Destroy(gameObject, delay);
                    //lightIntensityController.FadeIntensityToZero(3f);
                }
            }
        }
    }
}
