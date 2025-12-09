using UnityEngine;

public class Lockdown : MonoBehaviour
{
    public MonoBehaviour[] scriptsToDisable;  // Assign scripts here in Inspector
    public GameObject[] objectsToDisable;     // Assign objects here to disable
    public GameObject[] objectsToEnable;      // Assign objects here to enable
    public bool lockdownActive = false;
    public void TriggerLockdown()
    {   
        lockdownActive = true;
        Debug.Log("Lockdown triggered!");

        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"Disabled GameObject: {obj.name}");
            }
        }

        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled GameObject: {obj.name}");
            }
        }
    }
}
