// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

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
    [Header("----- Popups -----")]
    [SerializeField] Transform controlPopupTransform;
    [SerializeField] GameObject controlPopupPrefab;
    private Dictionary<string, GameObject> controlPopups = new Dictionary<string, GameObject>();
    [Header("----- Text -----")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text cropCountText;
    [SerializeField] TMP_Text waveTimerText;
    [SerializeField] TMP_Text waveCountLoseText;
    [SerializeField] TMP_Text coinCountText;
    [SerializeField] TMP_Text cropInventoryText;
    [SerializeField] TMP_Text seedInventoryText;

    [Header("----- Screen Effects -----")]
    public GameObject playerDamageScreen;
    [SerializeField] GameObject effectBlind;
    [SerializeField] GameObject effectShield;
    public bool isPaused;
    float timeScaleOrig;

    [Header("----- Powerup UI -----")]
    [SerializeField] Transform powerupIconsContent;
    [SerializeField] GameObject powerupIconPrefab;
    private List<GameObject> powerupIcons = new List<GameObject>();
    private List<Image> powerupImages = new List<Image>();
    int powerupCount = 0;

    [Header("----- Player Stuff -----")]
    public PlayerController playerScript;
    public GameObject player;
    public Image playerHPBar;
    [Header("----- Crop Info -----")]
    public DirtActivity DirtActivityScript;
    public List<GameObject> cropsArray; //only public for debugging purposes (do not change in inspector)
    int cropArrayCount;
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
    private int coins;

    //endless mode
    [SerializeField] bool isEndlessMode;
    private int difficultyMultiplier;
    private float spawnRateMultiplier;
    private float waveCooldownReduction;
    private float healthScale;
    private float speedScale;
    private float damageScale;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        timeScaleOrig = Time.timeScale;

        currentLevel = SceneManager.GetActiveScene().buildIndex;

        cropCountOriginal = 50;
        cropCountText.text = cropCount.ToString("F0");
        cropsArray.AddRange(GameObject.FindGameObjectsWithTag("Crop"));

        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        waveCount = 0;
        enemyScoreTotal = 0;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        UpdateCropInventory(0);
        UpdateSeedInventory(0);

        SetCoins(0);

        waveTimerText.text = "--";
        StartNextWave();
    }

    public void UnregisterCrop(GameObject crop)
    {
        for (int i = 0; i < cropArrayCount; i++)
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

    public void AddCropToArray(GameObject newCrop)
    {
        cropsArray.Add(newCrop);
    }


    void Update()
    {
        cropArrayCount = cropsArray.Count;
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

        UpdatePowerupIcons();
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

        waveCountLoseText.text = $"Wave: {waveCount}";

        if (isEndlessMode)
        {
            int prevHighscore = PlayerPrefs.GetInt("EndlessRecord", 0);
            if (prevHighscore < waveCount)
            {
                PlayerPrefs.SetInt("EndlessRecord", waveCount);
            }
        }
    }
    public void GameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
        Debug.Log($"CropCount: " + (float)cropCount + " CropOrig: " + (float)cropCountOriginal + " EnemyScore: " + (float)enemyScoreTotal);


        if (enemyCount <= 0)
        {
            if (waveCount < numOfWaves || isEndlessMode)
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

    public void UpdateCropInventory(int amount)
    {
        cropInventoryText.text = amount.ToString("F0") + " / " + Instance.playerScript.maxCropInInventory.ToString("F0");

    }

    public void UpdateSeedInventory(int amount)
    {
        seedInventoryText.text = $"{amount} / {playerScript.maxSeedsInInventory}";
    }

    public IEnumerator ApplyBlindEffect()
    {
        if (effectBlind != null)
        {
            effectBlind.SetActive(true);
            yield return new WaitForSeconds(3.5f);
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
      

        if (!isEndlessMode && waveCount >= numOfWaves)
        {
            Debug.Log("All waves completed.");
            waveTimerText.text = "--";
            return;
        }

        if (isEndlessMode)
        {
            ScaleDifficulty(); // Adjust difficulty for endless mode
            UpdateWaveCountUI();
            StartCoroutine(SpawnWave());
        }

        if (waveCount < numOfWaves || isEndlessMode)
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
        if (waveCount < numOfWaves || isEndlessMode)
        {
            // Wait for cooldown before starting next wave
            for (int i = 0; i < cooldown; i++)
            {
                yield return new WaitForSeconds(1.0f);
                waveTimerText.text = (cooldown - i - 1).ToString();
            }
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
        //check for spawn and enemy prefabs
        if (spawnPoints.Count > 0 && molePrefabs.Count > 0)
        {
            //randomly select spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            //randomly select enemy prefab
            GameObject enemyPrefab = molePrefabs[Random.Range(0, molePrefabs.Count)];
            //initiate enemy prefab to spawn point
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            //checking for endless mode
            if (isEndlessMode)
            {
                //get cdenemy script
                CDEnemyAI enemyAI = enemy.GetComponent<CDEnemyAI>();
                if (enemyAI != null)
                {
                    // Scale enemy properties
                    enemyAI.HP = Mathf.RoundToInt(enemyAI.HP * healthScale);
                    if (enemyAI.agent != null)
                    {
                        enemyAI.agent.speed *= speedScale;
                    }
                    //scales enemy attack
                    enemyAI.attackDamage = Mathf.RoundToInt(enemyAI.attackDamage * damageScale);
                }
            }
        }
    }

    void UpdateEnemyCountUI()
    {
        enemyCountText.text = enemyCount.ToString("F0");
    }

    void UpdateWaveCountUI()
    {
        if (isEndlessMode)
        {
            waveCountText.text = waveCount.ToString();
        }
        else
        {
            waveCountText.text = waveCount + " / " + numOfWaves;
        }

    }

    public int GetCurrentLevelIndex()
    {
        return currentLevel;
    }

    private int CalculateScore()
    {
        return Mathf.RoundToInt(((float)cropCount / (float)cropCountOriginal) * (float)enemyScoreTotal);
    }

    public void AddControlPopup(string action, string key)
    {
        if (controlPopups.ContainsKey(action))
        {
            RemoveControlPopup(action);
        }

        GameObject popup = Instantiate(controlPopupPrefab, controlPopupTransform);
        popup.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{key}: {action}";

        controlPopups.Add(action, popup);
    }

    public void RemoveControlPopup(string action)
    {
        if (!controlPopups.ContainsKey(action)) { return; }

        GameObject popup = controlPopups[action];
        controlPopups.Remove(action);
        Destroy(popup);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) { coins = 0; }

        coinCountText.text = coins.ToString();
    }

    public void SetCoins(int amount)
    {
        coins = amount;
        if (coins < 0) { coins = 0; }

        coinCountText.text = coins.ToString();
    }

    public int GetCoinCount()
    {
        return coins;
    }

    //endless mode
    private void ScaleDifficulty()
    {
        // Adjust scaling factors for enemies based on wave count
        healthScale = Mathf.Pow(1.1f, waveCount);
        speedScale = Mathf.Pow(1.05f, waveCount);
        damageScale = Mathf.Pow(1.05f, waveCount);

        Debug.Log($"Scaling difficulty: Health x{healthScale}, Speed x{speedScale}, Damage x{damageScale}");
    }

    private void ResetPowerupIcons()
    {
        while (powerupIcons.Count > 0)
        {
            Destroy(powerupIcons[0]);
            powerupIcons.RemoveAt(0);
            powerupImages.RemoveAt(0);
        }

        for (int i = 0; i < playerScript.activePowerups.Count; i++)
        {
            GameObject icon = Instantiate(powerupIconPrefab, powerupIconsContent);
            powerupIcons.Add(icon);
            powerupImages.Add(icon.transform.GetChild(0).GetComponent<Image>());

            icon.transform.GetChild(1).GetComponent<TMP_Text>().text = playerScript.activePowerups[i].type.ToString();
        }
    }

    private void UpdatePowerupIcons()
    {
        if (powerupCount != playerScript.activePowerups.Count)
        {
            ResetPowerupIcons();
            powerupCount = playerScript.activePowerups.Count;
        }

        for (int i = 0; i < playerScript.activePowerups.Count; i++)
        {
            powerupImages[i].fillAmount = playerScript.activePowerups[i].duration / playerScript.activePowerups[i].maxDuration;
        }
    }
}
