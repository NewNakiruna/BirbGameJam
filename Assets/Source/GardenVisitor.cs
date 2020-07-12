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
        public EVisitorPersuasion VisitorPersuasion = EVisitorPersuasion.EVP_Regular;
        public bool IsGround;
        public float MoveSpeed = 1.0f;
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
        protected float LerpPosition;
        protected EVisitorAnimState CurrentAnimState;
        protected Animator VisitorAnimator;

        public void InitializeGardenVisitor()
        {
            SpawnerId = -1;
            FeederId = -1;
            FeederLandingSpotId = -1;
            EatTimer = 0.0f;
            IsEating = false;
            LerpPosition = 1.0f;
            PendingLeave = false;
            VisitorAnimator = gameObject.GetComponent<Animator>();
            SetAnimState(EVisitorAnimState.EVAS_Move);
        }

        public void UpdateGardenVisitor(float deltaTime)
        {
            if (IsEating)
            {
                UpdateEatTimer(deltaTime);
            }
            else
            {
                UpdateLerpPosition(deltaTime);
            }
        }

        void UpdateEatTimer(float deltaTime)
        {
            EatTimer += deltaTime;

            int floorSecs = Mathf.FloorToInt(EatTimer);
            int projectedTotal = floorSecs * EatAmountPerSec;
            int difference = projectedTotal - ConsumedAmount;

            PendingEatAmount += difference;
            if (EatTimer >= EatDuration)
            {
                Leave();
            }
        }

        void UpdateLerpPosition(float deltaTime)
        {
            if (PendingLeave)
            {
                LerpPosition = Mathf.Min(1.0f, LerpPosition + (MoveSpeed * deltaTime));
            }
            else
            {
                LerpPosition = Mathf.Max(0.0f, LerpPosition - (MoveSpeed * deltaTime));
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

        public bool GetIsEating()
        {
            return IsEating;
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

        public bool GetPendingLeave()
        {
            return PendingLeave;
        }

        public void Leave()
        {
            SetAnimState(EVisitorAnimState.EVAS_Move);
            gameObject.transform.Rotate(Vector3.up, 180);

            if (ConsumedAmount <= 0 && MustEatToTap)
            {
                PendingEatAmount = EatAmountPerSec;
            }

            IsEating = false;
            PendingLeave = true;
        }

        public float GetLerpPosition()
        {
            return LerpPosition;
        }

        void OnMouseDown()
        {
            if (CanTap())
            {
                Debug.Log(DisplayName + " click.");
                gameObject.transform.parent.GetComponent<GardenManager>().TapVisitor(this);
            }
        }

        public void SetAnimState(EVisitorAnimState newState)
        {
            CurrentAnimState = newState;
            if (VisitorAnimator != null)
            {
                switch (CurrentAnimState)
                {
                    case EVisitorAnimState.EVAS_Idle:
                        VisitorAnimator.SetBool("IsStand", true);
                        VisitorAnimator.SetBool("IsFly", false);
                        VisitorAnimator.SetBool("IsEat", false);
                        break;
                    case EVisitorAnimState.EVAS_Eat:
                        VisitorAnimator.SetBool("IsEat", true);
                        VisitorAnimator.SetBool("IsFly", false);
                        VisitorAnimator.SetBool("IsStand", false);
                        break;
                    case EVisitorAnimState.EVAS_Move:
                        VisitorAnimator.SetBool("IsFly", true);
                        VisitorAnimator.SetBool("IsStand", false);
                        VisitorAnimator.SetBool("IsEat", false);
                        break;
                }
            }
        }
    }

}
