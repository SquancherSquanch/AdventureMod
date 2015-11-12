﻿
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
    public class PluginMain : CSharpPlugin, IEventListener
    {
        WorldManager worldManager;
        public static string File = "";

        public override void OnLoad()
        {
            worldManager = WorldManager.getInstance();
            GUIManager.getInstance().AddTextLine("Adventure Mod Loaded");
            GUIManager.getInstance().gameObject.AddComponent(typeof(AdventureMap));
            GUIManager.getInstance().gameObject.AddComponent(typeof(PartyMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(BattleManager));
            GUIManager.getInstance().gameObject.AddComponent(typeof(Draftees));
            GUIManager.getInstance().gameObject.AddComponent(typeof(TransitionScreen));
            GUIManager.getInstance().gameObject.AddComponent(typeof(Messenger));
            GUIManager.getInstance().gameObject.AddComponent(typeof(BattleOverMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(MessengerMenu));
        }

        public override void OnEnable()
        {
            GUIManager.getInstance().AddTextLine("Adventure Mod Enabled");
            EventManager.getInstance().Register(this);
            GUIManager.getInstance().AddTextLine("Adventure Mod Registered Events");
        }

        [EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
            string deadDraftee = evt.getUnit().unitName;
            PartyMenu.draftees.Remove(PartyMenu.draftees.Find(x => x.uName == deadDraftee));
        }

        [EventHandler(Priority.Monitor)]
        public void onMigrantAcceptMonitor(EventMigrantAccept evt)
        { 
            if (evt.unit.unitName == "Messenger")
            {
                GUIManager.getInstance().AddTextLine("A messenger comes this way asking for aid.");
            }
            PartyMenu.draftees.Add(new Draftees() { UnitId = PartyMenu.draftees.Count, uName = evt.unit.unitName, Health = evt.unit.hitpoints, Experience = evt.unit.getProfession().currentXP, isEnlisted = false });
        }

        [EventHandler(Priority.Monitor)]
        public void onMigrantAcceptMonitor(EventMigrantDeny evt)
        {
        }

        [EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            if (evt.invasion.getName() == "wolf" || evt.invasion.getName() == "spider" || evt.invasion.getName() == "skeleton" || evt.invasion.getName() == "necromancer" || evt.invasion.getName() == "goblin")
            {
                GUIManager.getInstance().AddTextLine("A " + evt.invasion.getName() + " has found you!");
                BattleOverMenu.invasion = evt.invasion.getName();
                BattleManager.invasion = evt.invasion.getName();
            }
            if (evt.invasion.getName() == "wolves" || evt.invasion.getName() == "spiders" || evt.invasion.getName() == "skeletons" || evt.invasion.getName() == "goblins")
            {
                GUIManager.getInstance().AddTextLine("Some " + evt.invasion.getName() + " have found you!");
            }

            //GUIManager.getInstance().AddTextLine(evt.invasion.getName());
        }

        [EventHandler(Priority.Normal)]
        public void onGameLoad(EventGameLoad evt)
        {
            if (File == "")
            {
                File = worldManager.settlementName.ToString() + ".tass.gz";
            }

            if (!AdventureMap.isMapCreated)
            {
                AManager<MapManager>.getInstance().CreateWorldMap();
                AdventureMap.isMapCreated = true;
            }

            if (BattleManager.isStartingFight)
            {
                //TransitionScreen.CloseWindow();
                AManager<TimeManager>.getInstance().pause();
                Screen.lockCursor = false;
                Screen.showCursor = true;
                AManager<WorldManager>.getInstance().enableSaving = false;
                GUIManager.getInstance().startMenu = "New6";
                GUIManager.getInstance().inStartMenu = true;
                GUIManager.getInstance().inGame = false;
                BattleManager.isPlacingUnits = true;
            }

            if (PartyMenu.draftees.Count <= 0)
            {
                PartyMenu.ManageParty(0);
            }
        }
    }
}
