using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BirbSimulator
{
    public enum EResourceType
    {
        ERT_Seed,
        ERT_Money
    }

    public enum EVisitorAnimState
    {
        EVAS_Idle,
        EVAS_Move,
        EVAS_Eat
    }

    public enum EVisitorPersuasion
    {
        EVP_Regular,
        EVP_Pest,
        EVP_Special
    }

    public class GardenManager : MonoBehaviour
    {
        // Begin Inspector Values
        public float MinTimeBetweenSpawns;
        public float MaxTimeBetweenSpawns;
        public int ProbabilityForPests = 30;
        public int ProbabilityForSpecial = 10;
        public List<Feeder> PossibleFeeders;
        public List<Spawner> PossibleSpawners;
        public List<GardenVisitor> PossibleGardenVisitorPrefabs;
        public List<Seed> PossibleSeedTypes;
        // End Inspector Values

        // Non-Inspector Values
        protected List<GardenVisitor> CurrentGardenVisitors;
        protected List<int> SeenVisitorIds;
        protected float TimeSinceLastSpawn;
        protected float TimeBetweenSpawns;
        protected Inventory PlayerInventory;

        // Start is called before the first frame update
        void Start()
        {
            bool gameLoaded = LoadGame();
            if (!gameLoaded)
            {
                InitializeNewGame();
            }

            CurrentGardenVisitors = new List<GardenVisitor>();

            TimeSinceLastSpawn = 0.0f;
            TimeBetweenSpawns = Random.Range(MinTimeBetweenSpawns, MaxTimeBetweenSpawns);
        }

        // Update is called once per frame
        void Update()
        {
            float frameDeltaTime = Time.deltaTime;

            TimeSinceLastSpawn += frameDeltaTime;
            if (TimeSinceLastSpawn >= TimeBetweenSpawns)
            {
                AttemptSpawn();

                TimeSinceLastSpawn = 0.0f;
                TimeBetweenSpawns = Random.Range(MinTimeBetweenSpawns, MaxTimeBetweenSpawns);
            }

            foreach (GardenVisitor visitor in CurrentGardenVisitors)
            {
                Feeder feeder = GetFeederById(visitor.GetFeederId());
                bool eating = visitor.GetIsEating();
                bool leaving = visitor.GetPendingLeave();

                visitor.UpdateGardenVisitor(frameDeltaTime);
                int amountToConsume = visitor.GetPendingEatAmount();
                
                if (!eating)
                {
                    // t = 0 is a (feeder)
                    // t = 1 is b (spawner)
                    // Vector2.Lerp(Vector2 a, Vector2 b, float t)
                    FeederLandingSpot landingSpot = null;
                    if (visitor.IsGround)
                    {
                        landingSpot = feeder.GetGroundLandingSpotById(visitor.GetFeederLandingSpotId());
                    }
                    else
                    {
                        landingSpot = feeder.GetAirLandingSpotById(visitor.GetFeederLandingSpotId());
                    }
                    Vector2 landingSpotPosition2d = landingSpot.transform.position;

                    Spawner spawner = GetSpawnerById(visitor.GetSpawnerId());
                    Vector2 spawnerPosition2d = spawner.transform.position;

                    float visitorLerpPosition = visitor.GetLerpPosition();
                    visitor.transform.position = Vector2.Lerp(landingSpotPosition2d, spawnerPosition2d, visitorLerpPosition);

                    if (visitor.GetPendingLeave())
                    {
                        if (visitorLerpPosition >= 1.0f)
                        {
                            CurrentGardenVisitors.Remove(visitor);
                            Destroy(visitor);
                        }
                    }
                    else
                    {
                        if (visitorLerpPosition <= 0.0f)
                        {
                            visitor.BeginEating();
                        }
                    }
                }

                switch (visitor.EatType)
                {
                    case EResourceType.ERT_Seed:
                        feeder.Consume(amountToConsume);
                        if (feeder.GetIsEmpty())
                        {
                            foreach (GardenVisitor gv in CurrentGardenVisitors.Where(gardenVisitor => gardenVisitor.GetFeederId() == feeder.FeederId))
                            {
                                if (gv.MinRarity >= 0)
                                {
                                    gv.Leave();
                                }
                            }
                        }
                        break;
                    case EResourceType.ERT_Money:
                        PlayerInventory.UpdateMoney(-1 * amountToConsume);
                        break;
                }
                visitor.ConsumePending();
            }
        }

        void InitializeNewGame()
        {
            foreach (Feeder feeder in PossibleFeeders)
            {
                feeder.InitializeFeeder();
            }

            if (PossibleFeeders.Count > 0 && PossibleFeeders[0] != null)
            {
                PossibleFeeders[0].Unlock();
            }

            PlayerInventory = new Inventory();
            PlayerInventory.InitializePlayerInventory();
        }

        public void ResetGameProgress()
        {

        }

        void AttemptSpawn()
        {
            EVisitorPersuasion persuasion = ChooseVisitorPersuasion();
            
            bool isGround = persuasion == EVisitorPersuasion.EVP_Pest;

            FeederLandingSpot landingSpot = null;
            Feeder spawningFeeder = null;
            foreach (Feeder feeder in PossibleFeeders)
            {
                if (feeder != null && feeder.GetIsUnlocked())
                {
                    if (isGround && feeder.CanSpawnGround())
                    {
                        landingSpot = feeder.GetNextFreeGroundSlot();
                        spawningFeeder = feeder;
                        break;
                    }

                    if (!isGround && feeder.CanSpawnAir())
                    {
                        landingSpot = feeder.GetNextFreeAirSpot();
                        spawningFeeder = feeder;
                        break;
                    }
                }
            }

            if (spawningFeeder != null && landingSpot != null)
            {
                Spawner spawner = ChooseSpawner(isGround);
                Vector3 spawnPosition = spawner.transform.position;
                spawnPosition.z = 0;

                GardenVisitor attemptedVisitor = ChooseVisitorType(spawningFeeder.GetCurrentFeedRarity(), persuasion);
                if (attemptedVisitor == null)
                {
                    return;
                }

                GardenVisitor newVisitor = Instantiate(attemptedVisitor, spawnPosition, Quaternion.identity);
                newVisitor.InitializeGardenVisitor();
                newVisitor.SetFeederId(spawningFeeder.FeederId);
                newVisitor.SetFeederLandingSpotId(landingSpot.LandingSpotId);
                newVisitor.SetSpawnerId(spawner.SpawnerId);

                CurrentGardenVisitors.Add(newVisitor);
            }
        }

        EVisitorPersuasion ChooseVisitorPersuasion()
        {
            EVisitorPersuasion persuasion = EVisitorPersuasion.EVP_Regular;

            int roll = Random.Range(1, 100);
            if (roll <= (ProbabilityForPests + ProbabilityForSpecial))
            {
                persuasion = EVisitorPersuasion.EVP_Special;
            }

            if (roll <= ProbabilityForPests)
            {
                persuasion = EVisitorPersuasion.EVP_Pest;
            }

            return persuasion;
        }

        GardenVisitor ChooseVisitorType(int rarity, EVisitorPersuasion persuasion)
        {
            GardenVisitor visitorTypePrefab = null;

            IEnumerable<GardenVisitor> filteredList = PossibleGardenVisitorPrefabs.Where(visitor => visitor.MinRarity <= rarity && visitor.MaxRarity >= rarity && visitor.VisitorPersuasion == persuasion);
            int filteredCount = filteredList.Count();
            if (filteredCount > 0)
            {
                int roll = Random.Range(0, filteredCount - 1);
                visitorTypePrefab = filteredList.ElementAt(roll);
            }

            return visitorTypePrefab;
        }

        Spawner ChooseSpawner(bool isGround)
        {
            Spawner spawner = null;

            if (isGround)
            {
                IEnumerable<Spawner> groundSpawners = PossibleSpawners.Where(spawn => spawn.IsGround);
                int roll = Random.Range(0, groundSpawners.Count() - 1);
                spawner = groundSpawners.ElementAt(roll);
            }
            else
            {
                IEnumerable<Spawner> airSpawners = PossibleSpawners.Where(spawn => !spawn.IsGround);
                int roll = Random.Range(0, airSpawners.Count() - 1);
                spawner = airSpawners.ElementAt(roll);
            }

            return spawner;
        }

        Feeder GetFeederById(int feederId)
        {
            Feeder feeder = null;

            foreach (Feeder test in PossibleFeeders)
            {
                if (test.FeederId == feederId)
                {
                    feeder = test;
                    break;
                }
            }

            return feeder;
        }

        Spawner GetSpawnerById(int spawnerId)
        {
            Spawner spawner = null;

            foreach (Spawner test in PossibleSpawners)
            {
                if (test.SpawnerId == spawnerId)
                {
                    spawner = test;
                    break;
                }
            }

            return spawner;
        }

        GardenVisitor GetGardenVisitorTypeById(int gvId)
        {
            GardenVisitor visitor = null;

            foreach (GardenVisitor test in PossibleGardenVisitorPrefabs)
            {
                if (test.VisitorId == gvId)
                {
                    visitor = test;
                    break;
                }
            }

            return visitor;
        }

        void TapVisitor(GardenVisitor visitor)
        {
            switch (visitor.RewardType)
            {
                case EResourceType.ERT_Money:
                    PlayerInventory.UpdateMoney(visitor.MoneyRewardAmount);
                    break;
                case EResourceType.ERT_Seed:
                    PlayerInventory.AddSeed(visitor.SeedRewardId, 1);
                    break;
            }

            visitor.OnTap();
        }

        void TapFeeder(Feeder feeder)
        {

        }

        void UpdateMoney(int amount)
        {
            PlayerInventory.UpdateMoney(amount);
        }

        void AddSeed(int seedId, int amount)
        {
            PlayerInventory.AddSeed(seedId, amount);
        }

        void RemoveSeed(int seedId, int amount)
        {
            PlayerInventory.RemoveSeed(seedId, amount);
        }

        void SaveGame()
        {

        }

        bool LoadGame()
        {
            return false;
        }
    }

}
