// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject menuActive, menuPause, menuWin, menuLose;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] GameObject effectBlind;
    public GameObject player;
    public PlayerController playerScript;
    public bool isPaused;
    float timeScaleOrig;

    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject[] cropsArray;
    int enemyCount;
    int cropCount;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        timeScaleOrig = Time.timeScale;

        cropsArray = GameObject.FindGameObjectsWithTag("Crop");
        cropCount = cropsArray.Length;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    public void UnregisterCrop(GameObject crop)
    {
        for (int i = 0; i < cropsArray.Length; i++)
        {
            if (cropsArray[i] == crop)
            {
                cropsArray[i] = null;
                break;
            }
        }
        UpdateCrop(-1);
    }

    public GameObject GetNearestCrop(Vector3 position)
    {
        GameObject nearestCrop = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject crop in cropsArray)
        {
            if (crop != null)
            {
                float distance = Vector3.Distance(position, crop.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCrop = crop;
                }
            }
        }

        return nearestCrop;
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
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
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

    public void UpdateCrop(int amount)
    {
        cropCount += amount;

        if (cropCount <= 0)
        {
            YouLose();
        }
    }

    public IEnumerator ApplyBlindEffect()
    {
        if (effectBlind != null)
        {
            effectBlind.SetActive(true);
            yield return new WaitForSeconds(2f);
            effectBlind.SetActive(false);
        }
    }
}
