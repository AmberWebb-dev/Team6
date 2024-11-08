// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //UI Stuff
    [SerializeField] GameObject menuActive, menuPause, menuWin, menuLose;
    [SerializeField] TMP_Text enemyCountText, waveCountText, cropCountText;
    [SerializeField] GameObject effectBlind;
    public bool isPaused;
    int enemyCount;
    int cropCount;
    int waveCount;
    //Player Stuff
    public PlayerController playerScript;
    public GameObject player;
    public GameObject playerDamageScreen;
    public Image playerHPBar;
    float timeScaleOrig;

    public GameObject[] cropsArray;

    // Wave settings
    [SerializeField] GameObject molePrefab; // Prefab for the enemy
    [SerializeField] List<Transform> spawnPoints; // List of spawn points
    [SerializeField] int enemiesPerWave = 5; // Number of enemies per wave
    [SerializeField] float timeBetweenSpawns = 1f, waveCooldown = 50F; // Time between enemy spawns

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        timeScaleOrig = Time.timeScale;

        // Initialize crops and crop count
        cropsArray = GameObject.FindGameObjectsWithTag("Crop");
        //cropCount = cropsArray.Length;
        cropCountText.text = cropCount.ToString("F0");

        // Initialize enemy and wave count
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        waveCount = 0; // Start at wave 0, will increment with first wave

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        StartNextWave();
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

    //Game States
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

        cropCountText.text = cropCount.ToString("F0");

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

    public void StartNextWave()
    {
        waveCount++;
        UpdateWaveCountUI();
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        // Wait for wave cooldown before next wave starts
        yield return new WaitForSeconds(waveCooldown);

        if (cropCount > 0) // Only start a new wave if crops are remaining
        {
            StartNextWave();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Count > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(molePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void UpdateEnemyCountUI()
    {
        enemyCountText.text = enemyCount.ToString("F0");
    }

    void UpdateWaveCountUI()
    {
        waveCountText.text = waveCount.ToString("F0"); 
    }
}
