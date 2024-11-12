using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float shieldDuration;

    //spawning behavior stuff
    public Vector2 spawnRangeX;
    public Vector2 spawnRangeZ;
    public float yPosition = 1f;
    public float respawnDelay;

    private static bool shieldActive;

    private void Start()
    {
        shieldActive = true;

        StartCoroutine(respawnShield());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && shieldActive)
        {
            //debugging-ignore
            Debug.Log("Player collected shield, starting respawn coroutine");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateShield(shieldDuration);
            }

            shieldActive = false;

            StartCoroutine(respawnShield());

            //gameObject.SetActive(false);
            transform.position = new Vector3(-999, -999, -999); // Move it out of view
        }
    }

    private IEnumerator respawnShield()
    {
        //debugging-ignore
        Debug.Log("spawnShield coroutine started");

        yield return new WaitForSeconds(respawnDelay);

        // Center point offset
        float centerX = 30f;
        float centerZ = -10f;

        //generating the new position within the floor
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y) + centerX;
        float randomZ = Random.Range(spawnRangeZ.x, spawnRangeZ.y) + centerZ;

        Vector3 spawnPosition = new Vector3(randomX, yPosition, randomZ);

        //move the shield to the new positon and reactivate it
        transform.position = spawnPosition;
        //debugging-ignore
        Debug.Log("shield moved! omg it moved! youre not crazy!");

        //debugging-ignore
        Debug.Log("respawning shield at new position");
        
        gameObject.SetActive(true);

        //setting shield to to true to confirm new shield is active.
        shieldActive = true;
        
    }
}
