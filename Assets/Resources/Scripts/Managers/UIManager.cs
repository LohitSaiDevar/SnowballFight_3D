using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;
    public TMP_Text playerWinText;
    bool isGamePaused;

    public static UIManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = !isGamePaused;
        }

        if (isGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    public void DisplayGameOverMenuUI()
    {
        gameOverMenu.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
