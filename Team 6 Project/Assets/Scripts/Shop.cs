using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteract
{
    [Header("----- Shop Settings -----")]
    [SerializeField] private ShopType shopType;
    [SerializeField] private string shopPopupText;
    [SerializeField] private int cost;
    [SerializeField] private GameObject shovelPickupPatch;

    [Header("----- Shop Text -----")]
    [SerializeField] TMPro.TMP_Text shopText;
    [SerializeField] Canvas textCanvas;

    [Header("----- Prefabs -----")]
    [SerializeField] private GameObject shovelPickupPrefab;
    private GameObject shovelPickup;

    bool isHovered;
    float interactionCooldown;

    public void Interact()
    {
        if (isHovered == false)
        {
            GameManager.Instance.AddControlPopup(shopPopupText, "E");
            textCanvas.gameObject.SetActive(true);
            string costText = (shopType == ShopType.SellShop) ? $"Coins per crop: {cost}" : $"Cost: {cost}";
            shopText.text = $"{shopPopupText}\n{costText}";
        }
        isHovered = true;
        interactionCooldown = 0.01f;
    }

    void Update()
    {
        if (interactionCooldown <= 0.0f)
        {
            if (isHovered)
            {
                GameManager.Instance.RemoveControlPopup(shopPopupText);
                textCanvas.gameObject.SetActive(false);
            }
            isHovered = false;
        }
        else
        {
            interactionCooldown -= Time.deltaTime;
        }

        if (isHovered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch(shopType)
                {
                    case ShopType.SeedShop:
                        if (GameManager.Instance.GetCoinCount() >= cost)
                        {
                            Debug.Log("BOUGHT SEEDS");
                            GameManager.Instance.AddCoins(-cost);
                            GameManager.Instance.playerScript.currentSeedsInInventory = GameManager.Instance.playerScript.maxSeedsInInventory;
                            GameManager.Instance.UpdateSeedInventory(GameManager.Instance.playerScript.maxSeedsInInventory);
                        }
                        else
                        {
                            Debug.Log("CANNOT AFFORD SEEDS");
                        }
                        break;
                    case ShopType.ShovelShop:
                        if (GameManager.Instance.GetCoinCount() >= cost && shovelPickup == null)
                        {
                            Debug.Log("BOUGHT SHOVEL");
                            GameManager.Instance.AddCoins(-cost);
                            shovelPickup = Instantiate(shovelPickupPrefab, shovelPickupPatch.transform.position + Vector3.up, Quaternion.identity);
                        }
                        else
                        {
                            Debug.Log("CANNOT AFFORD SHOVEL");
                        }
                        break;
                    case ShopType.SellShop:
                        Debug.Log($"{GameManager.Instance.playerScript.currentCropsInInventory} crops sold!");
                        GameManager.Instance.AddCoins(GameManager.Instance.playerScript.currentCropsInInventory * cost);
                        GameManager.Instance.playerScript.currentCropsInInventory = 0;
                        GameManager.Instance.UpdateCropInventory(0);
                        break;
                }
            }
        }
    }

    enum ShopType { SeedShop, ShovelShop, SellShop }
}
