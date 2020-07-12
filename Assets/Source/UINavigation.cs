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
        Debug.Log(Buttons[2].name);
    }
}
