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
    public class AdventureMap : MonoBehaviour
    {
        public static bool bAdventureMapOpen, isMapCreated;
        public Rect windowRect;
        private float intendedWindowWidth = Screen.width;

        private AdventureMap()
        {
        }

        public static void OpenWindow()
        {
            bAdventureMapOpen = true;
            if (isMapCreated)
            {
                
                //GUIManager.getInstance().inGame = false;
                AManager<WorldManager>.getInstance().mapSize = "Large";
                AManager<MapManager>.getInstance().ShowMap(0f, 0f, 1f, 1f, false);
                AManager<MapManager>.getInstance().View3D();
            }
        }

        public static void CloseWindow()
        {
            bAdventureMapOpen = false;
            AManager<MapManager>.getInstance().HideMap();
            Screen.showCursor = true;
            Screen.lockCursor = false;
        }

        public bool IsOpen()
        {
            return bAdventureMapOpen;
        }

        public void RenderWindow(int windowID)
        {
            Rect location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }
        }

        public void OnGUI()
        {
            if (GUIManager.getInstance().inGame)
            {
                return;
            }
            if (!bAdventureMapOpen)
            {
                return;
            }
            Screen.showCursor = false;
            Screen.lockCursor = true;
            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(192, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(192);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
            
        }

        public void Update()
        {
        }
    }
}