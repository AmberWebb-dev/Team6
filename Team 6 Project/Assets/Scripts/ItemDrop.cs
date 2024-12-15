using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Header("----- Item Drop Settings -----")]
    [SerializeField] private ItemDropType dropType;
    [SerializeField] private int value; // Arbituary value, not necessarily needed for use

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (dropType)
            {
                case ItemDropType.Coin:
                    GameManager.Instance.AddCoins(value);
                    break;
                case ItemDropType.Health:
                    AudioManager.Instance.healSound.PlayOnPlayer();
                    GameManager.Instance.playerScript.HealAmount(value);
                    break;
            }
            Destroy(gameObject);
        }
    }

    private enum ItemDropType { Coin, Health }
}
