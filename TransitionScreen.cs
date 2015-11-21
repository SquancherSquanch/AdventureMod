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
    public class BattleStartMenu : MonoBehaviour
    {
        public static bool _open;
        public Rect windowRect;
        private float intendedWindowWidth = 180f;

        private BattleStartMenu()
        {

        }
         
        public static void OpenWindow()
        {
            _open = true;
        }

        public static void CloseWindow()
        {
            _open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }

        public void RenderWindow(int windowID)
        {
            Rect location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }
            Rect location6 = new Rect((float)(Screen.width / 2 - 270), 32f, 580f, 180f);
            GUIManager.getInstance().DrawWindow(location6, "Fight Initiated!", false);
            //GUIManager.getInstance().DrawWindow(location6, "" + GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>().WorldPositionAtMouse(), false);
            GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 30f, location6.width - 16f, 110f), "Left click to place a unit.");
            GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 60f, location6.width - 16f, 110f), "E/Q to rotate unit.");
            GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location6.xMin + 8f, location6.yMin + 90f, location6.width - 16f, 110f), "TAB to replace unit.");
            if (BattleManager.UnitsToPlace == 0)
            {
                if (GUIManager.getInstance().DrawButton(new Rect(Screen.width / 2 - 100, location6.yMin + 130f, 200f, 34f), "Ready"))
                {
                    BattleManager.isPlacingUnits = false;
                    BattleManager.isPlacingUnits2 = false;
                    BattleManager.isPlaced = true;
                    GUIManager.getInstance().inGame = true;
                    BattleManager.GetEnemyTarget();
                    BattleManager.GetEnemyRemaining();
                    CloseWindow();
                    /************
                    * BattleGui here! when initiated take the inGame back in all functions
                    * Make a in battle !inGame comparison.
                    ************/
                    
                }
            }
        }

        public void OnGUI()
        {
            if (GUIManager.getInstance().inGame)
            {
                return;
            }
            if (!IsOpen())
            {
                return;
            }
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect windowRect = GUI.Window(193, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(193);
        }

        public void Update()
        {
        }
    }

    public class BattleOverMenu : MonoBehaviour
    {
        public static bool _open;
        public static string invasion;
        public Rect windowRect;
        private float intendedWindowWidth = 200f;

        private BattleOverMenu()
        {

        }

        public void Start()
        {
            
        }

        public static void OpenWindow()
        {
            _open = true;
        }

        public static void CloseWindow()
        {
            _open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }

        public void RenderWindow(int windowID)
        {
            Rect location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }

            AManager<TimeManager>.getInstance().pause();
            location = new Rect((Screen.width / 2 - 240f), Screen.height / 2 - 90f, 480f, 200f);
            GUIManager.getInstance().DrawWindow(location, "Battle Summary", false);
            GUIManager.getInstance().DrawTextCenteredBlack(new Rect(location.xMin + 8f, location.yMin + 30f, location.width - 16f, 110f), "You found!");

            int num, num2;

            Resource rawMats = Resource.FromID(GUIManager.getInstance().rawMatsSortType[0]);
            Resource pMats = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[0]);

            Rect location2 = new Rect(location.x + 120f, location.y + 58f, 24f, 24f);
            GUI.DrawTexture(new Rect(location2.x, location2.y, 24f, 24f), rawMats.icon);
            GUIManager.getInstance().DrawTextCenteredWhite(new Rect(location2.x + 25f, location2.y, 50f, 24f), BattleManager.foodAmount + "");

            Rect location3 = new Rect(location2.x, location2.y + 24f, 24f, 24f);
            GUI.DrawTexture(new Rect(location3.x, location3.y, 24f, 24f), pMats.icon);
            GUIManager.getInstance().DrawTextCenteredWhite(new Rect(location3.x + 25f, location3.y, 50f, 24f), BattleManager.coinAmount + "");

            Resource resource = Resource.FromID(GUIManager.getInstance().rawMatsSortType[0]);

            if (GUIManager.getInstance().DrawButton(new Rect(Screen.width/2 - 71f, location.y + 162f, 142f, 32f), "Ok"))
            {
                if (BattleManager.isToTown == false)
                {
                    BattleManager.isBattleOver = false;
                    BattleManager.isToTown = true;
                    AManager<TimeManager>.getInstance().pause();
                    if (!TransitionScreen.IsOpen())
                    {
                        CloseWindow();
                        TransitionScreen.OpenWindow();
                        AdventureMap.OpenWindow();
                        BattleManager.time = 0;
                    }
                }

            }

            num = 120; num2 = 82;
            num2 += 24;
            for (int i = 0; i < BattleManager.rewards.Count; i++)
            {
                int type = BattleManager.rewards.Find(x => x.id == i).type;
                int itemID = BattleManager.rewards.Find(x => x.id == i).itemid;
                int amount = BattleManager.rewards.Find(x => x.id == i).amount;

                if (i == 1)
                {
                    num = 200;
                    num2 = 58;
                }
                if (i == 4)
                {
                    num = 280;
                    num2 = 58;
                }
                    
                if (type == 0)
                {
                    resource = Resource.FromID(GUIManager.getInstance().weaponsSortType[itemID]);
                }
                if (type == 1)
                {
                    resource = Resource.FromID(GUIManager.getInstance().armorSortType[itemID]);
                }
                if (type == 2)
                {
                    resource = Resource.FromID(GUIManager.getInstance().indexesProcessedMats[itemID]);
                }
                if (type == 3)
                {
                    resource = Resource.FromID(GUIManager.getInstance().rawMatsSortType[itemID]);
                }
                location2 = new Rect(location.x + num, location.y + num2, 24f, 24f);
                GUI.DrawTexture(new Rect(location2.x, location2.y, 24f, 24f), resource.icon);
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(location2.x + 25f, location2.y, 50f, 24f), "" + amount);
                    
                    
                    
                    num2 += 24;
            }
            //Note: implement by invasion type or unit type loot list....
            /*if (invasion == "wolf")
            {
                //EnemyCount
                Resource animalRawMats = Resource.FromID(GUIManager.getInstance().rawMatsSortType[9]);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 36f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                animalRawMats = Resource.FromID(GUIManager.getInstance().rawMatsSortType[10]);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 12f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

                animalRawMats = Resource.FromID(GUIManager.getInstance().rawMatsSortType[11]);
                GUI.DrawTexture(new Rect(Screen.width / 2 + 36f, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

            }
            else if (invasion == "spider")
            {
                //EnemyCount
                Resource animalRawMats = Resource.FromID(GUIManager.getInstance().rawMatsSortType[7]);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 12, Screen.height / 2 + 55f, 24f, 24f), animalRawMats.icon);

            }
            else
            {
                if (bounty1 > 0.8f)
                {
                    Resource weapons = Resource.FromID(GUIManager.getInstance().weaponsSortType[reward1]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 + 16f, Screen.height / 2 + 88f, 25f, 25f), weapons.icon);
                    GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 + 16f, Screen.height / 2 + 88f, 50f, 24f), "100");
                }
                if (BattleManager.bounty2 > 0.8f)
                {
                    Resource armor = Resource.FromID(GUIManager.getInstance().armorSortType[reward2]);
                    GUI.DrawTexture(new Rect(Screen.width / 2 + 49f, Screen.height / 2 + 88f, 25f, 25f), armor.icon);
                }
            }*/
        }

        public void OnGUI()
        {
            if (!GUIManager.getInstance().inGame)
            {
                return;
            }
            if (!IsOpen())
            {
                return;
            }
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect windowRect = GUI.Window(194, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(194);
        }

        public void Update()
        {

        }
    }

    public class TransitionScreen : MonoBehaviour
    {
        public static bool bOpen;
        GUIManager guiMgr;
        private ControlPlayer controller;
        
        public Rect windowRect;
        private float intendedWindowWidth = Screen.width;
        //private Vector2 horizontalScrollPosition;
        public Rect windowViewRect;
        public static float countdown;

        private TransitionScreen()
        {
        }

        public void Start()
        {
            guiMgr = AManager<GUIManager>.getInstance();
            windowRect = new Rect(0f, 0f, Screen.width, Screen.height);
            windowViewRect = new Rect(0f, 0f, Screen.width, Screen.height);
            this.controller = GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>();
            countdown = 0f;
        }
        
        public static void OpenWindow()
        {
            countdown = 0;
            bOpen = true;
            GUIManager.getInstance().inGame = false;
        }

        public static void CloseWindow()
        {
            bOpen = false;
        }

        public static bool IsOpen()
        {
            return bOpen;
        }

        public void RenderWindow(int windowID)
        {
            if (BattleManager.isInTown)
            {
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 - 110f, 100f, 220f, 30f), "Traveling to mission.");
            }
            if (BattleManager.isToTown)
            {
                GUIManager.getInstance().DrawTextCenteredWhite(new Rect(Screen.width / 2 - 110, 100f, 220f, 30f), "Traveling back home.");
            }
        }

        public void OnGUI()
        {
            if (GUIManager.getInstance().inGame)
            {
                return;
            }
            if (!IsOpen())
            {
                return;
            }
            
            if (BattleManager.isInTown)
            {
                countdown++;
                if (countdown >= 30f)
                {
                    
                    BattleManager.SendPartyOnQuest();
                    AdventureMap.CloseWindow();
                    CloseWindow();
                    countdown = 0;
                    return;
                }
            }
            if (BattleManager.isToTown)
            {
                countdown++;
                if (countdown >= 30f)
                {
                    CloseWindow();
                    AdventureMap.CloseWindow();
                    BattleManager.SendPartyBackHome();
                    countdown = 0;
                    return;
                }
            }

            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(191, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(191);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
        }

        public void Update()
        {
        }
    }
}