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

        }

        public void OnGUI()
        {
            if (bAdventureMapOpen)
            {
                Screen.showCursor = false;
                Screen.lockCursor = true;
            }
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect windowRect = GUI.Window(192, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(192);
        }

        public void Update()
        {
        }
    }
}