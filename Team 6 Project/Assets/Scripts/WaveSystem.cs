using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaveSystem : MonoBehaviour
{
    [SerializeField] GameObject molePrefab; // Prefab for the enemy
    [SerializeField] List<Transform> spawnPoints; // List of spawn points
    [SerializeField] int enemiesPerWave = 5; // Number of enemies per wave
    [SerializeField] float timeBetweenSpawns = 1f; // Time between enemy spawns
    [SerializeField] float waveCooldown = 35f; // Time between waves

    private bool isWaveDone;

    private void Start()
    {
        StartCoroutine(CreateMoleWave());
    }

    private void Update()
    {
        // You can check if the wave is done or manage other wave-related logic here
    }

    IEnumerator CreateMoleWave()
    {
        isWaveDone = false;

        // Spawn enemies
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        // Wait for a cooldown before the next wave
        yield return new WaitForSeconds(waveCooldown);
        isWaveDone = true;

        // Optionally, you can start the next wave here
        StartCoroutine(CreateMoleWave());
    }

    private void SpawnEnemy()
    {
        // Choose a random spawn point
        if (spawnPoints.Count > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(molePrefab, spawnPoint.position, spawnPoint.rotation);
            GameManager.Instance.GameGoal(1); // Increment enemy count in GameManager
        }
    }
}