using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [Header("----- Powerup Settings -----")]
    [SerializeField] PlayerController.PowerupType type;
    [SerializeField] float duration;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.AddPowerup(type, duration);
            Destroy(gameObject);
        }
    }
}
