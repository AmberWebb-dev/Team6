using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [Header("----- Powerup Settings -----")]
    [SerializeField] PlayerController.PowerupType type;
    [SerializeField] float duration;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.AddPowerup(type, duration);
            AudioManager.Instance.powerupPickupSound.PlayOnPlayer();
            Destroy(gameObject);
        }
    }
}
