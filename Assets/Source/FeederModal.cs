using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirbSimulator
{
    public class FeederModal : MonoBehaviour
    {
        public GameObject ContentRoot;
        public FeederItem FeederItemPrefab;

        protected GardenManager GardenManagerRef;
        protected Feeder FeederRef;

        public void InitFeederModal(Feeder feeder, GardenManager gardenManager)
        {
            FeederRef = feeder;
            GardenManagerRef = gardenManager;

            Dictionary<int, int> seedInventory = GardenManagerRef.GetSeeds();
            foreach (int seedId in seedInventory.Keys)
            {
                Seed seedtype = GardenManagerRef.GetSeedTypeById(seedId);
                FeederItem item = Instantiate<FeederItem>(FeederItemPrefab, ContentRoot.transform);
                item.InitFeederItem(seedtype, seedInventory[seedId], this);
            }
        }

        public void OnClickFeederItem(Seed seedType)
        {
            GardenManagerRef.FillFeederWithSeed(FeederRef, seedType);
            OnClickExit();
        }

        public void OnClickExit()
        {
            Debug.Log("Clicked exit.");
            GardenManagerRef.NavigationManager.CloseFeederMenu();
            Destroy(gameObject);
        }
    }
}
