// CropDamage.cs
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CropDamage : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] GameObject prefab;
    public int timeBetweenStages;
    public int maxGrowth;

    public int currentProgression;

    bool inRange;
    bool fullInventory;
    bool isHarvestable;

    Color colourOriginal;

    void Start()
    {
        colourOriginal = model.material.color;
        GameManager.Instance.UpdateCrop(1);
        currentProgression = 1;
        InvokeRepeating("PlantGrowth", timeBetweenStages, timeBetweenStages);
    }

    void Update()
    {
        if (!isHarvestable)
        {
            GameManager.Instance.RemoveControlPopup("Pick Up");
        }
        if (inRange && Input.GetButtonDown("Pick Up") && !fullInventory && isHarvestable)
        {
            if (GameManager.Instance.playerScript.currentCropsInInventory 
                < GameManager.Instance.playerScript.maxCropInInventory)
            {
                PickUpCrop();
                GameManager.Instance.UpdateCrop(-1);
            }
            else
            {
                fullInventory = true;
                Debug.Log($"Crop inventory full!");
            }
        }
    }

    void PickUpCrop()
    {
        GameManager.Instance.playerScript.PickUpCrop();
        Destroy(gameObject);
        GameManager.Instance.RemoveControlPopup("Pick Up");
        Debug.Log($"Crop added to inventory");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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

    public void TakeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            AudioManager.Instance.cropDeathSound.PlayOnPlayer();
            GameManager.Instance.UpdateCrop(-1);

            Destroy(gameObject);
        }
        else
        {
            AudioManager.Instance.cropHitSound.PlayAtPoint(transform.position);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colourOriginal;
    }

    void PlantGrowth()
    {
        if (currentProgression != maxGrowth)
        {
            gameObject.transform.GetChild(currentProgression).gameObject.SetActive(true);
        }
        if (currentProgression > 0 && currentProgression < maxGrowth)
        {
            gameObject.transform.GetChild(currentProgression - 1).gameObject.SetActive(false);
        }
        if(currentProgression < maxGrowth)
        {
            currentProgression++;
        }
        if (currentProgression == maxGrowth)
        {
            isHarvestable = true;
        }
    }
}
