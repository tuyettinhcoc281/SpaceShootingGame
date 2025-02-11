using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    public Button nextLevelButton; // Gán nút "Next Level" trong Inspector

    void Start()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("No more levels. Returning to main menu or quitting game.");
            // Bạn có thể thay thế bằng SceneManager.LoadScene("MainMenu"); nếu có màn chính
            Application.Quit();
        }
    }
}