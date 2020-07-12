using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirbSimulator
{
    public class GardenVisitor : MonoBehaviour
    {
        // Begin Inspector Values
        public int VisitorId;
        public string DisplayName = "Birb";
        public string DisplayDescription = "Is a birb. Look.";
        public int EatDuration;
        public int EatAmountPerSec;
        public EResourceType EatType = EResourceType.ERT_Seed;
        public int MoneyRewardAmount;
        public int SeedRewardId;
        public EResourceType RewardType = EResourceType.ERT_Money;
        public bool LeavesOnTap = true;
        public bool MustEatToTap = true;
        public int MinRarity;
        public int MaxRarity;
        public bool IsGround;
        public Animator VisitorAnimator;
        // End Inspector Values

        // Non-Inspector Values
        protected int PendingEatAmount;
        protected int ConsumedAmount;
        protected bool IsEating;
        protected float EatTimer;
        protected bool PendingLeave;
        protected bool HasBeenTapped;
        protected int SpawnerId;
        protected int FeederId;
        protected int FeederLandingSpotId;
        protected EVisitorAnimState CurrentAnimState;

        public void InitializeGardenVisitor()
        {
            SpawnerId = -1;
            FeederId = -1;
            FeederLandingSpotId = -1;
            IsEating = false;
            PendingLeave = false;
        }

        public void UpdateGardenVisitor(float deltaTime)
        {
            if (IsEating)
            {
                UpdateEatTimer(deltaTime);
            }
        }

        public void UpdateEatTimer(float deltaTime)
        {
            EatTimer += deltaTime;

            int floorSecs = Mathf.FloorToInt(EatTimer);
            int projectedTotal = floorSecs * EatAmountPerSec;
            int difference = projectedTotal - ConsumedAmount;

            PendingEatAmount += difference;
            if (EatTimer >= EatDuration)
            {
                IsEating = false;
                PendingLeave = true;
            }
        }

        public int GetSpawnerId()
        {
            return SpawnerId;
        }

        public void SetSpawnerId(int spawnerId)
        {
            SpawnerId = spawnerId;
        }

        public int GetFeederId()
        {
            return FeederId;
        }

        public void SetFeederId(int feederId)
        {
            FeederId = feederId;
        }

        public int GetFeederLandingSpotId()
        {
            return FeederLandingSpotId;
        }

        public void SetFeederLandingSpotId(int feederLandingSpotId)
        {
            FeederLandingSpotId = feederLandingSpotId;
        }

        public void BeginEating()
        {
            IsEating = true;
        }

        public int GetPendingEatAmount()
        {
            return PendingEatAmount;
        }

        public void ConsumePending()
        {
            ConsumedAmount += PendingEatAmount;
            PendingEatAmount = 0;
        }

        public bool CanTap()
        {
            return !MustEatToTap || (MustEatToTap && IsEating);
        }

        public void OnTap()
        {
            if (LeavesOnTap)
            {
                Leave();
            }
        }

        public void Leave()
        {
            if (ConsumedAmount <= 0)
            {
                PendingEatAmount = EatAmountPerSec;
                ConsumePending();
            }

            IsEating = false;
            PendingLeave = true;
        }

        public void SetAnimState(EVisitorAnimState newState)
        {
            if (VisitorAnimator != null)
            {
                switch(newState)
                {
                    case EVisitorAnimState.EVAS_Idle:
                        break;
                    case EVisitorAnimState.EVAS_Move:
                        break;
                    case EVisitorAnimState.EVAS_Eat:
                        break;
                }
            }
            CurrentAnimState = newState;
        }
    }

}
