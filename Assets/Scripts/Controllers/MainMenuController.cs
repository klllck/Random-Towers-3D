using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        //For Build
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
