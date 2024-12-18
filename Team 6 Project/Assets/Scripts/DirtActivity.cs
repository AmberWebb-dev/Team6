using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DirtActivity : MonoBehaviour
{
    
    [SerializeField] GameObject prefab;

    private GameObject newCrop;

    bool hasCrop;
    bool inRange;

    Vector3 newCropPos;

    private bool plantCooldown = false;

    private void Update()
    {
        if (inRange && !hasCrop && Input.GetButtonDown("Plant Crop") && GameManager.Instance.playerScript.currentSeedsInInventory > 0)
        {
            PlantSeeds();
        }
        else if (hasCrop && inRange || GameManager.Instance.playerScript.currentSeedsInInventory <= 0)
        {
            GameManager.Instance.RemoveControlPopup("Plant Crop");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddControlPopup("Plant Crop", "G");
            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Crop"))
        {
            hasCrop = true;
        }
        else
        {
            hasCrop = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Crop"))
        {
            hasCrop = false;
        }
        
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RemoveControlPopup("Plant Crop");
            inRange = false;
        }
    }

    private void PlantSeeds()
    {
        if (!plantCooldown)
        {
            Debug.Log("Placing Seed");
            GameManager.Instance.playerScript.PlaceCrop();
            newCropPos = gameObject.transform.position;
            newCropPos.y = prefab.transform.position.y;
            newCrop = Instantiate(prefab, newCropPos, Quaternion.identity);
            GameManager.Instance.AddCropToArray(newCrop);
            AudioManager.Instance.plantCropSound.PlayAtPoint(newCropPos);

            plantCooldown = true;
            Invoke("PlantCooldown", 5.0f);
        }
    }

    void PlantCooldown()
    {
        plantCooldown = false;
    }
}
