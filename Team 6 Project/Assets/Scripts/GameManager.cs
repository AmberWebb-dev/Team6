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
    [SerializeField] Button creditsButton;
    [Header("----- Popups -----")]
    [SerializeField] Transform controlPopupTransform;
    [SerializeField] GameObject controlPopupPrefab;
    private Dictionary<string, GameObject> controlPopups = new Dictionary<string, GameObject>();
    [SerializeField] GameObject tutorialPopup;
    [SerializeField] Image tutorialTimer;
    float tutorialLength = 0.0f;
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
    public CropDamage cropScript;
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
    private int seedCount;

    //endless mode
    [SerializeField] bool isEndlessMode;
    private int difficultyMultiplier;
    private float spawnRateMultiplier;
    private float waveCooldownReduction;
    private float healthScale;
    private float speedScale;
    private float damageScale;

    [SerializeField] public GameObject flashlightText;

    private Coroutine blackoutCoroutine;
    private float blackoutTimer = 0f;
    private bool isBlinding = false;



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

        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1)
        {
            tutorialPopup.SetActive(false);
        }
        else
        {
            tutorialPopup.SetActive(true);
            PlayerPrefs.SetInt("TutorialComplete", 1);
            PlayerPrefs.Save();
            tutorialLength = 15.0f;
        }

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
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.P))
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

        if (tutorialLength <= 0)
        {
            tutorialPopup.SetActive(false);
        }
        else
        {
            tutorialTimer.fillAmount = tutorialLength / 15.0f;
            tutorialLength -= Time.deltaTime;
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

        AudioManager.Instance.loseSound.Play();

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

                AudioManager.Instance.winSound.Play();

                string starKey = $"Level_{currentLevel}_Star";
                PlayerPrefs.SetInt(starKey, 1); // 1 indicates a star earned
                PlayerPrefs.Save();
                // Set Win UI Stuff
                if (currentLevel + 1 > totalLevelCount)
                {
                    creditsButton.gameObject.SetActive(true);
                    nextLevelButton.gameObject.SetActive(false);
                    nextLevelButton.interactable = false;
                }
                else
                {
                    creditsButton.gameObject.SetActive(false);
                    nextLevelButton.gameObject.SetActive(true);
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
        seedCount = amount;
    }

    public void ApplyBlindEffect(float duration = 3.5f)
    {
        // Extend the blackout timer by adding the new duration
        blackoutTimer = Mathf.Max(blackoutTimer, Time.time + duration);

        // Start the effect if it isn’t already running
        if (!isBlinding)
        {
            StartCoroutine(HandleBlindEffect());
        }
    }

    private IEnumerator HandleBlindEffect()
    {
        isBlinding = true;

        if (effectBlind != null)
        {
            effectBlind.SetActive(true);

            while (Time.time < blackoutTimer) // Keeps checking if blackout should continue
            {
                yield return null; // Waits until the next frame
            }

            effectBlind.SetActive(false);
        }

        isBlinding = false; // Reset the flag when the blackout ends
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

            Animator anim = enemy.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.Rebind(); // Force rebinding the skeleton
                anim.Update(0); // Apply first frame immediately
                anim.cullingMode = AnimatorCullingMode.AlwaysAnimate; // Stop culling issues
                Debug.Log($"Animator initialized for {enemy.name} - State: {anim.GetCurrentAnimatorStateInfo(0).IsName("idle")}");
            }
            else
            {
                Debug.LogError($"No Animator found on {enemy.name}!");
            }

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

    public int GetSeedCount()
    {
        return seedCount;
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
