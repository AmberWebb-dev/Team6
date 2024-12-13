using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PowerupChance
{
    public GameObject powerupPrefab;
    public float chanceToSpawn;
    public int sceneLock = -1;

    public bool RollChance()
    {
        if (sceneLock == -1 || sceneLock == SceneManager.GetActiveScene().buildIndex)
        {
            float r = Random.Range(0.0f, 1.0f);
            if (r <= chanceToSpawn)
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnPowerup(Vector3 position)
    {
        MonoBehaviour.Instantiate(powerupPrefab, position, Quaternion.identity);
    }
}
