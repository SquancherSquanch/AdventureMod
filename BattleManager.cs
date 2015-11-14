using System;
using System.Collections.Generic;
using Timber_and_Stone.Tasks;
using Timber_and_Stone.Utility;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.API.Event.Task;
using Timber_and_Stone.Profession.Human;
using EventHandler = Timber_and_Stone.API.Event.EventHandler;
using System.Linq;

namespace Plugin.Squancher.AdventureMod 
{
    public class Reward : IEquatable<Reward>
    {
        public int id { get; set; }
        public int type { get; set; }
        public int itemid { get; set; }
        public int amount { get; set; }

        public override string ToString()
        {
            return "ID: " + id + "Type: " + type + "ID: " + itemid + "   Amount: " + amount;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Reward objAsPart = obj as Reward;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return id;
        }
        public bool Equals(Reward other)
        {
            if (other == null) return false;
            return (this.id.Equals(other.id));
        }
        // Should also override == and != operators.
    }

    public class BattleManager : MonoBehaviour
    {
        public static int time, timesinceload;
        public static bool bAdventureMapOpen, isMapCreated, isStartingFight, isFighting, isPlacingUnits, isPlacingUnits2, isPlaced, isBattleOver, isFromTown, isToTown, isArrivingTown, isInTown;
        GUIManager guiMgr = GUIManager.getInstance();
        GameSave gamesave;
        private bool reverseSort;
        public static string invasion;
        public static List<Reward> rewards = new List<Reward>();
        public static int bonusLoot, foodAmount, coinAmount, EnemyCount, UnitsToPlace;

        public enum SortType
        {
            NONE,
            NAME,
            PROFESSION
        }

        

        public UnitList.SortType sortType;
        private Dictionary<Type, string> professionList = new Dictionary<Type, string>();
        public Type professionSort;
        public float bounty1, bounty2;
        public static Vector3 StartPosition;
        

        private BattleManager()
        {
        }

        public void Start()
        {
            isInTown = true;
        }

        private APlayableEntity[] SortUnits(APlayableEntity[] sortedUnits)
        {
            UnitList.SortType sortType = this.sortType;
            if (sortType != UnitList.SortType.NAME)
            {
                if (sortType == UnitList.SortType.PROFESSION)
                {
                    Array.Sort<APlayableEntity>(sortedUnits, delegate (APlayableEntity U1, APlayableEntity U2)
                    {
                        AProfession profession = U1.getProfession(this.professionSort);
                        AProfession profession2 = U2.getProfession(this.professionSort);
                        if (profession == null && profession2 == null)
                        {
                            return 0;
                        }
                        if (profession == null)
                        {
                            return -1;
                        }
                        if (profession2 == null)
                        {
                            return 1;
                        }
                        int num = profession.getLevel().CompareTo(profession2.getLevel());
                        if (num == 0)
                        {
                            AProfession profession3 = U1.getProfession();
                            AProfession profession4 = U2.getProfession();
                            if (profession3 == profession && profession4 == profession2)
                            {
                                return 0;
                            }
                            if (profession3 == profession)
                            {
                                return 1;
                            }
                            if (profession4 == profession2)
                            {
                                return -1;
                            }
                        }
                        return num;
                    });
                    if (!this.reverseSort)
                    {
                        Array.Reverse(sortedUnits);
                    }
                }
            }
            else
            {
                Array.Sort<APlayableEntity>(sortedUnits, (APlayableEntity U1, APlayableEntity U2) => U1.unitName.CompareTo(U2.unitName));
                if (this.reverseSort)
                {
                    Array.Reverse(sortedUnits);
                }
            }
            return sortedUnits;
        }
        
        public void DestoryAll()
        {

            foreach (ALivingEntity allEntities in UnitManager.getInstance().allUnits)
            {
                if (isArrivingTown)
                {
                    allEntities.Destroy();
                    continue;
                }
                if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(allEntities.faction) != Alignment.Ally)
                    allEntities.Destroy();
                if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(allEntities.faction) == Alignment.Neutral)
                    allEntities.Destroy();
                if (allEntities.unitName == "Sheep")
                    allEntities.Destroy();
                if (allEntities.unitName == "Boar")
                    allEntities.Destroy();
                if (allEntities.unitName == "Chicken")
                    allEntities.Destroy();
                //AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().selectedChunk = new Vector3(0,0,0);
                //UnityEngine.Object.Destroy(GUIManager.getInstance().desi.designSelection..GetComponents.DestoryAll);
            }
        }

        public static int PrepBattleField(int trade)
        {
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            BattleManager battleManager = new BattleManager();
            battleManager.SortUnits(array);

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                if (aPlayableEntity.isAlive())
                {
                    if (PartyMenu.draftees.Count != 0)
                    {
                        //place unit before battle
                        if (trade == 0)
                        {
                        
                            if (PartyMenu.draftees.Contains(new Draftees { uName = aPlayableEntity.unitName }))
                            {
                                if (PartyMenu.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted)
                                {
                                    aPlayableEntity.transform.position = new Vector3(1, 100, 1);
                                    aPlayableEntity.coordinate = Coordinate.FromWorld(aPlayableEntity.transform.position);
                                }
                                else
                                {
                                    aPlayableEntity.Destroy();
                                }
                            }
                        }
                    }
                }
            }
            EnemyCount = GetEnemyRemaining();
            return trade;
        }

        public static void PlaceUnits()
        {
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            BattleManager battleManager = new BattleManager();
            battleManager.SortUnits(array);
            StartPosition = GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().WorldPositionAtMouse();
            
            
            array[UnitsToPlace-1].transform.position = new Vector3(StartPosition.x, StartPosition.y, StartPosition.z);
            array[UnitsToPlace - 1].coordinate = Coordinate.FromWorld(array[UnitsToPlace - 1].transform.position);
        }

        public static void Reward(int clear = 0)
        {
            if (clear == 1)
            {
                rewards.Clear();
                return;
            }
            rewards.Clear();
            bonusLoot = UnityEngine.Random.Range(1, 7);
            bonusLoot = 7;

            foodAmount = UnityEngine.Random.Range(10,75);
            coinAmount = UnityEngine.Random.Range(5,30);

            for (int i = 0; i < bonusLoot; i++)
            {
                int lootTableRoll = UnityEngine.Random.Range(1, 100);
                int type, itemID, amount;
                bool bPass = false;
                itemID = 0;
                type = 0;
                amount = 0;

                //weapon armor
                if (lootTableRoll < 20)
                {
                    type = UnityEngine.Random.Range(0, 1);
                    amount = 1;

                    //weapon
                    if (type == 0)
                    {
                        itemID = UnityEngine.Random.Range(0, 16);
                        
                    }
                    //armor
                    if (type == 1)
                    {
                        itemID = UnityEngine.Random.Range(0, 20);
                        
                    }
                }

                //processed mats
                if (lootTableRoll > 19 && lootTableRoll < 50)
                {
                    itemID = UnityEngine.Random.Range(0, 4); //7-10

                    if (itemID == 0) { itemID = 7; }
                    if (itemID == 1) { itemID = 8; }
                    if (itemID == 2) { itemID = 9; }
                    amount = UnityEngine.Random.Range(1, 16);
                    if (itemID == 3) { itemID = 10; amount = UnityEngine.Random.Range(1, 8); }
                    if (itemID == 4) { itemID = 11; amount = UnityEngine.Random.Range(1, 6); }
                    type = 2;
                }
                //raw mats
                if (lootTableRoll > 49)
                {
                    itemID = UnityEngine.Random.Range(0, 14); //6-10 4, 12 - 15 3, 17-20 3

                    if (itemID == 0) { itemID = 6; }
                    if (itemID == 1) { itemID = 7; }
                    if (itemID == 2) { itemID = 8; }
                    if (itemID == 3) { itemID = 9; }
                    if (itemID == 4) { itemID = 10; }
                    if (itemID == 5) { itemID = 12; }
                    if (itemID == 6) { itemID = 13; }
                    if (itemID == 7) { itemID = 14; }
                    if (itemID == 8) { itemID = 15; }
                    if (itemID == 9) { itemID = 17; }
                    if (itemID == 10) { itemID = 18; }
                    if (itemID == 11) { itemID = 19; }
                    amount = UnityEngine.Random.Range(1, 24);
                    if (itemID == 12) { itemID = 20; amount = UnityEngine.Random.Range(1, 18); }
                    if (itemID == 13) { itemID = 21; amount = UnityEngine.Random.Range(1, 12); }
                    if (itemID == 14) { itemID = 22; amount = UnityEngine.Random.Range(1, 6); }
                    type = 3;
                }

                for (int j = 0; j < rewards.Count; j++)
                {
                    if (rewards.Find(x => x.id == j).type == type)
                    {
                        if (rewards.Find(x => x.id == j).itemid == itemID)
                        {
                            rewards.Find(x => x.id == j).amount += amount;
                            bPass = true;
                            continue;
                        }
                    }
                }
                if (!bPass)
                {
                    rewards.Add(new Reward() { id = i, type = type, itemid = itemID, amount = amount });
                }
            }
        }

        public static void TransferLoot()
        {


            Resource resource2 = Resource.FromID(GUIManager.getInstance().rawMatsSortType[0]);
            WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + foodAmount);
            resource2 = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[0]);
            WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + coinAmount);

            for (int i = 0; i < rewards.Count; i++)
            {
                int itemID = rewards.Find(x => x.id == i).itemid;
                int type = rewards.Find(x => x.id == i).type;
                int amount = rewards.Find(x => x.id == i).amount;

                switch (type)
                {
                    case 0:
                        resource2 = Resource.FromID(GUIManager.getInstance().weaponsSortType[itemID]);
                        goto case 4;
                    case 1:
                        resource2 = Resource.FromID(GUIManager.getInstance().armorSortType[itemID]);
                        goto case 4;
                    case 2:
                        resource2 = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[itemID]);
                        goto case 4;
                    case 3:
                        resource2 = Resource.FromID(GUIManager.getInstance().rawMatsSortType[itemID]);
                        goto case 4;
                    case 4:
                        //GUIManager.getInstance().AddTextLine(i + "Count : " + rewards.Count + " : " + type + " : " + itemID + " : " + amount + " : " + resource2.name + " : " + WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                        WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                        //GUIManager.getInstance().AddTextLine(i + "Count : " + rewards.Count + " : " + type + " : " + itemID + " : " + amount + " : " + resource2.name + " : " + WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                        continue;
                }
            }
            rewards.Clear();
        }

        public void LoadBattle()
        {
            AManager<ChunkManager>.getInstance().DeleteChunkData();
            GUIManager.getInstance().selectedBlock = null;
            DestoryAll();
            AManager<WorldManager>.getInstance().settlementName = "Test Dungeon";
            AManager<WorldManager>.getInstance().animalSheep = 0f;
            AManager<WorldManager>.getInstance().animalBoar = 0f;
            AManager<WorldManager>.getInstance().animalChicken = 0f;
            AManager<WorldManager>.getInstance().mountains = 0f;
            AManager<WorldManager>.getInstance().trees = 0.2f;
            int river = UnityEngine.Random.Range(0, 100);
            AManager<WorldManager>.getInstance().river = false;
            AManager<WorldManager>.getInstance().coast = false;
            AManager<WorldManager>.getInstance().CreateNewGame(AManager<WorldManager>.getInstance().settlementName, new Vector3i(2, 48, 2));
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();

            //AManager<WorldManager>.getInstance().SpawnInvasion();
            float num = -650f;
            if(AManager<ResourceManager>.getInstance().getWealth() <= 0)
            {
                num += 50f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
            }
            else
            {
                num += AManager<ResourceManager>.getInstance().getWealth();
            }
            num += 100f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
            num += 50f * (float)AManager<TimeManager>.getInstance().day;
            AManager<WorldManager>.getInstance().SpawnInvasion(Mathf.FloorToInt(num));

            BattleStartMenu.OpenWindow();
        }

        public void LoadHome()
        {
            GUIManager.getInstance().AddTextLine("Fight over");
            AManager<ChunkManager>.getInstance().DeleteChunkData();
            GUIManager.getInstance().selectedBlock = null;
            foreach (ALivingEntity allEntities in UnitManager.getInstance().allUnits)
            {
                allEntities.Destroy();
            }
            //WorldManager.getInstance().LoadGame(PluginMain.File);
            //base.StartCoroutine(this.LoadGame(0.1f, saveFileData.file));
            this.gamesave = GameSave.Create("saves/" + PluginMain.File + ".tass.gz");
            this.gamesave.Load();
            WorldManager.getInstance().enableSaving = true;
            Vector3i worldSize = AManager<ChunkManager>.getInstance().worldSize;
            Vector2i chunkSize = AManager<ChunkManager>.getInstance().chunkSize;
            float voxelSize = AManager<ChunkManager>.getInstance().voxelSize;
            WorldManager.getInstance().controllerObj.GetComponent<ControlPlayer>().worldEdge = WorldManager.getInstance().GetEdgePosition();
            WorldManager.getInstance().pickBox.localScale = new Vector3((float)(chunkSize.x * worldSize.x - 1), (float)(worldSize.y - 1), (float)(chunkSize.z * worldSize.z - 1)) * voxelSize;
            WorldManager.getInstance().bedRock.localScale = new Vector3((float)(chunkSize.x * worldSize.x - 1), 0f, (float)(chunkSize.z * worldSize.z - 1)) * voxelSize * 0.1f;
            WorldManager.getInstance().bedRock.position = new Vector3(0f, (float)worldSize.y * -0.1f - 0.1f, 0f);
            WorldManager.getInstance().topHeight = (float)worldSize.y * 0.1f - 0.1f;
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            AManager<GUIManager>.getInstance().inGame = true;
        }

        public static void SendPartyOnQuest()
        {
            isInTown = false;
            isStartingFight = true;
            PrepBattleField(0);
            BattleManager battlemanager = new BattleManager();
            battlemanager.LoadBattle();
            AManager<WorldManager>.getInstance().enableSaving = false;
            isPlacingUnits = true;
            isPlacingUnits2 = true;
            isFighting = true;
            BattleStartMenu.OpenWindow();
            UnitsToPlace = PartyMenu.PartySize;
        }

        public static void SendPartyBackHome()
        {
            isFighting = false;
            isPlaced = false;
            isBattleOver = false;
            isToTown = false;
            isInTown = true;
            isArrivingTown = true;
            PartyMenu.ManageParty(1);
            BattleManager battleManager = new BattleManager();
            battleManager.LoadHome();
            if (WorldManager.getInstance().settlementName.ToString() != PluginMain.File)
            {
                WorldManager.getInstance().settlementName = PluginMain.File;
            }
            AManager<WorldManager>.getInstance().enableSaving = true;
        }

        public static int GetEnemyRemaining()
        {
            int eNum = 0;

            foreach (ALivingEntity allEntities in UnitManager.getInstance().allUnits)
            {
                if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(allEntities.faction) != Alignment.Ally)
                {
                    if (isFighting && isPlaced)
                    {
                        if (allEntities.isAlive())
                        {
                            eNum++;
                        }
                        
                    }
                }
            }
            GUIManager.getInstance().DrawTextCenteredWhite(new Rect(0f, Screen.height / 10, 200f, 35f), "Enemy Count:" + eNum);
            return eNum;
        }

        public void OnGUI()
        {
            if (isPlacingUnits2)
            {
                PlaceUnits();
            }

            if (isArrivingTown)
            {
                if (isInTown)
                {
                    timesinceload++;
                    if(timesinceload >= 10f)
                    {
                        isArrivingTown = false;
                        PartyMenu.ManageParty(2);
                        TransferLoot();
                        timesinceload = 0;
                    }
                }
            }

            if (isFighting && isPlaced)
            {
                //GUIManager.getInstance().DrawTextCenteredWhite(new Rect(0f, Screen.height / 10, 200f, 35f), "Enemy Count:" + EnemyCount);
                if (GetEnemyRemaining() <= 0)
                {
                    if (!isBattleOver)
                    {
                        Reward();
                        isBattleOver = true;
                    }
                    else
                    {
                        time++;
                        if (time >= 500f)
                        {
                            BattleOverMenu.OpenWindow();
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (isPlacingUnits)
                {
                    if ( UnitsToPlace > 1)
                    {
                        UnitsToPlace--;
                    }
                    else
                    {
                        UnitsToPlace--;
                        isPlacingUnits = false;
                        isPlacingUnits2 = false;
                        isPlaced = true;
                        //StartPosition = CheckPosition();
                        BattleStartMenu.CloseWindow();
                        GUIManager.getInstance().inGame = true;
                    }
                }
            }
        }
    }
}