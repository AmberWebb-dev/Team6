using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{

    public Light playerLight;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the flashlight
        if (other.CompareTag("Player"))
        {
            // Enable the player's light
            if (playerLight != null)
            {
                playerLight.enabled = true;
            }

            // Destroy the flashlight object after it's picked up
            Destroy(gameObject);
        }
    }
}
