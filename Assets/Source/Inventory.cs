using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirbSimulator
{
    public class Inventory : Object
    {
        // Non-Inspector Values
        protected int Money;
        protected Dictionary<int, int> SeedInventory; // seed id, amount

        public void InitializePlayerInventory()
        {
            Money = 0;
            if (SeedInventory != null)
            {
                SeedInventory.Clear();
            }
            else
            {
                SeedInventory = new Dictionary<int, int>();
            }
        }

        public int GetMoney()
        {
            return Money;
        }

        public void UpdateMoney(int amount)
        {
            Money = Mathf.Max(0, Money + amount);
        }

        public Dictionary<int, int> GetSeeds()
        {
            return SeedInventory;
        }

        public void AddSeed(int seedId, int amount)
        {
            if (SeedInventory.ContainsKey(seedId))
            {
                int currentAmount = SeedInventory[seedId];
                currentAmount++;
                SeedInventory[seedId] = currentAmount;
            }
            else
            {
                SeedInventory.Add(seedId, amount);
            }
        }

        public void RemoveSeed(int seedId, int amount)
        {
            if (SeedInventory.ContainsKey(seedId))
            {
                int currentAmount = SeedInventory[seedId];
                currentAmount--;
                if (currentAmount <= 0)
                {
                    SeedInventory.Remove(seedId);
                }
                else
                {
                    SeedInventory[seedId] = currentAmount;
                }
            }
        }
    }
}
