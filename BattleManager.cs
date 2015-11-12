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
        public static int time;
        public static bool bAdventureMapOpen, bReward, isMapCreated, isStartingFight, isFighting, isPlacingUnits, isPlaced, isBattleOver, isFromTown, isToTown, isArrivingTown, isInTown;
        GUIManager guiMgr = GUIManager.getInstance();
        GameSave gamesave;
        private bool reverseSort;
        public static string invasion;
        public static List<Reward> rewards = new List<Reward>();
        public static int bonusLoot, foodAmount, coinAmount;
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
        public int reward1, reward2 , EnemyCount;
        public Vector3 StartPosition;
        

        private BattleManager()
        {
        }

        public void Start()
        {
            isInTown = true;
            bReward = false;
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
                                    Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(1, AManager<WorldManager>.getInstance().topHeight, 1), Vector3.down, false);
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

                    //place units on click
                    if (trade == 1)
                    {
                        if (PartyMenu.draftees.Exists(x => x.uName == aPlayableEntity.unitName))
                        {

                            if (i == 0)
                            {
                                Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(battleManager.StartPosition.x, AManager<WorldManager>.getInstance().topHeight, battleManager.StartPosition.z), Vector3.down, false);
                                aPlayableEntity.transform.position = new Vector3(battleManager.StartPosition.x, AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], array2[1]).y + 0.1f, battleManager.StartPosition.z);
                            }
                            if ( i==1 || i==2 )
                            { 
                                Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(battleManager.StartPosition.x + (float)UnityEngine.Random.Range(-i/2, i / 2), AManager<WorldManager>.getInstance().topHeight, battleManager.StartPosition.z + (float)UnityEngine.Random.Range(-i / 2, i / 2)), Vector3.down, false);
                                aPlayableEntity.transform.position = new Vector3(battleManager.StartPosition.x - i / 2, AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], array2[1]).y + 0.1f, battleManager.StartPosition.z);
                            }
                            if ( i == 3)
                            {
                                Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(battleManager.StartPosition.x, AManager<WorldManager>.getInstance().topHeight, battleManager.StartPosition.z), Vector3.down, false);
                                aPlayableEntity.transform.position = new Vector3(battleManager.StartPosition.x, AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], array2[1]).y + 0.1f, battleManager.StartPosition.z - 0.5f);
                            }
                            if( i == 4 || i == 5)
                            {
                                Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(battleManager.StartPosition.x + (float)UnityEngine.Random.Range(-i / 2, i / 2), AManager<WorldManager>.getInstance().topHeight, battleManager.StartPosition.z + (float)UnityEngine.Random.Range(-i / 2, i / 2)), Vector3.down, false);
                                aPlayableEntity.transform.position = new Vector3(battleManager.StartPosition.x - i / 2, AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], array2[1]).y + 0.1f, battleManager.StartPosition.z - 0.5f);
                            }
                            aPlayableEntity.coordinate = Coordinate.FromWorld(aPlayableEntity.transform.position);
                            isPlaced = true;
                            battleManager = new BattleManager();
                            battleManager.EnemyCount = GetEnemyRemaining();
                            GUIManager.getInstance().inGame = true;
                        }
                    }
                }
            }
            return trade;
        }

        // StartSelector
        public Vector3 CheckPosition()
        {
            RaycastHit raycastHit;
            Vector3[] array;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                if (raycastHit.transform.tag == "PickPlane")
                {
                    Vector3 normalized = (raycastHit.point - Camera.main.transform.position).normalized;
                    array = AManager<ChunkManager>.getInstance().Pick(raycastHit.point, normalized, true);
                }
                else
                {
                    array = AManager<ChunkManager>.getInstance().Pick(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, true);
                    if (array == null)
                    {
                        return new Vector3(0, 0, 0);
                    }
                }
            }
            else
            {
                array = AManager<ChunkManager>.getInstance().Pick(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, true);
                if (array == null)
                {
                    return new Vector3(0, 0, 0);
                }
            }
            Vector3 a = new Vector3(array[1].x, array[1].y, array[1].z);
            Vector3 vector = a + array[2];
            Vector3 vector2 = AManager<ChunkManager>.getInstance().GetWorldPosition(array[0], vector);
            return vector2;
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

            foodAmount = UnityEngine.Random.Range(10,75);
            coinAmount = UnityEngine.Random.Range(5,30);

            for (int i = 0; i < bonusLoot; i++)
            {
                int lootTableRoll = UnityEngine.Random.Range(1, 100);
                int type, itemID, amount;
                //weapon armor
                if (lootTableRoll < 20)
                {
                    type = UnityEngine.Random.Range(0, 4);
                    amount = 1;
                    //weapon
                    if (type == 0)
                    {
                        itemID = UnityEngine.Random.Range(0, 16);

                        if (rewards.Exists(x => x.itemid == itemID))
                        {
                            rewards.Find(x => x.itemid == itemID).amount += amount;
                            continue;
                        }
                        rewards.Add(new Reward() { id = i, type = 0, itemid = itemID, amount = amount });
                    }
                    //armor
                    if (type == 1)
                    {
                        itemID = UnityEngine.Random.Range(0, 20);
                        if (rewards.Exists(x => x.itemid == itemID))
                        {
                            rewards.Find(x => x.itemid == itemID).amount += amount;
                            continue;
                        }
                        rewards.Add(new Reward() { id = i, type = 1, itemid = itemID, amount = amount });
                    }
                    //mithril
                    if (type == 2)
                    {
                        if (lootTableRoll < 50)
                        {
                            itemID = 22;
                            amount = UnityEngine.Random.Range(1, 6);
                            if (rewards.Exists(x => x.itemid == itemID))
                            {
                                rewards.Find(x => x.itemid == itemID).amount += amount;
                                continue;
                            }
                            rewards.Add(new Reward() { id = i, type = 2, itemid = itemID, amount = amount });
                        }

                        amount = UnityEngine.Random.Range(1,4);
                        if (lootTableRoll > 90)
                        {
                            itemID = 11;
                            type = 2;
                        }
                        else
                        {
                            itemID = 23;
                            type = 3;
                        }
                        if (rewards.Exists(x => x.itemid == itemID))
                        {
                            rewards.Find(x => x.itemid == itemID).amount += amount;
                            continue;
                        }
                        rewards.Add(new Reward() { id = i, type = type, itemid = itemID, amount = amount });
                    }
                    //gold
                    if (type == 3)
                    {
                        itemID = 22;
                        amount = UnityEngine.Random.Range(1, 6);
                        if (rewards.Exists(x => x.itemid == itemID))
                        {
                            rewards.Find(x => x.itemid == itemID).amount += amount;
                            continue;
                        }
                        rewards.Add(new Reward() { id = i, type = 3, itemid = itemID, amount = amount });
                    }
                    //silver
                    if (type == 4)
                    {
                        itemID = 21;
                        amount = UnityEngine.Random.Range(1, 8);
                        if (rewards.Exists(x => x.itemid == itemID))
                        {
                            rewards.Find(x => x.itemid == itemID).amount += amount;
                            continue;
                        }
                        rewards.Add(new Reward() { id = i, type = 3, itemid = itemID, amount = amount });
                    }
                }

                //processed mats
                if (lootTableRoll > 19 && lootTableRoll < 50)
                {
                    itemID = UnityEngine.Random.Range(0, 3); //7-10

                    if (itemID == 0) { itemID = 7; }
                    if (itemID == 1) { itemID = 8; }
                    if (itemID == 2) { itemID = 9; }
                    if (itemID == 3) { itemID = 10; }

                    amount = UnityEngine.Random.Range(1, 16);
                    if (rewards.Exists(x => x.itemid == itemID))
                    {
                        rewards.Find(x => x.itemid == itemID).amount += amount;
                        continue;
                    }
                        rewards.Add(new Reward() { id = i, type = 2, itemid = itemID, amount = amount });
                }
                //raw mats
                if (lootTableRoll > 49)
                {
                    itemID = UnityEngine.Random.Range(0, 12); //6-10 4, 12 - 15 3, 17-20 3

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
                    if (itemID == 12) { itemID = 20; }
                    amount = UnityEngine.Random.Range(1, 16);
                    if (rewards.Exists(x => x.itemid == itemID))
                    {
                        rewards.Find(x => x.itemid == itemID).amount += amount;
                        continue;
                    }
                    rewards.Add(new Reward() { id = i, type = 3, itemid = itemID, amount = amount });
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

                if (type == 0)
                {
                    resource2 = Resource.FromID(GUIManager.getInstance().weaponsSortType[itemID]);
                    WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                }
                if (type == 1)
                {
                    resource2 = Resource.FromID(GUIManager.getInstance().armorSortType[itemID]);
                    WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                }
                if (type == 2)
                {
                    resource2 = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[itemID]);
                    WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                }
                if (type == 3)
                {
                    resource2 = Resource.FromID(GUIManager.getInstance().rawMatsSortType[itemID]);
                    WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                }
                //WorldManager.getInstance().PlayerFaction.storage.setResource(resource2, WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
                //GUIManager.getInstance().AddTextLine("Count : "+ rewards.Count +" : " + type + " : " + itemID + " : " + amount + " : " + resource2.name + " : " + WorldManager.getInstance().PlayerFaction.storage.getResource(resource2) + amount);
            }
            rewards.Clear();
        }

        public void LoadBattle()
        {
            isFighting = true;
            AManager<ChunkManager>.getInstance().DeleteChunkData();
            GUIManager.getInstance().selectedBlock = null;
            DestoryAll();
            AManager<WorldManager>.getInstance().settlementName = "Test Dungeon";
            AManager<WorldManager>.getInstance().animalSheep = 0f;
            AManager<WorldManager>.getInstance().animalBoar = 0f;
            AManager<WorldManager>.getInstance().animalChicken = 0f;
            AManager<WorldManager>.getInstance().mountains = 0f;
            AManager<WorldManager>.getInstance().trees = 0.2f;
            AManager<WorldManager>.getInstance().river = false;
            AManager<WorldManager>.getInstance().coast = false;
            AManager<WorldManager>.getInstance().CreateNewGame(AManager<WorldManager>.getInstance().settlementName, new Vector3i(2, 48, 2));
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            AManager<WorldManager>.getInstance().SpawnInvasion();
        }

        public void LoadHome()
        {
            GUIManager.getInstance().AddTextLine("Fight over");
            isFighting = false;
            isPlaced = false;
            isArrivingTown = false;
            isInTown = true;
            AManager<ChunkManager>.getInstance().DeleteChunkData();
            GUIManager.getInstance().selectedBlock = null;
            foreach (ALivingEntity allEntities in UnitManager.getInstance().allUnits)
            {
                allEntities.Destroy();
            }

            //WorldManager.getInstance().LoadGame(PluginMain.File);
            //base.StartCoroutine(this.LoadGame(0.1f, saveFileData.file));
            this.gamesave = GameSave.Create("saves/" + PluginMain.File);
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
            isFromTown = true;
            isInTown = false;
        }

        public static void SendPartyBackHome()
        {
            isArrivingTown = true;
            isToTown = false;
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

            if (isPlacingUnits)
            {
                BattleStartMenu.OpenWindow();
            }

            if (isFighting && isPlaced)
            {
                if (isArrivingTown)
                {
                    PartyMenu.ManageParty(1);
                    LoadHome();
                    PartyMenu.ManageParty(2);
                    TransferLoot();
                    AManager<WorldManager>.getInstance().enableSaving = true;
                }
                else if (GetEnemyRemaining() <= 0 && !isArrivingTown)
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

            if (isFromTown)
            {
                PrepBattleField(0);
                isStartingFight = true;
                isFromTown = false;
                GUIManager.getInstance().inStartMenu = true;
            }

            if (isStartingFight)
            {
                LoadBattle();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (isPlacingUnits)
                {
                    isPlacingUnits = false;
                    StartPosition = CheckPosition();
                    PrepBattleField(1);
                    BattleStartMenu.CloseWindow();
                    GUIManager.getInstance().inGame = true;
                }
            }
        }
    }
}