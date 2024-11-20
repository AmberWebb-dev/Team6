// CropDamage.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDamage : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;

    Color colourOriginal;

    void Start()
    {
        colourOriginal = model.material.color;
        GameManager.Instance.UpdateCrop(1);
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
