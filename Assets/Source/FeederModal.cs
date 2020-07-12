using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirbSimulator
{
    public class FeederModal : MonoBehaviour
    {
        public GameObject ContentRoot;
        protected GardenManager GardenManagerRef;

        public void InitFeederModal(GardenManager gardenManager)
        {
            GardenManagerRef = gardenManager;

        }

        public void OnClickExit()
        {
            GardenManagerRef.NavigationManager.CloseFeederMenu();
            Destroy(gameObject);
        }
    }
}
