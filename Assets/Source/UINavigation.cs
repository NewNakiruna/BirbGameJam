using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINavigation : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject shopModal;

    public void OpenShopWindow()
    {
        Instantiate(shopModal, contentPanel);
    }
}
