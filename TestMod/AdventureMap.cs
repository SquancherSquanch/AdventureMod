
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
    public class UsingDeltaTime : MonoBehaviour
    {
        public static bool isMapOpen, isMapCreated, isStartingFight, isFighting, isGroupMenuOpen;
        private float countdown = 3.0f;
        GUIManager guiMgr = GUIManager.getInstance();
        GameSave gamesave;
        private bool reverseSort;

        public APlayableEntity[] humanEntity = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));

        public enum SortType
        {
            NONE,
            NAME,
            PROFESSION
        }

        public UnitList.SortType sortType;
        private Dictionary<Type, string> professionList = new Dictionary<Type, string>();
        public Type professionSort;

        private UsingDeltaTime()
        {
            this.professionList.Add(typeof(Fisherman), "Fisherman");
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

        public void OnGUI()
        {

            if (guiMgr.unitList.isOpen())
            {
                float num3 = 245f;
                float num4 = 55f;
                Rect location6 = new Rect(num3, num4, 75f, 25f);
                this.guiMgr.DrawTextLeftWhite(location6, "Jump");
            }

            if (isGroupMenuOpen)
            {

            }

            if (isMapOpen)
            {
                Screen.showCursor = false;
                Screen.lockCursor = true;
            }

            if (isStartingFight)
            {
                Rect location4 = new Rect((float)Screen.width * 0.15f - 1f, (float)Screen.height * 0.15f - 23f, (float)Screen.width * 0.7f + 2f, (float)Screen.height * 0.7f + 25f);
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(location4.xMin, location4.yMax, location4.width - 36f, 24f), "Ambushed!");
                //GUIManager.getInstance().DrawWindow(location4, "World Map - Choose Starting Location", true);
                countdown -= Time.deltaTime;
                if (countdown <= 0.0f)
                {
                    AManager<MapManager>.getInstance().HideMap();
                    AManager<ChunkManager>.getInstance().DeleteChunkData();
                    GUIManager.getInstance().selectedBlock = null;
                    AManager<WorldManager>.getInstance().settlementName = "Test Dungeon";
                    AManager<WorldManager>.getInstance().CreateNewGame(AManager<WorldManager>.getInstance().settlementName, new Vector3i(1, 48, 1));
                    GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
                    GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
                    GUIManager.getInstance().inStartMenu = false;
                    GUIManager.getInstance().inGame = true;
                    checkUnits();
                    //CreateNewWorldUnits(new Vector3(1, 6, 1));
                    AManager<WorldManager>.getInstance().SpawnInvasion();
                }
                //GUIManager.getInstance().DrawTextCenteredWhite(new Rect(location4.xMin, location4.yMax - 32f, location4.width - 36f, 24f), "Ambushed!");
            }

            if (isFighting)
            {
                Rect location6 = new Rect((float)(Screen.width / 2 - 270), 32f, 580f, 180f);
                GUIManager.getInstance().DrawWindow(location6, "Fight Initiated!", false);
                GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 30f, location6.width - 16f, 110f), "Choose Location to prepare for attack!");
                isStartingFight = false;
            }
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
            humanEntity.faction = AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>();
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
            humanEntity.unitName = entity.name;
            Vector3[] array = AManager<ChunkManager>.getInstance().Pick(new Vector3(pos.x, AManager<WorldManager>.getInstance().topHeight, pos.z), Vector3.down, false);
            humanEntity.transform.position = new Vector3(humanEntity.transform.position.x, AManager<ChunkManager>.getInstance().GetWorldPosition(array[0], array[1]).y + 0.1f, humanEntity.transform.position.z);
            humanEntity.fatigue = UnityEngine.Random.Range(0.7f, 1f);
            //this.RandomUnitTraits(humanEntity);
            humanEntity.coordinate = Coordinate.FromWorld(humanEntity.transform.position);
            entity.Destroy();
            return humanEntity.transform;
        }

        public void checkUnits()
        {
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            this.SortUnits(array);
            //GUIManager.getInstance().AddTextLine(array.Length.ToString());
            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                if (aPlayableEntity.isAlive())
                {
                    GUIManager.getInstance().AddTextLine(aPlayableEntity.unitName);
                    GUIManager.getInstance().AddTextLine(aPlayableEntity.getProfession().getProfessionName());

                    if (aPlayableEntity.getProfession().getProfessionName() == "Infantry")
                    {
                        AddHumanUnit(aPlayableEntity, new Vector3(1, 6, 1));
                    }

                    if (aPlayableEntity.getProfession().getProfessionName() == "Archer")
                    {

                    }

                    /*if (aPlayableEntity.getProfession().getProfessionName() != "Infantry" )
                        if (aPlayableEntity.getProfession().getProfessionName() != "Archer")
                        {
                            aPlayableEntity.Destroy();
                        }*/
                }
            }
        }
        public void Update()
        {
            /*
            AManager<WorldManager>.getInstance().settlementName = "Test Dungeon";
            AManager<WorldManager>.getInstance().CreateNewGame(AManager<WorldManager>.getInstance().settlementName, new Vector3i(4, 48, 4));
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().SwitchCamera();
            */



            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (isMapOpen)
                {
                    AManager<MapManager>.getInstance().HideMap();
                    isMapOpen = !isMapOpen;
                    GUIManager.getInstance().inGame = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                GUIManager.getInstance().AddTextLine("Trying!");
                //checkUnits();
                //CreateNewWorldUnits(new Vector3(1, 6, 1));
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                //isMapCreated = (isMapCreated == false) ? true : false;
                //WorldManager.getInstance().LoadGame("Durn.tass.gz");
                if (!isMapOpen)
                {

                    //checkUnits();
                    GUIManager.getInstance().inGame = false;
                    AManager<WorldManager>.getInstance().mapSize = "Large";
                    if (!isMapCreated)
                    {
                        AManager<MapManager>.getInstance().CreateWorldMap();
                        AManager<MapManager>.getInstance().ShowMap(0.15f, 0.15f, 0.7f, 0.7f, true);
                        AManager<MapManager>.getInstance().View3D();
                        isMapCreated = true;
                        isMapOpen = true;
                    }
                    if (isMapCreated)
                    {
                        AManager<MapManager>.getInstance().ShowMap(0.15f, 0.15f, 0.7f, 0.7f, true);
                        AManager<MapManager>.getInstance().View3D();
                        isMapOpen = true;
                    }
                    //Screen.showCursor = false;
                }
                if (isMapOpen)
                {
                    isStartingFight = true;
                    //GUIManager.getInstance().startMenu = "New6";
                }

                //GUIManager.getInstance().unitList.OpenWindow();
            }
        }
    }

    public class PluginMain : CSharpPlugin, IEventListener
    {
        WorldManager worldManager;
        GameSave gamesave;

        public override void OnLoad()
        {
            worldManager = WorldManager.getInstance();
            GUIManager.getInstance().AddTextLine("Adventure Mod Loaded");
            GUIManager.getInstance().gameObject.AddComponent(typeof(UsingDeltaTime));
        }

        public override void OnEnable()
        {
            GUIManager.getInstance().AddTextLine("Adventure Mod Enabled");
            EventManager.getInstance().Register(this);
            GUIManager.getInstance().AddTextLine("Adventure Mod Registered Events");
        }

        [EventHandler(Priority.Normal)]
        public void onInvasionNormal(EventInvasion evt)
        {
            //evt.result = Result.Deny;
        }

        [EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            //if (evt.result != Result.Deny) return;
            if (evt.invasion.getName() == "wolf" || evt.invasion.getName() == "spider" || evt.invasion.getName() == "skeleton" || evt.invasion.getName() == "necromancer")
            {
                GUIManager.getInstance().AddTextLine("A " + evt.invasion.getName() + " has found you!");
            }
            if (evt.invasion.getName() == "wolves" || evt.invasion.getName() == "spiders" || evt.invasion.getName() == "skeletons")
            {
                GUIManager.getInstance().AddTextLine("Some " + evt.invasion.getName() + " have found you!");
            }

            GUIManager.getInstance().AddTextLine(evt.invasion.getName());
        }

        [EventHandler(Priority.Normal)]
        public void onGameLoad(EventGameLoad evt)
        {
            if (UsingDeltaTime.isStartingFight)
            {
                Screen.lockCursor = false;
                Screen.showCursor = true;
                UsingDeltaTime.isFighting = true;
            }

        }
    }
}
