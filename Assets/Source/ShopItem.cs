using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BirbSimulator
{
    public class ShopItem : MonoBehaviour
    {
        public Image ItemImage;
        public Button BuyButton;
        public Text BuyText;
        public Text DescriptionText;

        protected GardenManager GardenManagerRef;
        protected Seed SeedRef;
               
        public void InitShopItem(Seed seed, GardenManager gmRef)
        {
            ItemImage.sprite = seed.gameObject.GetComponent<Image>().sprite;
            BuyText.text = "$ " + seed.Cost;
            DescriptionText.text = seed.DisplayName;
            ItemImage.color = seed.Color;

            GardenManagerRef = gmRef;
            SeedRef = seed;
        }

        public void OnBuyButtonClicked()
        {
            Debug.Log("I bought seed " + SeedRef.DisplayName + " for " + SeedRef.Cost);
            GardenManagerRef.UpdateMoney(-(SeedRef.Cost));
            GardenManagerRef.AddSeed(SeedRef.SeedId, 1);
            foreach (KeyValuePair<int, int> entry in GardenManagerRef.GetSeeds())
            {
                Debug.Log("I have the following Rarity " + entry.Key.ToString() + " seeds: " + entry.Value.ToString());
            }
        }
    }
}
