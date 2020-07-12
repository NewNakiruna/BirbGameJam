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

    public void OpenShopWindow()
    {
        thisShopWindow = Instantiate(shopModal, contentPanel);
        Component[] Buttons = thisShopWindow.GetComponentsInChildren<Button>();
        
        Buttons[0].GetComponent<Button>().onClick.AddListener(delegate { ResetGameState(); });
        Buttons[1].GetComponent<Button>().onClick.AddListener(delegate { CloseShopWindow(); });
        Buttons[2].GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });

        BuildTheShop();
    }

    void BuildTheShop()
    {
        Transform contentPanelForShop = thisShopWindow.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);
        for (int i = 0;i<thisGardenManager.PossibleSeedTypes.Count;i++)
        {
            GameObject lineItem = Instantiate(shopItem,contentPanelForShop);
            lineItem.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = thisGardenManager.PossibleSeedTypes[i].DisplayName;
        }
    }

    void CreateShopItem()
    {

    }

    void ResetGameState()
    {

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
