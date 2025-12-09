using UnityEngine;
using UnityEngine.InputSystem;
public class DoorInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                DoorOpen door = hit.collider.GetComponent<DoorOpen>();
                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }
}
