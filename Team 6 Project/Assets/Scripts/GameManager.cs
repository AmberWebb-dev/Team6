// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // Level settings
    // Used for next level button. Takes the index of the current scene and compares it
    // to the total amount of levels we have. Level one has to be index one, level two
    // index two, etc. Update this value whenever a new level is added.
    public int totalLevelCount;
    private int currentLevel;
    [Header("----- Menus -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] Button nextLevelButton;
    [Header("----- Text -----")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text cropCountText;
    [SerializeField] TMP_Text waveTimerText;
    [Header("----- Screen Effects -----")]
    public GameObject playerDamageScreen;
    [SerializeField] GameObject effectBlind;
    [SerializeField] GameObject effectShield;
    public bool isPaused;
    float timeScaleOrig;
    [Header("----- Player Stuff -----")]
    public PlayerController playerScript;
    public GameObject player;
    public Image playerHPBar;
    [Header("----- Crop Info -----")]
    public GameObject[] cropsArray;
    int cropCount;
    int cropCountOriginal;
    [Header("----- Wave Settings -----")]
    [SerializeField] List<GameObject> molePrefabs;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] int numOfWaves = 1;
    int waveCount;
    [SerializeField] int enemiesPerWave = 5;
    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] float waveCooldown = 50F;
    //Misc
    public int enemyScoreTotal;
    int enemyCount;
    Coroutine waveTimer;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        timeScaleOrig = Time.timeScale;

        currentLevel = SceneManager.GetActiveScene().buildIndex;

        cropsArray = GameObject.FindGameObjectsWithTag("Crop");
        cropCountOriginal = cropsArray.Length;
        //cropCount = cropsArray.Length;
        cropCountText.text = cropCount.ToString("F0");

        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        waveCount = 0;
        enemyScoreTotal = 0;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        waveTimerText.text = "--";
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
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }

    //Game States
    public void StatePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void StateUnpause()
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
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void GameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            if (waveCount < numOfWaves)
            {
                // Start the next wave if there are waves left
                if (waveTimer != null)
                {
                    StopCoroutine(waveTimer);
                }
                waveTimer = StartCoroutine(NextWaveWithCooldown(5.0f));
            }
            else
            {
                // End game with win condition if all waves are completed
                StatePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
                string starKey = $"Level_{currentLevel}_Star";
                PlayerPrefs.SetInt(starKey, 1); // 1 indicates a star earned
                PlayerPrefs.Save();
                // Set Win UI Stuff
                if (currentLevel + 1 > totalLevelCount)
                {
                    nextLevelButton.interactable = false;
                }
                else
                {
                    nextLevelButton.interactable = true;
                }

                int score = CalculateScore();
                scoreText.text = $"Score: {score}";

                // High score stuff
                string scoreKey = $"Level {currentLevel} Highscore";
                int highScore = PlayerPrefs.GetInt(scoreKey, 0);

                if (score > highScore || highScore == 0)
                {
                    highscoreText.text = "New Highscore!";
                    PlayerPrefs.SetInt(scoreKey, score);
                }
                else
                {
                    highscoreText.text = $"Highscore: {highScore}";
                }
            }
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

    public IEnumerator ApplyShieldEffect(float duration)
    {
        if (effectShield != null)
        {
            effectShield.SetActive(true);
            yield return new WaitForSeconds(duration);
            effectShield.SetActive(false);
        }
    }


    public void StartNextWave()
    {
        if (waveCount < numOfWaves)
        {
            waveCount++;
            UpdateWaveCountUI();
            StartCoroutine(SpawnWave());
        }
        else
        {
            Debug.Log("All waves completed.");
            waveTimerText.text = "--";
        }
    }

    IEnumerator NextWaveWithCooldown(float cooldown)
    {
        // Wait for cooldown before starting next wave
        for (int i = 0; i < cooldown; i++)
        {
            yield return new WaitForSeconds(1.0f);
            waveTimerText.text = (cooldown - i - 1).ToString();
        }

        waveTimerText.text = "--";

        if (cropCount > 0)
        {
            StartNextWave();
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (waveTimer != null)
        {
            StopCoroutine(waveTimer);
        }
        waveTimer = StartCoroutine(NextWaveWithCooldown(waveCooldown));
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Count > 0 && molePrefabs.Count > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject enemyPrefab = molePrefabs[Random.Range(0, molePrefabs.Count)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void UpdateEnemyCountUI()
    {
        enemyCountText.text = enemyCount.ToString("F0");
    }

    void UpdateWaveCountUI()
    {
        waveCountText.text = waveCount + " / " + numOfWaves;
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevel;
    }

    private int CalculateScore()
    {
        return Mathf.RoundToInt(((float)cropCount / (float)cropCountOriginal) * (float)enemyScoreTotal);
    }
}
