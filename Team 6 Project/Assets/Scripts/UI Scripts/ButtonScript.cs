using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.StateUnpause();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.StateUnpause();
    }
    public void MainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void NextLevel()
    {
        int nextSceneIndex = GameManager.Instance.GetCurrentLevelIndex() + 1;
        if (nextSceneIndex > GameManager.Instance.totalLevelCount)
        {
            return;
        }

        SceneManager.LoadScene(nextSceneIndex);
        GameManager.Instance.StateUnpause();
    }

    public void Credits()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateUnpause();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Credits");
    }

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
    }
}
