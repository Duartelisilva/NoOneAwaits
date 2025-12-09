// Attach this script to each PlayerZone object
using UnityEngine;

public class PlayerZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the GameManager and call UpdatePlayerLocation with this zone
            PlayerLocationTracker manager = FindFirstObjectByType<PlayerLocationTracker>();
            if (manager != null)
            {
                manager.UpdatePlayerLocation(gameObject);
            }
        }
    }
}
