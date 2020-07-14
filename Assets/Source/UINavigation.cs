using BirbSimulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public Transform contentPanel;
    public GardenManager thisGardenManager;
    public GameObject shopModal;
    public ShopItem shopItem;
    public FeederModal feederModal;
    public FeederItem feederItem;

    protected GameObject thisShopWindow;
    protected Button[] buyButtons;

    public void OpenShopWindow()
    {
        thisGardenManager.SetUIOpen(true);

        thisShopWindow = Instantiate(shopModal, contentPanel);
        Component[] Buttons = thisShopWindow.GetComponentsInChildren<Button>();
        
        Buttons[0].GetComponent<Button>().onClick.AddListener(delegate { ResetGameState(); });
        Buttons[1].GetComponent<Button>().onClick.AddListener(delegate { CloseShopWindow(); });
        Buttons[2].GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });

        BuildTheShop();
        //UpdateTheShop();
    }

    void BuildTheShop()
    {
        buyButtons = new Button[thisGardenManager.PossibleSeedTypes.Count];
        Transform contentPanelForShop = thisShopWindow.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);
        for (int i = 0;i<thisGardenManager.PossibleSeedTypes.Count;i++)
        {
            ShopItem lineItem = Instantiate(shopItem, contentPanelForShop);
            lineItem.InitShopItem(thisGardenManager.PossibleSeedTypes[i], thisGardenManager);
        }
    }

    public void UpdateTheShop()
    {
        for (int i = 0; i < thisGardenManager.PossibleSeedTypes.Count; i++)
        {
            if (thisGardenManager.GetMoney() < thisGardenManager.PossibleSeedTypes[i].Cost)
            {
                buyButtons[i].interactable = false;
            }
            else
            {
                buyButtons[i].interactable = true;
            }
        }
    }

    void CloseShopWindow()
    {
        thisGardenManager.SetUIOpen(false);
        GameObject.Destroy(thisShopWindow);
    }

    public void OpenFeederMenu()
    {
        thisGardenManager.SetUIOpen(true);
        FeederModal modal = Instantiate(feederModal, contentPanel);
        modal.InitFeederModal(thisGardenManager);
    }

    public void CloseFeederMenu()
    {
        thisGardenManager.SetUIOpen(false);
    }

    void ResetGameState()
    {
        thisGardenManager.ResetGameProgress();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
