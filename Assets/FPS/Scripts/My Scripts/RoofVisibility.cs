using UnityEngine;

public class RoofToggle : MonoBehaviour
{
    private GameObject[] roofs;

    void Start()
    {
        roofs = GameObject.FindGameObjectsWithTag("roof1");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var roof in roofs)
            {
                roof.SetActive(!roof.activeSelf);
            }
        }
    }
}
