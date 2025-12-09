using UnityEngine;

public class HardModeVariables : MonoBehaviour
{

    [Header ("Classes")]
    public NotesVisibilityManager nvm;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nvm.notesLimit = GameSettings.isHardMode ? 24 : 12;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
