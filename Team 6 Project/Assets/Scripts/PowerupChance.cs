using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerupChance
{
    public GameObject powerupPrefab;
    public float chanceToSpawn;

    public bool RollChance()
    {
        float r = Random.Range(0.0f, 1.0f);
        if (r <= chanceToSpawn)
        {
            return true;
        }
        return false;
    }

    public void SpawnPowerup(Vector3 position)
    {
        MonoBehaviour.Instantiate(powerupPrefab, position, Quaternion.identity);
    }
}
