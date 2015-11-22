using System;
using System.Collections.Generic;
using Timber_and_Stone.Tasks;
using Timber_and_Stone.Utility;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.Blocks;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.API.Event.Task;
using Timber_and_Stone.Profession.Human;
using EventHandler = Timber_and_Stone.API.Event.EventHandler;
using System.Linq;

namespace Plugin.Squancher.AdventureMod
{
    public class MessengerManager : MonoBehaviour
    {
        public static bool bAdventureMapOpen, isMapCreated, isAwaitingResults;
        public static HumanEntity messengerEntity;
        private int MessengerOdds;
        public static int day;
        private bool hasRolledToday;
        public static HashSet<IBlock> spawnPositions;

        public MessengerManager()
        {
        }

        public void Start()
        {
            hasRolledToday = false;
            isAwaitingResults = false;
            spawnPositions = getSpawnPositions();
        }

        public static void SpawnMessenger()
        {
            Vector3 vector;
            if (AManager<DesignManager>.getInstance().edgeRoads.Count == 0)
            {
                return;
            }
            int num = UnityEngine.Random.Range(0, 4);
            if (num == 0 && !AManager<DesignManager>.getInstance().edgeRoadSouth)
            {
                return;
            }
            if (num == 1 && !AManager<DesignManager>.getInstance().edgeRoadNorth)
            {
                return;
            }
            if (num == 2 && !AManager<DesignManager>.getInstance().edgeRoadEast)
            {
                return;
            }
            if (num == 3 && !AManager<DesignManager>.getInstance().edgeRoadWest)
            {
                return;
            }
            vector = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            //UnitManager.getInstance().AddHumanUnit();
            Debug.Log(vector);
            HumanEntity humanEntity = AManager<AssetManager>.getInstance().InstantiateUnit<HumanEntity>();
            int gender = UnityEngine.Random.Range(0, 100);
            humanEntity.gender = gender <= 50 ? APlayableEntity.Gender.Female: APlayableEntity.Gender.Male;
            humanEntity.unitName = "Messenger";
            humanEntity.fatigue = 1f;
            humanEntity.hunger = 0f;
            humanEntity.maxHP = 100f;
            humanEntity.hitpoints = 100f;
            humanEntity.coordinate = Coordinate.FromWorld(vector);
            humanEntity.faction = AManager<WorldManager>.getInstance().MigrantFaction;
            humanEntity.spottedTimer = 600000f;
            humanEntity.addProfession(new Messenger(humanEntity, 0));
            humanEntity.SetProfession(typeof(Messenger));
            humanEntity.interruptTask(new TaskMoveToHall(humanEntity));
            UnitManager.getInstance().visitors.Add(humanEntity.transform);
            messengerEntity = humanEntity;
            isAwaitingResults = true;
            day = AManager<TimeManager>.getInstance().day;
            GUIManager.getInstance().AddTextLine("A messenger has arrived." , humanEntity.transform, true);
        }

        public static void SpawnBandit()
        {
            MessengerManager mm = new MessengerManager();
            spawnPositions = mm.getSpawnPositions();
            float difficulty = mm.setDifficulty(0);
            float weapons = equipUnit(0);
            int ran = UnityEngine.Random.Range(8, 13);



            while (difficulty > 100)
            {
                if (spawnPositions.Count <= 0)
                {
                    break;
                }
                int i = UnityEngine.Random.Range(0, 100);
                i = i < 50 ? 0 : 1;
                IBlock block = spawnPositions.ElementAt(0);
                spawnPositions.Remove(block);
                //vector = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
                //UnitManager.getInstance().AddHumanUnit();
                //Debug.Log(vector);
                //UnitManager.getInstance().AddHumanUnit("Archer", vector, true, false, false);
                HumanEntity humanEntity = AManager<AssetManager>.getInstance().InstantiateUnit<HumanEntity>();
                int gender = UnityEngine.Random.Range(0, 100);
                humanEntity.gender = gender <= 50 ? APlayableEntity.Gender.Female : APlayableEntity.Gender.Male;
                humanEntity.unitName = "Bandit " + UnitManager.getInstance().RandomName(gender <= 50 ? true : false);
                humanEntity.fatigue = 1f;
                humanEntity.hunger = 0f;
                humanEntity.maxHP = 100f;
                humanEntity.hitpoints = 100f;
                humanEntity.coordinate = block.coordinate;
                humanEntity.faction = AManager<WorldManager>.getInstance().NeutralHostileFaction;
                humanEntity.spottedTimer = 600000f;
                if (i == 0)
                {
                    humanEntity.addProfession(new Archer(humanEntity, (int)AManager<ResourceManager>.getInstance().getWealth() * 100));
                    humanEntity.SetProfession(typeof(Archer));
                }
                if (i == 1)
                {
                    humanEntity.addProfession(new Infantry(humanEntity, (int)AManager<ResourceManager>.getInstance().getWealth() * 100));
                    humanEntity.SetProfession(typeof(Infantry));
                }
                //UnitManager.getInstance().visitors.Add(humanEntity.transform);
                GUIManager.getInstance().AddTextLine("Bandits have come for blood!", humanEntity.transform, true);
                if (weapons > 500)
                {
                    if (i == 1)
                    {
                    
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[4]), 1);     // 1-4
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[5]), 1);       //chest 3-5
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[11]), 1);       //head 9-11
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[16]), 1);      //feet 14-16
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[20]), 1);      //shield 17-20
                    }
                    if (i == 0)
                    {
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[8]), 1);     // 5-8
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[14]), 30);   // 13-15
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[2]), 1);       //chest 0-2
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[8]), 1);       //head 6-8
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[13]), 1);      //feet 12-13
                    }
                    weapons -= 500;
                }
                else
                {
                    if (i == 1)
                    {

                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[(int)UnityEngine.Random.Range(1,2)]), 1);     // 1-4
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(3, 4)]), 1);       //chest 3-5
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(9, 10)]), 1);       //head 9-11
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(14, 15)]), 1);      //feet 14-16
                        if ((int)UnityEngine.Random.Range(0, 100) > 50)
                        {
                            humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(17, 20)]), 1);      //shield 17-20
                        }
                    }
                    if (i == 0)
                    {
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[(int)UnityEngine.Random.Range(5, 7)]), 1);     // 5-8
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().weaponsSortType[13]), 20);   // 13-15
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(0, 2)]), 1);       //chest 0-2
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(6, 7)]), 1);       //head 6-8
                        humanEntity.inventory.Add(Resource.FromID(GUIManager.getInstance().armorSortType[(int)UnityEngine.Random.Range(12, 13)]), 1);      //feet 12-13
                    }
                }
                BattleManager.GetEnemyTarget(humanEntity);

                difficulty -= 100;
            }
        }

        public static int equipUnit(int type)
        {
            int weaponbonus = 0;
            int food, coin;
            int weapons = 0;
            int armor = 0;


            Resource resource2 = Resource.FromID(GUIManager.getInstance().rawMatsSortType[0]);
            food = WorldManager.getInstance().PlayerFaction.storage.getResource(resource2);

            resource2 = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[0]);
            coin = WorldManager.getInstance().PlayerFaction.storage.getResource(resource2);

            for (int i = 0; i < GUIManager.getInstance().weaponsSortType.Length; i++)
            {
                resource2 = Resource.FromID(GUIManager.getInstance().armorSortType[i]);
                weapons += WorldManager.getInstance().PlayerFaction.storage.getResource(resource2);
            }
            for (int i = 0; i < GUIManager.getInstance().armorSortType.Length; i++)
            {
                resource2 = Resource.FromID(GUIManager.getInstance().armorSortType[i]);
                armor += WorldManager.getInstance().PlayerFaction.storage.getResource(resource2);
            }
            coin *= 10;
            weapons *= 50;
            armor *= 50;
            weaponbonus += food + coin + weapons + armor;
            //GUIManager.getInstance().AddTextLine("Food: " + food + " Coin: " + coin + " Weapons: " + weapons + " Armor: " + armor + " Weapon bonus: " + weaponbonus);

            return weaponbonus;
        }

        public int setDifficulty(int type)
        {
            
            float difficulty = -650f;
            
            switch (type)
            {
                //bandits more into loot than day time and unit count. measure food, coins, weapons, and armor.
                case 0:
                    if (AManager<ResourceManager>.getInstance().getWealth() <= 0)
                    {
                        difficulty += 25f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
                    }
                    else
                    {
                        difficulty += 30f * AManager<ResourceManager>.getInstance().getWealth();
                    }
                    //difficulty += 25f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
                    difficulty += 10f * (float)AManager<TimeManager>.getInstance().day;
                    difficulty = (BattleManager.isFighting) == true ? 601: difficulty;
                    //GUIManager.getInstance().AddTextLine("Difficulty: " + difficulty);
                    return (int)difficulty;
                //Standard invasion check with 0 wealth check added.
                case 1:
                    if (AManager<ResourceManager>.getInstance().getWealth() <= 0)
                    {
                        difficulty += 75f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
                    }
                    else
                    {
                        difficulty += AManager<ResourceManager>.getInstance().getWealth();
                    }

                    difficulty += 100f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
                    difficulty += 50f * (float)AManager<TimeManager>.getInstance().day;
                    return (int)difficulty;
            }
            
            return (int)difficulty;
        }

        public HashSet<IBlock> getSpawnPositions()
        {
            ChunkManager instance = AManager<ChunkManager>.getInstance();
            Vector3i vector3i = new Vector3i(instance.chunkSize.x * instance.worldSize.x, instance.worldSize.y, instance.chunkSize.z * instance.worldSize.z);
            HashSet<IBlock> hashSet = new HashSet<IBlock>();

            switch ((int)UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    for (int i = 0; i < vector3i.z; i++)
                    {
                        IBlock block = instance.GetBlock(Coordinate.FromBlock(0, vector3i.y - 1, i));
                        block = instance.GetBlockOnTop(block);
                        if (block.properties.canUnitWalkOn() && block.relative(0, 1, 0).properties.canUnitWalkThrough() && block.relative(0, 2, 0).properties.canUnitWalkThrough() && block.properties != BlockProperties.BlockWater)
                        {
                            hashSet.Add(block.relative(0, 1, 0));
                        }
                    }
                    break;
                case 1:
                    for (int j = 0; j < vector3i.z; j++)
                    {
                        IBlock block2 = instance.GetBlock(Coordinate.FromBlock(vector3i.x - 1, vector3i.y - 1, j));
                        block2 = instance.GetBlockOnTop(block2);
                        if (block2.properties.canUnitWalkOn() && block2.relative(0, 1, 0).properties.canUnitWalkThrough() && block2.relative(0, 2, 0).properties.canUnitWalkThrough() && block2.properties != BlockProperties.BlockWater)
                        {
                            hashSet.Add(block2.relative(0, 1, 0));
                        }
                    }
                    break;
                case 2:
                    for (int k = 0; k < vector3i.x; k++)
                    {
                        IBlock block3 = instance.GetBlock(Coordinate.FromBlock(k, vector3i.y - 1, 0));
                        block3 = instance.GetBlockOnTop(block3);
                        if (block3.properties.canUnitWalkOn() && block3.relative(0, 1, 0).properties.canUnitWalkThrough() && block3.relative(0, 2, 0).properties.canUnitWalkThrough() && block3.properties != BlockProperties.BlockWater)
                        {
                            hashSet.Add(block3.relative(0, 1, 0));
                        }
                    }
                    break;
                default:
                    for (int l = 0; l < vector3i.x; l++)
                    {
                        IBlock block4 = instance.GetBlock(Coordinate.FromBlock(l, vector3i.y - 1, vector3i.z - 1));
                        block4 = instance.GetBlockOnTop(block4);
                        if (block4.properties.canUnitWalkOn() && block4.relative(0, 1, 0).properties.canUnitWalkThrough() && block4.relative(0, 2, 0).properties.canUnitWalkThrough() && block4.properties != BlockProperties.BlockWater)
                        {
                            hashSet.Add(block4.relative(0, 1, 0));
                        }
                    }
                    break;
            }
            return hashSet;
        }

        public bool GetMessenger()
        {
            if (messengerEntity.isAlive() && messengerEntity.getProfession() is Messenger)
            {
                Coordinate coordinate = messengerEntity.coordinate;
                if (AManager<ChunkManager>.getInstance().chunkArray[coordinate.chunk.x, coordinate.chunk.y - 1, coordinate.chunk.z].blocks[coordinate.block.x, coordinate.block.y, coordinate.block.z].isHall)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnGUI()
        {
            if (!BattleManager.isInTown)
            {
                return;
            }

            if (messengerEntity != null)
            {
                hasRolledToday = true;
                if (GetMessenger())
                {
                    AManager<TimeManager>.getInstance().pause();
                    MessengerMenu.OpenWindow();
                }
            }
            if (!hasRolledToday)
            { 
                if ((TimeManager.getInstance().hour == 8 || TimeManager.getInstance().hour == 12 || TimeManager.getInstance().hour == 16) && UnityEngine.Random.value < 0.25f)
                {
                    SpawnMessenger();
                }
            }
            else
            {
                if ((TimeManager.getInstance().hour < 8 || TimeManager.getInstance().hour > 16))
                {
                    hasRolledToday = false;
                }
            }
            //***************************
            
        }

        public void Update()
        {
        }
    }
}