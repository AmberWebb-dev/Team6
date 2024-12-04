using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    enum pickupType { tool, seed, crop };
    [SerializeField] pickupType type;
    [SerializeField] ShovelStats shovel;
    bool inRange;
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && type == pickupType.tool)
        {
            PickUpShovel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            GameManager.Instance.AddControlPopup("Pick Up", "E");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            GameManager.Instance.RemoveControlPopup("Pick Up");
        }

    }

    void PickUpShovel()
    {
        Debug.Log($"Item Picked Up");
        GameManager.Instance.playerScript.GetShovelStats(shovel);
        Destroy(gameObject);
        GameManager.Instance.RemoveControlPopup("Pick Up");

    }
}
