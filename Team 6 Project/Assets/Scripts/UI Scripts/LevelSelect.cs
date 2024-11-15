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
    [SerializeField] private List<GameObject> levelStars;

    void Start()
    {
        UpdateLevelStars();
        foreach (var levelButton in levelButtons)
        {
            string sceneToLoad = levelButton.sceneName;
            levelButton.button.onClick.AddListener(() => LoadScene(sceneToLoad));
        }
    }
    public void UpdateLevelStars()
    {
        for (int i = 0; i < levelStars.Count; i++)
        {
            int levelIndex = i + 1;
            string starKey = $"Level_{levelIndex}_Star";
            bool hasStar = PlayerPrefs.GetInt(starKey, 0) == 1;
            levelStars[i].SetActive(hasStar);
        }
    }
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
