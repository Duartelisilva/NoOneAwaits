using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingUI : MonoBehaviour
{
    public void LoadMainMenu()
    {
        // 1) Un‑pause if you ever set timeScale to 0
        Time.timeScale = 1f;

        // 2) Destroy the entire root GameObject that holds this button
        //    (so nothing from the ending screen persists)
        Destroy(transform.root.gameObject);

        // 3) Now load the main menu scene in Single mode
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
