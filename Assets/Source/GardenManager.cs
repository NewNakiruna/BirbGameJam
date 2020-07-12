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

    public class GardenManager : MonoBehaviour
    {
        // Begin Inspector Values
        public float MinTimeBetweenSpawns;
        public float MaxTimeBetweenSpawns;
        public int ProbabilityForPests = 40;
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
                                gv.Leave();
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

        void ResetGameProgress()
        {

        }

        void AttemptSpawn()
        {
            GardenVisitor attemptedVisitor = ChooseVisitorType();
            bool isGround = attemptedVisitor.IsGround;

            FeederLandingSpot landingSpot = null;
            Feeder spawningFeeder = null;
            foreach (Feeder feeder in PossibleFeeders)
            {
                if (feeder.GetIsUnlocked())
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
                GardenVisitor newVisitor = Instantiate(attemptedVisitor, spawnPosition, Quaternion.identity);
                newVisitor.InitializeGardenVisitor();
                CurrentGardenVisitors.Add(newVisitor);
            }
        }

        GardenVisitor ChooseVisitorType()
        {
            GardenVisitor visitorTypePrefab = null;

            int pestRoll = Random.Range(1, 100);
            if (pestRoll <= ProbabilityForPests)
            {
                IEnumerable<GardenVisitor> pests = PossibleGardenVisitorPrefabs.Where(visitor => visitor.IsGround);
                int roll = Random.Range(0, pests.Count() - 1);
                visitorTypePrefab = pests.ElementAt(roll);
            }
            else
            {
                IEnumerable<GardenVisitor> notPests = PossibleGardenVisitorPrefabs.Where(visitor => !visitor.IsGround);
                int roll = Random.Range(0, notPests.Count() - 1);
                visitorTypePrefab = notPests.ElementAt(roll);
            }

            return visitorTypePrefab;
        }

        Spawner ChooseSpawner(bool isGround)
        {
            Spawner spawner = null;

            if (isGround)
            {
                IEnumerable<Spawner> groundSpawners = PossibleSpawners.Where(spawn => spawn.IsGround);

            }
            else
            {
                IEnumerable<Spawner> airSpawners = PossibleSpawners.Where(spawn => !spawn.IsGround);

            }

            return spawner;
        }

        Feeder GetFeederById(int feederId)
        {
            return null;
        }

        Spawner GetSpawnerById(int spawnerId)
        {
            return null;
        }

        GardenVisitor GetGardenVisitorTypeById(int gvId)
        {
            return null;
        }

        void TapVisitor(GardenVisitor visitor)
        {

        }

        void TapFeeder(Feeder feeder)
        {

        }

        void MoveVisitor(GardenVisitor visitor)
        {

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
