using BirbSimulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject shopModal;
    public GardenManager thisGardenManager;
    public GameObject shopItem;

    protected GameObject thisShopWindow;
    protected Button[] buyButtons;

    public void OpenShopWindow()
    {
        thisShopWindow = Instantiate(shopModal, contentPanel);
        Component[] Buttons = thisShopWindow.GetComponentsInChildren<Button>();
        
        Buttons[0].GetComponent<Button>().onClick.AddListener(delegate { ResetGameState(); });
        Buttons[1].GetComponent<Button>().onClick.AddListener(delegate { CloseShopWindow(); });
        Buttons[2].GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });

        BuildTheShop();
        UpdateTheShop();
    }

    void BuildTheShop()
    {
        buyButtons = new Button[thisGardenManager.PossibleSeedTypes.Count];
        Transform contentPanelForShop = thisShopWindow.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);
        for (int i = 0;i<thisGardenManager.PossibleSeedTypes.Count;i++)
        {
            GameObject lineItem = Instantiate(shopItem,contentPanelForShop);
            //Change Description Text
            lineItem.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = thisGardenManager.PossibleSeedTypes[i].DisplayName;
            
            //Change Buy Button text
            lineItem.transform.GetChild(0).transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "$"+thisGardenManager.PossibleSeedTypes[i].Cost.ToString();

            //Add Buy Button to buyButtons list
            buyButtons[i] = lineItem.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();

            //Add Listener to the Buy Button to remove money = to cost and add 1 seed to player inventory
            lineItem.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                delegate { thisGardenManager.UpdateMoney(-thisGardenManager.PossibleSeedTypes[i].Cost); thisGardenManager.AddSeed(thisGardenManager.PossibleSeedTypes[i].SeedId, 1); });
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

    void ResetGameState()
    {
        thisGardenManager.ResetGameProgress();
    }

    void CloseShopWindow()
    {
        GameObject.Destroy(thisShopWindow);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
