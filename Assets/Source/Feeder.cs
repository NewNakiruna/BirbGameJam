using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirbSimulator
{

    public class Feeder : MonoBehaviour
    {
        // Begin Inspector Values
        public int FeederId;
        public int MaxPossibleSlots;
        public int StartingSlotAmount;
        public int MaxFeed;
        // End Inspector Values

        // Non-Inspector Values
        protected int CurrentSlotAmount;
        protected bool IsUnlocked;
        protected int CurrentFeedAmount;
        protected int CurrentFeedRarity;
        protected List<FeederLandingSpot> LandingSpots;

        public void InitializeFeeder()
        {
            CurrentSlotAmount = StartingSlotAmount;
            CurrentFeedAmount = 0;
            CurrentFeedRarity = -1;
        }

        public void UpdateFeeder(float deltaTime)
        {
            
        }

        public bool CanSpawn()
        {
            bool canSpawn = false;
            if(GetNextFreeSpot()!=-1)
            {
                canSpawn = true;
                Unlock();
            }
            else
            {
                Lock();
            }
            return canSpawn;
        }

        public void Unlock()
        {
            IsUnlocked = true;
        }

        public void Lock()
        {
            IsUnlocked = false;
        }

        public bool GetIsUnlocked()
        {
            return IsUnlocked;
        }

        public bool GetIsEmpty()
        {
            return CurrentFeedAmount <= 0;
        }

        public void FillFeeder(int feedRarity)
        {
            CurrentFeedAmount = MaxFeed;
            CurrentFeedRarity = feedRarity;
        }

        public int GetCurrentFeedRarity()
        {
            return CurrentFeedRarity;
        }

        public void Consume(int amount)
        {
            //If CurrentFeedAmount is greater than or equal to the desired Consume amount, update the CurrentFeedAmount. Otherwise, do nothing.
            if(CurrentFeedAmount >= amount)
            {
                CurrentFeedAmount = CurrentFeedAmount - amount;
            }
        }

        public int GetNextFreeSpot()
        {
            int returnValue = -1;
            //Loop through LandingSpots for an empty FeederLandingSpot
            for (int i=0; i < LandingSpots.count; i++)
            {
                //If FeederLandingSpot is empty, update returnValue to return the position in the array.
                if(LandingSpots[i].GetIsFilled() == false)
                {
                    returnValue =  i;
                    i = LandingSpots.count;
                }
            }
            return returnValue;
        }

        public FeederLandingSpot GetLandingSpotById(int landingSpotId)
        {
            int landingSpotLocation = -1;
            //Loop through LandingSpots looking for landingSpotId
            for (int i = 0; i < LandingSpots.count; i++)
            {
                if (LandingSpots[i].LandingSpotId == landingSpotId)
                {
                    landingSpotLocation = i;
                    i = LandingSpots.count;
                }
            }

            //If landingSpotId was not found, return null. Else, return found FeederLandingSpot.
            if (landingSpotLocation == -1)
            {
                return null;
            }
            else
            {
                return LandingSpots[landingSpotLocation];
            }
        }
    }

}
