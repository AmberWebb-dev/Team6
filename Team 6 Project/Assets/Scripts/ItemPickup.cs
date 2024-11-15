using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    enum pickupType { tool };
    [SerializeField] pickupType type;
    [SerializeField] ShovelStats shovel;
    void Start()
    {
        if (type == pickupType.tool)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.GetShovelStats(shovel);
            Destroy(gameObject);
        }
    }
}
