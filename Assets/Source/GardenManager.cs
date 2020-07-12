using System.Collections;
using System.Collections.Generic;
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
        public List<Feeder> PossibleFeeders;
        public List<Spawner> PossibleSpawners;
        public List<GardenVisitor> PossibleGardenVisitorPrefabs;
        public List<Seed> PossibleSeedTypes;
        // End Inspector Values

        // Non-Inspector Values
        protected List<GardenVisitor> CurrentGardenVisistors;
        protected List<int> SeenVisitorIds;
        protected float TimeSinceLastSpawn;
        protected Inventory PlayerInventory;

        // Start is called before the first frame update
        void Start()
        {
            bool gameLoaded = LoadGame();
            if (!gameLoaded)
            {
                InitializeNewGame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void InitializeNewGame()
        {
            if (PossibleFeeders.Count > 0 && PossibleFeeders[0] != null)
            {
                PossibleFeeders[0].InitializeFeeder();
            }

            if (PlayerInventory != null)
            {
                PlayerInventory.InitializePlayerInventory();
            }
        }

        void ResetGameProgress()
        {

        }

        void AttemptSpawn()
        {

        }

        bool CanSpawn()
        {
            return false;
        }

        int ChooseVisitorType()
        {
            return -1;
        }

        int ChooseSpawner()
        {
            return -1;
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
