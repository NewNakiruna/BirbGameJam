using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject shopModal;

    protected GameObject thisShopWindow;

    public void OpenShopWindow()
    {
        thisShopWindow = Instantiate(shopModal, contentPanel);
        Component[] Buttons = thisShopWindow.GetComponentsInChildren<Button>();
<<<<<<< HEAD
        Buttons[0].GetComponent<Button>().onClick.AddListener(delegate { ResetGameState(); });
        Buttons[1].GetComponent<Button>().onClick.AddListener(delegate { CloseShopWindow(); });
        Buttons[2].GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });
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

=======
        Debug.Log(Buttons[2].name);
>>>>>>> parent of 0ebfcc5... Revert "Merge branch 'master' of https://github.com/NewNakiruna/BirbGameJam"
    }
}
