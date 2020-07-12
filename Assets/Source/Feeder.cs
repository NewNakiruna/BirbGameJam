using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BirbSimulator
{

    public class Feeder : MonoBehaviour
    {
        // Begin Inspector Values
        public int FeederId;
        public int MaxPossibleSlots;
        public int StartingSlotAmount;
        public int MaxFeed;
        public List<FeederLandingSpot> AirLandingSpots;
        public List<FeederLandingSpot> GroundLandingSpots;
        public Image FillImage;
        // End Inspector Values

        // Non-Inspector Values
        protected int CurrentAvailableSlotAmount;
        protected bool IsUnlocked;
        protected int CurrentFeedAmount;
        protected int CurrentFeedRarity;
        

        public void InitializeFeeder()
        {
            CurrentAvailableSlotAmount = StartingSlotAmount;
            CurrentFeedAmount = 0;
            CurrentFeedRarity = -1;
            IsUnlocked = false;
            FillImage.fillAmount = 0;
        }

        public void UpdateFeeder(float deltaTime)
        {
            FillImage.fillAmount = (float)CurrentFeedAmount / (float)MaxFeed;
        }

        public bool CanSpawnAir()
        {
            int filledAirSlots = AirLandingSpots.Where(airSlot => airSlot.GetIsFilled()).Count();
            return filledAirSlots < CurrentAvailableSlotAmount;
        }

        public bool CanSpawnGround()
        {
            int filledGroundSlots = GroundLandingSpots.Where(groundSlot => groundSlot.GetIsFilled()).Count();
            return filledGroundSlots < GroundLandingSpots.Count;
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
            //If CurrentFeedAmount is greater than or equal to the desired Consume amount, update the CurrentFeedAmount. Otherwise, consume what is remaining.
            if(CurrentFeedAmount >= amount)
            {
                CurrentFeedAmount = CurrentFeedAmount - amount;
            }
            else
            {
                CurrentFeedAmount = 0;
                CurrentFeedRarity = -1;
            }

            FillImage.fillAmount = (float)CurrentFeedAmount / (float)MaxFeed;
        }

        public int AssignSlot(bool isGround)
        {
            int spotId = -1;
            FeederLandingSpot spot = null;
            if (isGround)
            {
                spot = GetNextFreeGroundSlot();
            }
            else
            {
                spot = GetNextFreeAirSpot();
            }

            if (spot != null)
            {
                spot.FillSlot();
                spotId = spot.LandingSpotId;
            }

            return spotId;
        }

        public void ReleaseSlot(bool isGround, int slotId)
        {
            FeederLandingSpot spot = null;
            if (isGround)
            {
                spot = GetGroundLandingSpotById(slotId);
            }
            else
            {
                spot = GetAirLandingSpotById(slotId);
            }

            if (spot != null)
            {
                spot.EmptySlot();
            }
        }

        public int GetNextFreeAirSpotId()
        {
            int landingSpotId = -1;
            //Loop through LandingSpots for an empty FeederLandingSpot
            for (int i=0; i < AirLandingSpots.Count; i++)
            {
                //If FeederLandingSpot is empty, update returnValue to return the position in the array.
                if(!AirLandingSpots[i].GetIsFilled())
                {
                    landingSpotId =  AirLandingSpots[i].LandingSpotId;
                    break;
                }
            }
            return landingSpotId;
        }

        public FeederLandingSpot GetNextFreeAirSpot()
        {
            FeederLandingSpot landingSpot = null;
            //Loop through LandingSpots for an empty FeederLandingSpot
            for (int i = 0; i < AirLandingSpots.Count; i++)
            {
                //If FeederLandingSpot is empty, update returnValue to return the position in the array.
                if (!AirLandingSpots[i].GetIsFilled())
                {
                    landingSpot = AirLandingSpots[i];
                    break;
                }
            }
            return landingSpot;
        }

        public FeederLandingSpot GetAirLandingSpotById(int landingSpotId)
        {
            int landingSpotIndex = -1;
            //Loop through LandingSpots looking for landingSpotId
            for (int i = 0; i < AirLandingSpots.Count; i++)
            {
                if (AirLandingSpots[i].LandingSpotId == landingSpotId)
                {
                    landingSpotIndex = i;
                    break;
                }
            }

            //If landingSpotId was not found, return null. Else, return found FeederLandingSpot.
            if (landingSpotIndex == -1)
            {
                return null;
            }
            else
            {
                return AirLandingSpots[landingSpotIndex];
            }
        }

        public int GetNextFreeGroundSlotId()
        {
            int landingSpotId = -1;
            //Loop through LandingSpots for an empty FeederLandingSpot
            for (int i = 0; i < GroundLandingSpots.Count; i++)
            {
                //If FeederLandingSpot is empty, update returnValue to return the position in the array.
                if (!GroundLandingSpots[i].GetIsFilled())
                {
                    landingSpotId = GroundLandingSpots[i].LandingSpotId;
                    break;
                }
            }
            return landingSpotId;
        }

        public FeederLandingSpot GetNextFreeGroundSlot()
        {
            FeederLandingSpot landingSpot = null;
            //Loop through LandingSpots for an empty FeederLandingSpot
            for (int i = 0; i < GroundLandingSpots.Count; i++)
            {
                //If FeederLandingSpot is empty, update returnValue to return the position in the array.
                if (!GroundLandingSpots[i].GetIsFilled())
                {
                    landingSpot = GroundLandingSpots[i];
                    break;
                }
            }
            return landingSpot;
        }

        public FeederLandingSpot GetGroundLandingSpotById(int landingSpotId)
        {
            int landingSpotIndex = -1;
            //Loop through LandingSpots looking for landingSpotId
            for (int i = 0; i < GroundLandingSpots.Count; i++)
            {
                if (GroundLandingSpots[i].LandingSpotId == landingSpotId)
                {
                    landingSpotIndex = i;
                    break;
                }
            }

            //If landingSpotId was not found, return null. Else, return found FeederLandingSpot.
            if (landingSpotIndex == -1)
            {
                return null;
            }
            else
            {
                return GroundLandingSpots[landingSpotIndex];
            }
        }

        void OnMouseDown()
        {
            Debug.Log("Feeder click.");
        }
    }

}
