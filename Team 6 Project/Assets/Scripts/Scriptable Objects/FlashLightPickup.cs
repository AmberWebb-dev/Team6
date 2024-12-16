using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    public GameObject playerLight;

    private void Start()
    {
      
    }

    private void Update()
    {
        playerLight = GameObject.FindGameObjectWithTag("FlashLight");

        if(playerLight == null )
        {
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.flashlight.SetActive(true);

            Destroy(playerLight); // Remove flashlight pickup object
        }
    }
}