using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BirbSimulator
{
    public class FeederItem : MonoBehaviour
    {
        public Image SeedImage;
        public Text SeedNameText;
        public Text SeedRarityText;
        public Text SeedQuantityText;

        protected FeederModal ModalRef;
        protected Seed SeedRef;

        public void InitFeederItem(Seed seed, int quantity, FeederModal parentModal)
        {
            SeedImage.sprite = seed.gameObject.GetComponent<Image>().sprite;
            SeedNameText.text = seed.DisplayName;
            SeedRarityText.text = "" + seed.Rarity;
            SeedQuantityText.text = "" + quantity;

            SeedRef = seed;
            ModalRef = parentModal;
        }

        public void OnClickFeederItem()
        {
            ModalRef.OnClickFeederItem(SeedRef);
        }
    }
}
