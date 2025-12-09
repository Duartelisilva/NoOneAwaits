using UnityEngine;

public class Optimization : MonoBehaviour
{
    [SerializeField] private GameObject targetToActivate;

    // Call this method from another script to activate the object
    public void ActivateTarget()
    {
        if (targetToActivate != null)
            targetToActivate.SetActive(true);
        else
            Debug.LogWarning("[Optimization] No target assigned to activate.");
    }
}
