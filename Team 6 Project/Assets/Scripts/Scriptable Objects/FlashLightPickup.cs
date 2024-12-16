using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    public Light playerLight;

    private void Start()
    {
        if (playerLight == null)
        {
            playerLight = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Light>();
            if (playerLight == null)
            {
                Debug.LogError("Player Light could not be found in the scene!");
            }
            else
            {
                Debug.Log("Player Light successfully found: " + playerLight.name);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggered the flashlight!");

            if (playerLight != null)
            {
                playerLight.gameObject.SetActive(false); // Force refresh
                playerLight.gameObject.SetActive(true);  // Re-enable the light
            }
            else
            {
                Debug.LogError("Player Light is not assigned in the Inspector!");
            }

            Destroy(gameObject); // Remove flashlight pickup object
        }
    }
}