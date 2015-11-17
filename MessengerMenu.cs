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
        QuestManager questManager = new QuestManager();

        private MessengerMenu()
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
            HumanEntity humanEntity = MessengerManager.messengerEntity;
            Rect location = new Rect((float)(Screen.width / 2 - 340), 100f, 670f, 380f);

            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }
            GUIManager.getInstance().DrawWindow(location, "A messenger has come asking for assistance!", false);
            GUI.Box(new Rect(location.x + 12f, location.y + 36f, 372f, 56f), string.Empty, GUIManager.getInstance().boxStyle);
            GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 28f, location.y + 40f, 300f, 28f), humanEntity.unitName);
            GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 28f, location.y + 65f, 300f, 28f), "Quest: " + QuestManager.quest.Find(x => x.conditions == false).name);
            GUIManager.getInstance().DrawTextLeftWhite(new Rect(location.x + 28f, location.y + 100f, 670f, 100f), QuestManager.quest.Find(x => x.conditions == false).summary);
            if (GUIManager.getInstance().DrawButton(new Rect(location.xMin + 100f, location.yMin + 340f, 200f, 28f), "Accept Quest"))
            {
                MessengerManager.isAwaitingResults = false;
                CloseWindow();
                QuestManager.isOnQuest = true;
                humanEntity.Destroy();
                AManager<TimeManager>.getInstance().play();
            }
            if (GUIManager.getInstance().DrawButton(new Rect(location.xMax - 100f - 200f, location.yMin + 340f, 200f, 28f), "Deny Quest"))
            {
                MessengerManager.isAwaitingResults = false;
                CloseWindow();
                QuestManager.quest.Clear();
                humanEntity.Destroy();
                AManager<TimeManager>.getInstance().play();
            }
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
            Rect windowRect = GUI.Window(195, rect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(195);
        }

        public void Update()
        {
        }
    }
}

