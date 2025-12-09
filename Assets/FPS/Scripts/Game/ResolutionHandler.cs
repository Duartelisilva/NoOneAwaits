using UnityEngine;

public class ResolutionSwitcher : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // 1920x1080 (16:9)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (Input.GetKeyDown(KeyCode.O)) // 1280x720 (16:9)
        {
            Screen.SetResolution(1280, 720, false);
        }
        else if (Input.GetKeyDown(KeyCode.I)) // 2560x1440 (16:9)
        {
            Screen.SetResolution(2560, 1440, false);
        }
        else if (Input.GetKeyDown(KeyCode.K)) // 1680x1050 (16:10)
        {
            Screen.SetResolution(1680, 1050, false);
        }
        else if (Input.GetKeyDown(KeyCode.L)) // 1920x1200 (16:10)
        {
            Screen.SetResolution(1920, 1200, false);
        }
        else if (Input.GetKeyDown(KeyCode.F)) // Fullscreen native resolution
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
    }
}
