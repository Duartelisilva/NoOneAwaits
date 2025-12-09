using UnityEngine;
using SojaExiles;

public class KeyPickup : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera playerCamera;
    public FirstEventTrigger firstEventTrigger;  // Reference to FirstEventTrigger
    public LightIntensityController lightController;

    public AudioSource audioSource;      // Assign in inspector or get in Start()
    public AudioClip pickupSound;        // Assign the key pickup sound here
    public Lockdown ld;
    public LoopingAudioController audioController;
    public GameObject objectToDestroy;
    private bool eventhappened = false;
    public Optimization optimization;
    void Start()
    {   
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioController == null)
            audioController = FindFirstObjectByType<LoopingAudioController>();
    }

    void Update()
    {
        if(ld.lockdownActive) return;
             
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (firstEventTrigger != null)
                    {
                        firstEventTrigger.OnKeyPickedUp();
                    }
                    lightController.SetRedLevel(75f);

                    // Fade intensity to 50% on key pickup:
                    if (audioController != null)
                    {
                        audioController.SetIntensity(0.5f);
                    }
                    optimization.ActivateTarget();
                    Destroy(objectToDestroy);
                    if (audioSource != null && pickupSound != null && !eventhappened)
                    {   
                        eventhappened = true;
                        audioSource.PlayOneShot(pickupSound);
                    }
                }
            }
        }
    }
}
