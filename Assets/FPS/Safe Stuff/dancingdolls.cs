using UnityEngine;

public class dancingdolls : MonoBehaviour
{
    public GameObject[] dollsToActivate;

    public void EnableAllDancingDolls()
    {
        foreach (var doll in dollsToActivate)
        {
            if (doll != null)
                doll.SetActive(true);
        }
    }
}
