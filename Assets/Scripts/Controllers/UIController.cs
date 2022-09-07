using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class UIController : Singleton<UIController>
{
    public bool gameIsPaused;
    public bool gameIsOver;
    public Slider slider;
    public TextMeshProUGUI loadingText;
    public GameObject loadingPannel;
    public GameObject pauseMenu;
    public GameObject pauseUI;
    public GameObject gameOverUI;

    private void Update()
    {
        if (gameIsOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        FindAllEnemiesOnBoard();

        if (PlayerManager.Instance.Lives <= 0)
        {
            GameOver();
        }
    }

    private void FindAllEnemiesOnBoard()
    {
        PlayerManager.Instance.EnemiesAlive = GameObject.FindGameObjectsWithTag(TagManager.enemy).Length;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        gameOverUI.SetActive(false);
        gameIsPaused = true;
    }
    private void GameOver()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        pauseUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetScreen()
    {
        loadingPannel.SetActive(true);
        loadingText.text = "0%";
        slider.value = 0;
    }

    public void SetLoadingValue(float value)
    {
        loadingText.text = (int)(value * 100) + "%";
        slider.value = value;
    }

    public void HideLoadingScreen()
    {
        loadingPannel.SetActive(false);
    }
}
