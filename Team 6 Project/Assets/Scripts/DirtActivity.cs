using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtActivity : MonoBehaviour
{
    bool hasCrop;
    bool inRange;

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
