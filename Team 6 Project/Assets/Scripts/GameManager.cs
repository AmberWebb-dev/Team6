// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton Instance

    [SerializeField] GameObject menuActive, menuPause, menuWin, menuLose;
    [SerializeField] TMP_Text enemyCountText;
    // Screen Effects
    [SerializeField] GameObject effectBlind;
    public GameObject player;
    public bool isPaused;
    float timeScaleOrig;

    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject crops;
    public CropDamage cropDamageScript;

    int enemyCount;

    void Awake()
    {
        Instance = this;
        timeScaleOrig = Time.timeScale;

        crops = GameObject.FindWithTag("Crop");
        cropDamageScript = crops.GetComponent<CropDamage>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void GameGoal(int amount)
    {
        enemyCount += amount;

        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount == 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    // Coroutine to apply blinding effect
    public IEnumerator ApplyBlindEffect()
    {
        if (effectBlind != null)
        {
            effectBlind.SetActive(true);
            yield return new WaitForSeconds(2f); // Blinding effect duration
            effectBlind.SetActive(false);
        }
    }
}