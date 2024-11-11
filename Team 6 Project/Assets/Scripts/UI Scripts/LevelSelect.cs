using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;      // The button for the level
        public string sceneName;   // The scene to load for this level
    }

    public List<LevelButton> levelButtons; // List of buttons with corresponding scenes

    void Start()
    {
        // Assign each button a click listener to load its associated scene
        foreach (var levelButton in levelButtons)
        {
            string sceneToLoad = levelButton.sceneName;  // Capture the scene name to avoid closure issues
            levelButton.button.onClick.AddListener(() => LoadScene(sceneToLoad));
        }
    }

    // Method to load a scene by name
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
