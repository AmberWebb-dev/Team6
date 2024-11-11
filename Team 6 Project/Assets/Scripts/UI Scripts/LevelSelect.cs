using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;      
        public string sceneName;   
    }

    public List<LevelButton> levelButtons;

    void Start()
    {
        foreach (var levelButton in levelButtons)
        {
            string sceneToLoad = levelButton.sceneName;
            levelButton.button.onClick.AddListener(() => LoadScene(sceneToLoad));
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
