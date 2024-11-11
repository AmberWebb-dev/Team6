using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSelect : MonoBehaviour
{
    public List<GameObject> pages; // UI pages: 0 - Main Menu, 1 - Level Select
    public Button nextButton;
    public Button previousButton;
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
        }
    }

    private void UpdateButtonVisibility()
    {
        nextButton.gameObject.SetActive(currentPageIndex < pages.Count - 1);
        previousButton.gameObject.SetActive(currentPageIndex > 0);
    }
}