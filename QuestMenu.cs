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
    public class QuestMenu : MonoBehaviour
    {
        public static bool _open;
        public Rect windowRect;
        private float intendedWindowWidth = 180f;

        private QuestMenu()
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
            Rect windowRect = GUI.Window(196, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(196);
        }

        public void Update()
        {
        }
    }
}