using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteract
{
    [Header("----- Shop Settings -----")]
    [SerializeField] private ShopType shopType;
    [SerializeField] private string shopPopupText;
    [SerializeField] private int cost;

    [Header("----- Shop Text -----")]
    [SerializeField] TMPro.TMP_Text shopText;
    [SerializeField] Canvas textCanvas;

    bool isHovered;
    float interactionCooldown;

    public void Interact()
    {
        if (isHovered == false)
        {
            GameManager.Instance.AddControlPopup(shopPopupText, "E");
            textCanvas.gameObject.SetActive(true);
            shopText.text = $"{shopPopupText}\nCost: {cost}";
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
                            Debug.Log("BOUGHT SEEDS (SEEDS NOT IMPLEMENTED)");
                            GameManager.Instance.AddCoins(-cost);
                            // TODO: Add seeds when buying them
                        }
                        else
                        {
                            Debug.Log("CANNOT AFFORD SEEDS");
                        }
                        break;
                    case ShopType.ShovelShop:
                        if (GameManager.Instance.GetCoinCount() >= cost)
                        {
                            Debug.Log("BOUGHT SHOVEL (SHOVEL BUYING NOT IMPLEMENTED)");
                            GameManager.Instance.AddCoins(-cost);
                            // TODO: Add shovel when buying it
                        }
                        else
                        {
                            Debug.Log("CANNOT AFFORD SHOVEL");
                        }
                        break;
                    case ShopType.SellShop:
                        Debug.Log("SELLING CROPS (NOT IMPLEMENTED)");
                        break;
                }
            }
        }
    }

    enum ShopType { SeedShop, ShovelShop, SellShop }
}
