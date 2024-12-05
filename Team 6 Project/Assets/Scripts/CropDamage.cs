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

    bool inRange;
    bool fullInventory;

    Color colourOriginal;

    void Start()
    {
        colourOriginal = model.material.color;
        GameManager.Instance.UpdateCrop(1);
    }

    void Update()
    {
        if(inRange && Input.GetButtonDown("Pick Up") && !fullInventory)
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
        //GameManager.Instance.UpdateCropInventory(GameManager.Instance.playerScript.currentCropsInInventory);
        Debug.Log($"Crop added to inventory");

    }

    void PlaceCrop()
    {
        if (GameManager.Instance.playerScript.currentCropsInInventory > 0)
        {
            GameManager.Instance.playerScript.PlaceCrop();

            Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
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
            //GameManager.Instance.UnregisterCrop(gameObject); // Unregister before destruction
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
}
