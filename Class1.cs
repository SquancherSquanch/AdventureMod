using System;
using System.Collections.Generic;
using Timber_and_Stone.Tasks;
using Timber_and_Stone.Utility;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.Blocks;
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
            GUIManager.getInstance().gameObject.AddComponent(typeof(TransitionScreen));
            GUIManager.getInstance().gameObject.AddComponent(typeof(PartyMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(BattleStartMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(BattleOverMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(MessengerMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(QuestMenu));
            GUIManager.getInstance().gameObject.AddComponent(typeof(BattleManager));
            GUIManager.getInstance().gameObject.AddComponent(typeof(MessengerManager));
            GUIManager.getInstance().gameObject.AddComponent(typeof(QuestManager));
            GUIManager.getInstance().gameObject.AddComponent(typeof(PartyManager));
            GUIManager.getInstance().gameObject.AddComponent(typeof(Draftees));
            GUIManager.getInstance().gameObject.AddComponent(typeof(Messenger));
            GUIManager.getInstance().gameObject.AddComponent(typeof(Bandit));

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

            if (!evt.getUnit().GetType().Equals(typeof(HumanEntity)))
            {   
                if (BattleManager.EnemyCount <= 0 && BattleManager.isFighting)
                {
                    BattleManager.Reward();
                    BattleManager.isBattleOver = true;
                }
                if (BattleManager.EnemyCount > 0)
                {
                    BattleManager.EnemyCount--;
                }
            }
            else
            {
                HumanEntity bandit = (HumanEntity)evt.getUnit();

                if (PartyManager.draftees.Exists(x => x.uName == evt.getUnit().unitName))
                {
                    
                    if (bandit.getProfession().getProfessionName() != "Bandit")
                    {
                        if (PartyManager.draftees.Find(x => x.uName == evt.getUnit().unitName).isEnlisted)
                        {
                            if (PartyManager.PartySize > 0)
                            {
                                PartyManager.PartySize--;
                            }
                        }
                        PartyManager.draftees.Remove(PartyManager.draftees.Find(x => x.uName == evt.getUnit().unitName));
                        if (BattleManager.isInTown)
                        {
                            PartyManager partyManager = new PartyManager();
                            partyManager.ManageParty(2);
                        }
                    }
                    else
                    {
                        evt.getUnit().Destroy();
                    }
                }
                else
                {
                    evt.getUnit().Destroy();
                }
            }

            if (MessengerManager.messengerEntity != null)
            {
                if (evt.getUnit().unitName == MessengerManager.messengerEntity.unitName)
                {
                    evt.getUnit().Destroy();
                }
            }

           
            
        }

        [EventHandler(Priority.Monitor)]
        public void onMigrantAcceptMonitor(EventMigrantAccept evt)
        {
            PartyManager.draftees.Add(new Draftees() { UnitId = PartyManager.draftees.Count, uName = evt.unit.unitName, Health = evt.unit.hitpoints, Experience = evt.unit.getProfession().currentXP, isEnlisted = false });
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
                float roll = UnityEngine.Random.value;
                GUIManager.getInstance().AddTextLine("bandit roll " + roll);
                if (roll < 0.25f)
                {
                    evt.invasion.CancelInvasion();
                    MessengerManager.SpawnBandit();
                    return;
                }
                if (BattleManager.isFighting && BattleManager.isPlaced)
                {
                    GUIManager.getInstance().AddTextLine("A " + evt.invasion.getName() + " has found you!");
                }
                BattleOverMenu.invasion = evt.invasion.getName();
                BattleManager.invasion = evt.invasion.getName();
            }
            //GUIManager.getInstance().AddTextLine(evt.invasion.getName());
        }

        [EventHandler(Priority.Normal)]
        public void onGameLoad(EventGameLoad evt)
        {
            if (File == "")
            {
                File = worldManager.settlementName.ToString();
            }

            if (!AdventureMap.isMapCreated)
            {
                AManager<MapManager>.getInstance().CreateWorldMap();
                AdventureMap.isMapCreated = true;
            }

            if (BattleManager.isStartingFight)
            {
                TransitionScreen.CloseWindow();
                AManager<TimeManager>.getInstance().pause();
                Screen.lockCursor = false;
                Screen.showCursor = true;
                GUIManager.getInstance().startMenu = "";
                GUIManager.getInstance().inStartMenu = true;
                GUIManager.getInstance().inGame = false;
                BattleManager.isStartingFight = false;
            }

            if (PartyManager.draftees.Count <= 0)
            {
                PartyManager partyManager = new PartyManager();
                partyManager.ManageParty(0);
            }
        }
    }
}
