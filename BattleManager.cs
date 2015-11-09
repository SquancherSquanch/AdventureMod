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
using UnityEngine;
using EventHandler = Timber_and_Stone.API.Event.EventHandler;
using System.Linq;

namespace Plugin.Squancher.TestMod
{
    public class Draftees : IEquatable<Draftees>
    {
        public int UnitId { get; set; }
        public string uName { get; set; }
        public float Health { get; set; }
        public int Experience { get; set; }
        public bool isEnlisted { get; set; }

        public override string ToString()
        {
            return "ID: " + UnitId + "   Name: " + uName + "   Health: " + Health + "   Experience: " + Experience + "   Enlisted: " + isEnlisted;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Draftees objAsPart = obj as Draftees;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return UnitId;
        }
        public bool Equals(Draftees other)
        {
            if (other == null) return false;
            return (this.UnitId.Equals(other.UnitId));
        }
        // Should also override == and != operators.
    }

    public class BattleManager : MonoBehaviour
    {
        public static bool bAdventureMapOpen, isMapCreated, isStartingFight, isFighting, isPlacingUnits, isPlaced, isBattleOver, isFromTown, isToTown, isArrivingTown, isInTown;
        GUIManager guiMgr = GUIManager.getInstance();
        GameSave gamesave;
        private bool reverseSort;
        public static string invasion;
        public APlayableEntity[] warPartyEntity = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));

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
        public float time = 0;
        public bool bReward;

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

        public void CreateNewWorldUnits(Vector3 startPosition)
        {
            Vector3 vector = new Vector3(startPosition.x, 0f, startPosition.z);
            Vector3 zero = Vector3.zero;
            List<int> list = new List<int>();
            List<Vector3> list2 = new List<Vector3>();
            for (int i = 0; i < 8; i++)
            {
                int iD;
                do
                {
                    zero = new Vector3((float)UnityEngine.Random.Range(-5, 6) * 0.2f, 0f, (float)UnityEngine.Random.Range(-5, 6) * 0.2f);
                    Vector3[] array = AManager<ChunkManager>.getInstance().Pick(new Vector3(vector.x + zero.x, 48 * 0.1f - 0.1f, vector.z + zero.z), Vector3.down, false);
                    iD = (int)AManager<ChunkManager>.getInstance().GetBlockOnTop(Coordinate.FromChunkBlock(array[0], array[1])).properties.getID();
                }
                while (iD >= 60 || iD <= 0 || list2.Contains(zero));
                list2.Add(zero);
                bool flag;
                int num;
                do
                {
                    flag = true;
                    num = UnityEngine.Random.Range(1, 14);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (num == list[j])
                        {
                            flag = false;
                        }
                    }
                }
                while (!flag);
                list.Add(num);
                if (num == 1)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 2)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 3)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 4)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 5)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 6)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 7)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 8)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("infantry", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 9)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 10)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 11)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 12)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
                else if (num == 13)
                {
                    AManager<UnitManager>.getInstance().AddHumanUnit("archer", vector + zero, true, false, UnityEngine.Random.Range(0, 3) == 1);
                }
            }
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().MoveToPosition(vector);
            Vector3 eulerAngles = GUIManager.getInstance().controllerObj.eulerAngles;
            eulerAngles.y = (float)UnityEngine.Random.Range(130, 210);
            GUIManager.getInstance().controllerObj.eulerAngles = eulerAngles;
        }

        public Transform AddHumanUnit(APlayableEntity entity, Vector3 pos)
        {
            HumanEntity humanEntity = AManager<AssetManager>.getInstance().InstantiateUnit<HumanEntity>();
            humanEntity.coordinate = Coordinate.FromWorld(pos);

            /* messenger
            humanEntity.faction = AManager<WorldManager>.getInstance().controllerObj.GetComponent<MigrantFactionController>();
            humanEntity.spottedTimer = 600000f;
            humanEntity.interruptTaskIgnoreAlive(new TaskMigrate(humanEntity));
            */

            //humanEntity.faction = AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>();
            humanEntity.faction = AManager<WorldManager>.getInstance().controllerObj.GetComponent<MigrantFactionController>();
            humanEntity.gender = entity.gender;

            List<AProfession> list = new List<AProfession>();
            for (int i = 1; i < entity.getProfessions().Length; i++)
            {
                //list.Add(entity.getProfessions()[i]);
                humanEntity.addProfession(entity.getProfessions()[i]);
            }
            humanEntity.SetProfession(entity.getProfession());

            AProfession profession = humanEntity.getProfession();
            profession.setLevel(profession.getLevel());
            humanEntity.maxHP = 100f;
            humanEntity.hitpoints = 100f;
            humanEntity.unitName = entity.unitName;

            // messenger
            //UnitManager.getInstance().visitors.Add(humanEntity.transform);

            Vector3[] array = AManager<ChunkManager>.getInstance().Pick(new Vector3(pos.x, AManager<WorldManager>.getInstance().topHeight, pos.z), Vector3.down, false);
            humanEntity.transform.position = new Vector3(humanEntity.transform.position.x, AManager<ChunkManager>.getInstance().GetWorldPosition(array[0], array[1]).y + 0.1f, humanEntity.transform.position.z);
            humanEntity.fatigue = UnityEngine.Random.Range(0.7f, 1f);
            //this.RandomUnitTraits(humanEntity);
            humanEntity.coordinate = Coordinate.FromWorld(humanEntity.transform.position);
            Vector3 vect3;
            if (AManager<DesignManager>.getInstance().edgeRoads.Count == 0)
            {
            }
            int num = UnityEngine.Random.Range(0, 4);
            if (num == 0 && !AManager<DesignManager>.getInstance().edgeRoadSouth)
            {
                vect3 = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            }
            if (num == 1 && !AManager<DesignManager>.getInstance().edgeRoadNorth)
            {
                vect3 = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            }
            if (num == 2 && !AManager<DesignManager>.getInstance().edgeRoadEast)
            {
                vect3 = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
                //humanEntity.interruptTaskIgnoreAlive(new TaskForcedMove(humanEntity, vect3, Vector3i block));
            }
            if (num == 3 && !AManager<DesignManager>.getInstance().edgeRoadWest)
            {
                vect3 = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
                HallDesignation hallDesignation = new HallDesignation();
                humanEntity.interruptTaskIgnoreAlive(new TaskForcedMove(humanEntity, (Vector3i)vect3, new Vector3i(hallDesignation.coordinates[0].block.x, hallDesignation.coordinates[0].block.y, hallDesignation.coordinates[0].block.z)));
            }

            return humanEntity.transform;
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
                    //place unit before battle
                    if (trade == 0)
                    {
                        if (PartyMenu.draftees.Count != 0)
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
                        Vector3[] array2 = AManager<ChunkManager>.getInstance().Pick(new Vector3(battleManager.CheckPosition().x + (float)UnityEngine.Random.Range(-3, 3), AManager<WorldManager>.getInstance().topHeight, battleManager.CheckPosition().z + (float)UnityEngine.Random.Range(-3, 3)), Vector3.down, false);
                        aPlayableEntity.transform.position = new Vector3(battleManager.CheckPosition().x + (float)UnityEngine.Random.Range(-1, 1), AManager<ChunkManager>.getInstance().GetWorldPosition(array2[0], array2[1]).y + 0.1f, battleManager.CheckPosition().z + (float)UnityEngine.Random.Range(-1, 1));
                        aPlayableEntity.coordinate = Coordinate.FromWorld(aPlayableEntity.transform.position);
                        isPlaced = true;
                        GUIManager.getInstance().inGame = true;
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

        public void RewardScreen()
        {
            
        }

        public void Reward()
        {
            /*********
                food 4
                bones 6
                feathers 7
                spider silk 8
                coal 9
                animal hide 10 
                animal fur 11
                fat 12
                flax fiber 14
                cotton 15
                scrap metal 18
                tin ore 19
                copper ore 20
                iron ore 21
                silver ore 22
                gold ore 23
                mithril ore 24

                twine 42
                rope 43
                cloth 46
                leather 47
                standard ingot 42
                solid ingot 53
                strong ingot 54
                coin 55


            Resource weapons = Resource.FromID(this.guiMgr.weaponsSortType[1]);
                GUI.DrawTexture(position4, resource5.icon);

            Resource armor = Resource.FromID(this.guiMgr.armorSortType[1]);
            Resource rawMats = Resource.FromID(this.guiMgr.rawMatsSortType[1]);
            Resource pMats = Resource.FromID(this.guiMgr.craftSortType[1]);
            **********/
            
        }

        public void RollOdds()
        {

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
            EnemyCount = GetEnemyRemaining();
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
            //AManager<MapManager>.getInstance().HideMap();
            isFromTown = true;
            isInTown = false;
        }

        public static void SendPartyBackHome()
        {
            //AManager<MapManager>.getInstance().HideMap();
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

        public void RenderWindow(int windowID)
        {
            if (isPlacingUnits)
            {
                Rect location6 = new Rect((float)(Screen.width / 2 - 270), 32f, 580f, 180f);
                GUIManager.getInstance().DrawWindow(location6, "Fight Initiated!", false);
                GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 30f, location6.width - 16f, 110f), "Left click to place party!");
                isStartingFight = false;
            }

            if (bReward)
            {
                AManager<TimeManager>.getInstance().pause();
                Rect location6 = new Rect((float)(Screen.width / 2 - 290), Screen.height / 2, 580f, 180f);
                GUIManager.getInstance().DrawWindow(location6, "Battle Summary", false);
                GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 30f, location6.width - 16f, 110f), "");

                int num = 0;
                int num2 = 0;

                Resource rawMats = Resource.FromID(this.guiMgr.rawMatsSortType[0]);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 133f, Screen.height / 2 + 88f, 24f, 24f), rawMats.icon);
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 - 79f, Screen.height / 2 + 88f, 50f, 24f), "100");

                Resource pMats = Resource.FromID(this.guiMgr.indexesProcessedMats[0]);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 54f, Screen.height / 2 + 88f, 24f, 24f), pMats.icon);
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 - 25f, Screen.height / 2 + 88f, 50f, 24f), "100");
                //Note: this was cut off for methods
                if (invasion == "wolf")
                {
                    //EnemyCount
                    Resource animalRawMats = Resource.FromID(this.guiMgr.rawMatsSortType[9]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 - 36f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                    animalRawMats = Resource.FromID(this.guiMgr.rawMatsSortType[10]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 - 12f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                    animalRawMats = Resource.FromID(this.guiMgr.rawMatsSortType[11]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 + 36f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                }
                else if (invasion == "spider")
                {
                    //EnemyCount
                    Resource animalRawMats = Resource.FromID(this.guiMgr.rawMatsSortType[7]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 - 12, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                }
                else
                {
                    if (bounty1 > 0.8f)
                    {
                        Resource weapons = Resource.FromID(this.guiMgr.weaponsSortType[reward1]);
                        GUI.DrawTexture(new Rect(Screen.width / 2 + 16f, Screen.height / 2 + 88f, 25f, 25f), weapons.icon);
                        GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 + 16f, Screen.height / 2 + 88f, 50f, 24f), "100");
                    }
                    if (bounty2 > 0.8f)
                    {
                        Resource armor = Resource.FromID(this.guiMgr.armorSortType[reward2]);
                        GUI.DrawTexture(new Rect(Screen.width / 2 + 49f, Screen.height / 2 + 88f, 25f, 25f), armor.icon);
                    }
            }

            if (this.guiMgr.DrawButton(new Rect(Screen.width / 2 - 71f, Screen.height / 2 + 138, 142f, 32f), "Ok"))
            {
                if (isToTown == false)
                {
                    isBattleOver = false;
                    isToTown = true;
                    AManager<TimeManager>.getInstance().pause();
                    if (!TransitionScreen.IsOpen())
                    {
                        TransitionScreen.OpenWindow();
                        AdventureMap.OpenWindow();
                        time = 0;
                        bReward = false;
                    }
                }
            }
        }
    }

        public void OnGUI()
        {
            
            if (isFighting && isPlaced)
            {
                if (isArrivingTown)
                {
                    PartyMenu.ManageParty(1);
                    LoadHome();
                    PartyMenu.ManageParty(2);
                }
                else if (GetEnemyRemaining() <= 0 && !isArrivingTown)
                {
                    if (!isBattleOver)
                    {
                        bounty1 = UnityEngine.Random.Range(0.0f, 1.0f);
                        bounty2 = UnityEngine.Random.Range(0.0f, 1.0f);
                        reward1 = (int)UnityEngine.Random.Range(0f, 17f);
                        reward2 = (int)UnityEngine.Random.Range(0f, 20f);
                        isBattleOver = true;
                    }
                    else
                    {
                        time++;
                        if (time >= 500f)
                        {
                            bReward = true;
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
                    PrepBattleField(1);
                    GUIManager.getInstance().inGame = true;
                }
            }
        }
    }
}