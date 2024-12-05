using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtActivity : MonoBehaviour
{
    bool hasCrop;
    bool inRange;
    [SerializeField] GameObject prefab;

    Vector3 newCropPos;

    private void Update()
    {

        if (inRange && !hasCrop && Input.GetButtonDown("Plant Crop") && GameManager.Instance.playerScript.currentCropsInInventory > 0)
        {
            Debug.Log("Placing Crop");
            GameManager.Instance.playerScript.PlaceCrop();
            newCropPos = gameObject.transform.position;
            newCropPos.y = prefab.transform.position.y;
            Instantiate(prefab, newCropPos, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crop") && other.CompareTag("Player"))
        {
            hasCrop = true;
            Debug.Log($"Crop here");
        }
        else
        {
            hasCrop = false;
            Debug.Log($"no Crop here");
        }

        if (other.CompareTag("Player") && !hasCrop)
        {
            GameManager.Instance.AddControlPopup("Plant Crop", "G");
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Crop"))
        {
            hasCrop = true;
        }
        else
        {
            hasCrop = false;
        }
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RemoveControlPopup("Plant Crop");
            inRange = false;
        }
    }

}
