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
    public class MessengerMenu : MonoBehaviour
    {
        public static bool _open;
        public Rect windowRect;
        private float intendedWindowWidth = 180f;

        private MessengerMenu()
        {

        }

        public static void OpenWindow()
        {
            MessengerMenu._open = true;
        }

        public static void CloseWindow()
        {
            MessengerMenu._open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }

        public void RenderWindow(int windowID)
        {
            HumanEntity humanEntity = Messenger.MessengerEntity;
            Rect location = new Rect((float)(Screen.width / 2 - 340), 100f, 670f, 380f);

            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }
            GUIManager.getInstance().DrawWindow(location, "A messenger has come, asking for assistance!", false);
            GUI.Box(new Rect(location.x + 12f, location.y + 36f, 372f, 56f), string.Empty, GUIManager.getInstance().boxStyle);
            //GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 28f, location.y + 40f, 300f, 28f), humanEntity.unitName);
            GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 16f, location.y + 100f, 670f, 28f), "  Please, some bullies have beat me up and stolen my lunch moneys.");
            GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 16f, location.y + 128f, 670f, 28f), "I ask for revenge!  Slay them all!"); 
            if (GUIManager.getInstance().DrawButton(new Rect(location.xMin + 100f, location.yMin + 340f, 200f, 28f), "Accept Quest"))
            {
                Messenger.isAwaitingResults = false;
                CloseWindow();
                AManager<TimeManager>.getInstance().play();
            }
            if (GUIManager.getInstance().DrawButton(new Rect(location.xMax - 100f - 200f, location.yMin + 340f, 200f, 28f), "Deny Quest"))
            {
                Messenger.isAwaitingResults = false;
                CloseWindow();
                AManager<TimeManager>.getInstance().play();
            }
        }

        public void OnGUI()
        {
            
            if (!IsOpen())
            {
                return;
            }
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect windowRect = GUI.Window(195, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(195);
        }

        public void Update()
        {
        }
    }
}