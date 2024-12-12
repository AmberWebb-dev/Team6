using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class toSettings : MonoBehaviour
{
    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("Settings");

    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
