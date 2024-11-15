using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelect : MonoBehaviour
{
    public List<GameObject> pages; // UI pages: 0 - Main Menu, 1 - Level Select
    public Button nextButton;
    public Button previousButton;
    [SerializeField] private TMPro.TMP_Text levelOneHighscoreText, levelTwoHighscoreText, levelThreeHighscoreText;
    private int currentPageIndex = 0; // Current page index

    void Start()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
        nextButton.onClick.AddListener(SwitchToNextPage);
        previousButton.onClick.AddListener(SwitchToPreviousPage);
        UpdateButtonVisibility();
    }

    public void SwitchToNextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex++;
            pages[currentPageIndex].SetActive(true);
            UpdateButtonVisibility();
            if (currentPageIndex == 1)
            {
                UpdateHighscoreText();
                LevelSelect levelSelect = pages[currentPageIndex].GetComponent<LevelSelect>();
                if (levelSelect != null)
                {
                    levelSelect.UpdateLevelStars();
                }
            }
        }
    }

    public void SwitchToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex--;
            pages[currentPageIndex].SetActive(true);
            UpdateButtonVisibility();
            if (currentPageIndex == 0)
            {
                UpdateHighscoreText();
                LevelSelect levelSelect = pages[currentPageIndex].GetComponent<LevelSelect>();
                if (levelSelect != null)
                {
                    levelSelect.UpdateLevelStars();
                }
            }
        }
    }

    private void UpdateButtonVisibility()
    {
        nextButton.gameObject.SetActive(currentPageIndex < pages.Count - 1);
        previousButton.gameObject.SetActive(currentPageIndex > 0);
    }

    private void UpdateHighscoreText()
    {
        int highscore;
        highscore = PlayerPrefs.GetInt("Level 1 Highscore", 0);
        levelOneHighscoreText.text = $"Highscore: {(highscore == 0 ? "--" : highscore)}";
        highscore = PlayerPrefs.GetInt("Level 2 Highscore", 0);
        levelTwoHighscoreText.text = $"Highscore: {(highscore == 0 ? "--" : highscore)}";
        highscore = PlayerPrefs.GetInt("Level 3 Highscore", 0);
        levelThreeHighscoreText.text = $"Highscore: {(highscore == 0 ? "--" : highscore)}";
    }
}