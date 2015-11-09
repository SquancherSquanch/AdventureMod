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
    public class TransitionScreen : MonoBehaviour
    {
        public static bool bOpen;
        GUIManager guiMgr;
        private ControlPlayer controller;
        
        public Rect windowRect;
        private float intendedWindowWidth = Screen.width;
        private Vector2 horizontalScrollPosition;
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
            this.controller = this.guiMgr.controllerObj.GetComponent<ControlPlayer>();
            countdown = 0f;
        }
        
        public static void OpenWindow()
        {
            countdown = 0;
            if (BattleManager.isInTown)
            {
                AManager<WorldManager>.getInstance().SaveGame();
            }
            TransitionScreen.bOpen = true;
            GUIManager.getInstance().inGame = false;
            
        }

        public static void CloseWindow()
        {
            TransitionScreen.bOpen = false;
        }

        public static bool IsOpen()
        {
            return bOpen;
        }

        public void RenderWindow(int windowID)
        {
            if (IsOpen())
            {
                if (BattleManager.isInTown)
                {
                    this.guiMgr.DrawTextCenteredWhite(new Rect(Screen.width / 2 - 110f, 100f, 220f, 30f), "Traveling to mission.");
                }
                if (BattleManager.isToTown)
                {
                    this.guiMgr.DrawTextCenteredWhite(new Rect(Screen.width / 2 - 110, 100f, 220f, 30f), "Traveling back home.");
                }
            }
        }

        public void OnGUI()
        {
            if (!IsOpen())
            {
                return;
            }

            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(91, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, this.guiMgr.hiddenButtonStyle);
            GUI.FocusWindow(91);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
            countdown++;
            if (BattleManager.isInTown)
            {
                if (countdown >= 30f)
                {
                    BattleManager.SendPartyOnQuest();
                    AdventureMap.CloseWindow();
                    CloseWindow();
                    countdown = 0;
                }
            }
            if (BattleManager.isToTown)
            {
                if (countdown >= 300f)
                {
                    CloseWindow();
                    AdventureMap.CloseWindow();
                    BattleManager.SendPartyBackHome();
                    countdown = 0;
                }
            }
        }

        public void Update()
        {
        }
    }
}