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
        public static int time, timesinceload, tick;
        public static bool bAdventureMapOpen, isMapCreated, isStartingFight, isFighting, isPlacingUnits, isPlacingUnits2, isPlaced, isBattleAi, isBattleOver, isFromTown, isToTown, isArrivingTown, isInTown;
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
        public static APlayableEntity enemiesTarget;

        private BattleManager()
        {
        }

        public void Start()
        {
            isInTown = true;
        }

        public static void DestoryAll()
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

        public static void PrepBattleField()
        {
            PartyManager partyManager = new PartyManager();
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            partyManager.SortUnits(array);

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                if (aPlayableEntity.isAlive())
                {
                    if (PartyManager.draftees.Count != 0)
                    {
                        //place unit before battle
                        if (PartyManager.draftees.Contains(new Draftees { uName = aPlayableEntity.unitName }))
                        {
                            if (PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted)
                            {
                                aPlayableEntity.transform.position = new Vector3(0, 100, 0);
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

        public static void RecantPlacedUnits()
        {
            PartyManager partyManager = new PartyManager();
            if (UnitsToPlace >= 0 && UnitsToPlace < PartyManager.PartySize)
            {
                
                APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
                BattleManager battleManager = new BattleManager();
                partyManager.SortUnits(array);
                if (UnitsToPlace == 0)
                {
                    UnitsToPlace++;
                    array[UnitsToPlace - 1].transform.position = new Vector3(0, 100, 0);
                    array[UnitsToPlace - 1].coordinate = Coordinate.FromWorld(array[UnitsToPlace].transform.position);
                }
                else
                {
                    array[UnitsToPlace - 1].transform.position = new Vector3(0, 100, 0);
                    array[UnitsToPlace - 1].coordinate = Coordinate.FromWorld(array[UnitsToPlace - 1].transform.position);
                    UnitsToPlace++;
                }
                
            }
        }

        public static void PlaceUnits()
        {
            PartyManager partyManager = new PartyManager();
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            partyManager.SortUnits(array);

            RaycastHit raycastHit;
            Vector3[] array2;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                if (raycastHit.transform.tag == "PickPlane")
                {
                    Vector3 normalized = (raycastHit.point - Camera.main.transform.position).normalized;
                    array2 = AManager<ChunkManager>.getInstance().Pick(raycastHit.point, normalized, true);
                }
                else
                {
                    array2 = AManager<ChunkManager>.getInstance().Pick(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, true);
                    if (array == null)
                    {
                        return;
                    }
                }
            }
            else
            {
                array2 = AManager<ChunkManager>.getInstance().Pick(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, true);
                if (array == null)
                {
                    return;
                }
            }
            Vector3 a = new Vector3(array2[1].x, array2[1].y, array2[1].z);
            Vector3 vector = a + array2[2];
            Vector3 vector2 = AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], vector);
            vector2 += new Vector3(0f, -0.2f / 2f, 0f);

            array[UnitsToPlace-1].transform.position = vector2;
            array[UnitsToPlace-1].coordinate = Coordinate.FromWorld(array[UnitsToPlace-1].transform.position);
        }

        public static void RotateUnit(int i)
        {
            PartyManager partyManager = new PartyManager();
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            partyManager.SortUnits(array);
            StartPosition = GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().WorldPositionAtMouse();
            Vector3 eulerAngles = WorldManager.getInstance().controllerObj.eulerAngles;
            eulerAngles.y = i == 0 ? 45f : -45f;
            array[UnitsToPlace - 1].transform.Rotate(eulerAngles);
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
            int river = UnityEngine.Random.Range(0, 100);
            float forest = UnityEngine.Random.Range(0f, 0.5f);
            AManager<WorldManager>.getInstance().settlementName = "Away on Battle";
            AManager<WorldManager>.getInstance().animalSheep = 0f;
            AManager<WorldManager>.getInstance().animalBoar = 0f;
            AManager<WorldManager>.getInstance().animalChicken = 0f;
            float mountain = UnityEngine.Random.Range(0f, 0.1f);
            AManager<WorldManager>.getInstance().mountains = river <= 50 ? 0f + mountain : 0.05f;
            AManager<WorldManager>.getInstance().trees = 0.2f + forest;
            AManager<WorldManager>.getInstance().river = river <= 50 ? false : true ;
            AManager<WorldManager>.getInstance().coast = false;
            AManager<WorldManager>.getInstance().CreateNewGame(AManager<WorldManager>.getInstance().settlementName, new Vector3i(2, 48, 2));
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();

            //AManager<WorldManager>.getInstance().SpawnInvasion();
            float num = -650f;
            if(AManager<ResourceManager>.getInstance().getWealth() <= 0)
            {
                num += 75f * (float)AManager<WorldManager>.getInstance().PlayerFaction.LiveUnitCount();
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
            //WorldManager.getInstance().LoadGame(PluginMain.File + ".tass.gz");
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
            //GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            //GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            AManager<GUIManager>.getInstance().inGame = true;
            QuestManager.isOnQuest = false;
            QuestManager.quest.Clear();
        }

        public static void SendPartyOnQuest()
        {
            isInTown = false;
            isStartingFight = true;
            PrepBattleField();
            BattleManager battlemanager = new BattleManager();
            battlemanager.LoadBattle();
            AManager<WorldManager>.getInstance().enableSaving = false;
            isPlacingUnits = true;
            isPlacingUnits2 = true;
            isFighting = true;
            BattleStartMenu.OpenWindow();
            UnitsToPlace = PartyManager.PartySize;
        }

        public static void SendPartyBackHome()
        {
            PartyManager partyManager = new PartyManager();
            isFighting = false;
            isPlaced = false;
            isBattleAi = false;
            isBattleOver = false;
            isToTown = false;
            isInTown = true;
            isArrivingTown = true;
            partyManager.ManageParty(1);
            BattleManager battleManager = new BattleManager();
            battleManager.LoadHome();
            if (WorldManager.getInstance().settlementName == "Away on Battle")
            {
                WorldManager.getInstance().settlementName = PluginMain.File;
            }
            AManager<WorldManager>.getInstance().enableSaving = true;
        }

        public static int GetEnemyRemaining()
        {
            int eNum = 0;

            foreach (ALivingEntity entity in (ALivingEntity[])FindObjectsOfType(typeof(ALivingEntity)))
            {
                if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(entity.faction) == Alignment.Enemy)
                {
                    if (isFighting && isPlaced)
                    {
                        if (entity.isAlive())
                        {
                            eNum++;
                        }
                        
                    }
                }
            }
            EnemyCount = eNum;
            //GUIManager.getInstance().DrawTextCenteredWhite(new Rect(0f, Screen.height / 10, 200f, 35f), "Enemy Count:" + eNum);
            return eNum;
        }

        public static void GetEnemyTarget(ALivingEntity otherEntity = null)
        {
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            PartyManager.getInstance().SortUnits(array);
            int i = UnityEngine.Random.Range(0, array.Length - 1);

            if (otherEntity != null)
            {
                otherEntity.interruptTask(new TaskAttack(otherEntity, array[i].transform));

            }
            foreach (ALivingEntity entity in UnitManager.getInstance().allUnits)
            {
                if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(entity.faction) != Alignment.Ally)
                {
                    if (entity.getWhatImDoing().Contains("Waiting"))
                    {
                        entity.interruptTask(new TaskAttack(entity, array[i].transform));
                    }
                }
            }
        }


        public void OnGUI()
        {
            if (isArrivingTown)
            {
                if (isInTown)
                {
                    timesinceload++;
                    if (timesinceload >= 10f)
                    {
                        isArrivingTown = false;
                        PartyManager.getInstance().ManageParty(2);
                        TransferLoot();
                        timesinceload = 0;
                    }
                }
            }

            if (isFighting && isPlaced)
            {
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(0f, Screen.height / 10, 200f, 35f), "Enemy Count:" + EnemyCount);
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

                //if (!isBattleAi)
                //{
                    tick++;
                    if (tick >= 2000f)
                    {
                        //GUIManager.getInstance().AddTextLine("Getting Targets");
                        GetEnemyTarget();
                        isBattleAi = true;
                        tick = 0;
                    }
                //}
            }
        }

        public void Update()
        {
            if (!isFighting)
            {
                return;
            }

            if (isPlacingUnits2)
            {
                if (UnitsToPlace > 0)
                {
                    PlaceUnits();
                }
                GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().DeSelect();
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                if(isPlacingUnits)
                {
                    RecantPlacedUnits();
                }
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                if (isPlacingUnits)
                {
                    RotateUnit(1);
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                if (isPlacingUnits)
                {
                    RotateUnit(0);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (isPlacingUnits)
                {
                    if (UnitsToPlace > 0)
                    {
                        UnitsToPlace--;
                    }
                }
            }
        }
    }
}