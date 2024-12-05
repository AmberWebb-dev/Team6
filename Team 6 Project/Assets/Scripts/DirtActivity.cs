using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtActivity : MonoBehaviour
{
    bool hasCrop;
    bool inRange;
    [SerializeField] GameObject prefab;

    private void Update()
    {
        if(!hasCrop && inRange)
        {
            Debug.Log($"inRange and Does not have crop!!");

        }
        else if(hasCrop && inRange)
        {
            Debug.Log($"inRangs and HAS crop");
            
        }
        else
        {
            //nothing
        }

        if (inRange && Input.GetButtonDown("Plant Crop") && GameManager.Instance.playerScript.currentCropsInInventory > 0)
        {
            Debug.Log("Placing Crop");
            GameManager.Instance.playerScript.PlaceCrop();

            Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crop"))
        {
            hasCrop = true;
           // Debug.Log($"Crop here");
        }
        else
        {
            hasCrop = false;
            //Debug.Log($"no Crop here");
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
