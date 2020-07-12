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
            BuyText.text = "" + seed.Cost;
            DescriptionText.text = seed.DisplayName;

            GardenManagerRef = gmRef;
            SeedRef = seed;
        }

        public void OnBuyButtonClicked()
        {
            GardenManagerRef.UpdateMoney(-(SeedRef.Cost));
            GardenManagerRef.AddSeed(SeedRef.SeedId, 1);
        }
    }
}
