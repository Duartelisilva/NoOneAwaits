using UnityEngine;
using UnityEngine.UI;

public static class GameSettings
{
    public static bool isHardMode = false;
    public static bool hardModeUnlocked = false;
}

public class HardModeButtonController : MonoBehaviour
{
    public GameObject hardModeButtonParent;  // Assign the parent GameObject of the hard mode toggle/button
    public Toggle hardModeToggle;             // Assign the Toggle component

    void Start()
    {
        // disable hard mode
        //PlayerPrefs.DeleteKey("HardModeUnlocked");
        //PlayerPrefs.Save();

        // Check if hard mode was unlocked in a previous session
        bool unlockedBefore = PlayerPrefs.GetInt("HardModeUnlocked", 0) == 1;
        GameSettings.hardModeUnlocked = unlockedBefore;

        // Show/hide the parent containing the hard mode button
        hardModeButtonParent.SetActive(GameSettings.hardModeUnlocked);

        // If unlocked, set the toggle state accordingly (default off)
        if (GameSettings.hardModeUnlocked)
            hardModeToggle.isOn = false;



    }

    // Call this when player finishes normal mode
    public static void UnlockHardMode()
    {
        GameSettings.hardModeUnlocked = true;
        PlayerPrefs.SetInt("HardModeUnlocked", 1);
        PlayerPrefs.Save();
    }

    // Call this when starting the game
    public void StartGame()
    {
        GameSettings.isHardMode = hardModeToggle.isOn;
        // Load the game scene after this...
        Debug.Log("Hard Mode is " + (GameSettings.isHardMode ? "ON" : "OFF"));

    }
}
