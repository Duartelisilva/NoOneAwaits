using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;


    void Start()
    {
        SetObjective("???");
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
            objectiveText.text = "Objective: " + text;
    }

    public void ClearObjective()
    {
        if (objectiveText != null)
            objectiveText.text = "";
    }
}
