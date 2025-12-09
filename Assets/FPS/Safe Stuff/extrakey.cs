using UnityEngine;

public class extrakey : MonoBehaviour
{
    public GameObject objectX;           // Object to interact with
    public GameObject objectY;           // Object to destroy on interaction
    public AudioClip interactionSound;  // Sound to play on interaction
    public ObjectiveUI objectiveUI;      // Reference to your ObjectiveUI script

    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;
    public Camera playerCamera;
    public bool extrakeypicked = false;
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.gameObject == objectX)
                {
                    if (objectY != null)
                        Destroy(objectY);

                    if (interactionSound != null)
                        AudioSource.PlayClipAtPoint(interactionSound, objectX.transform.position);

                    if (objectiveUI != null)
                        objectiveUI.SetObjective("leave the house");

                    extrakeypicked=true;
                }
            }
        }
    }
}
