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
    public class MessengerManager : MonoBehaviour
    {
        public static bool bAdventureMapOpen, isMapCreated, isAwaitingResults;
        public static HumanEntity messengerEntity;
        private int MessengerOdds;
        public static int day;
        private bool hasRolledToday;

        private MessengerManager()
        {
        }

        public void Start()
        {
            hasRolledToday = true;
            isAwaitingResults = false;
        }

        public static void SpawnMessenger()
        {
            Vector3 vector;
            if (AManager<DesignManager>.getInstance().edgeRoads.Count == 0)
            {
                return;
            }
            int num = UnityEngine.Random.Range(0, 4);
            if (num == 0 && !AManager<DesignManager>.getInstance().edgeRoadSouth)
            {
                return;
            }
            if (num == 1 && !AManager<DesignManager>.getInstance().edgeRoadNorth)
            {
                return;
            }
            if (num == 2 && !AManager<DesignManager>.getInstance().edgeRoadEast)
            {
                return;
            }
            if (num == 3 && !AManager<DesignManager>.getInstance().edgeRoadWest)
            {
                return;
            }
            vector = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            //UnitManager.getInstance().AddHumanUnit();
            Debug.Log(vector);
            HumanEntity humanEntity = AManager<AssetManager>.getInstance().InstantiateUnit<HumanEntity>();
            int gender = UnityEngine.Random.Range(0, 100);
            humanEntity.gender = gender <= 0 ? APlayableEntity.Gender.Female: APlayableEntity.Gender.Male;
            humanEntity.unitName = "Messenger";
            humanEntity.fatigue = 1f;
            humanEntity.hunger = 0f;
            humanEntity.maxHP = 100f;
            humanEntity.hitpoints = 100f;
            humanEntity.coordinate = Coordinate.FromWorld(vector);
            humanEntity.faction = AManager<WorldManager>.getInstance().MigrantFaction;
            humanEntity.spottedTimer = 600000f;
            humanEntity.addProfession(new Messenger(humanEntity, 0));
            humanEntity.SetProfession(typeof(Messenger));
            humanEntity.preferences["migrant.leaving"] = true;
            humanEntity.interruptTask(new TaskMoveToHall(humanEntity));
            UnitManager.getInstance().visitors.Add(humanEntity.transform);
            messengerEntity = humanEntity;
            isAwaitingResults = true;
            day = AManager<TimeManager>.getInstance().day;
            GUIManager.getInstance().AddTextLine("A messenger has arrived." , humanEntity.transform, true);
        }

        public void OnGUI()
        {
            if (!BattleManager.isInTown)
            {
                return;
            }
            if (messengerEntity != null)
            {
                //GUIManager.getInstance().AddTextLine(messengerEntity.getWhatImDoing());
                if (messengerEntity.getWhatImDoing().Contains("Leaving map") && isAwaitingResults)
                {
                    AManager<TimeManager>.getInstance().pause();
                    MessengerMenu.OpenWindow();
                }
            }
            //***************************
            if (AManager<TimeManager>.getInstance().timeOfDay.Contains("Morning"))
            {

                if (!hasRolledToday)
                {
                    MessengerOdds = UnityEngine.Random.Range(0, 100);
                    if (MessengerOdds < 25f)
                    {
                        SpawnMessenger();
                        //GUIManager.getInstance().AddTextLine("Messenger Spawned! Odds = " + MessengerOdds);
                    }
                    hasRolledToday = true;
                    day = AManager<TimeManager>.getInstance().day;
                    //GUIManager.getInstance().AddTextLine("Roll failed, good luck tomorrow! Odds = "+ MessengerOdds);
                }
                else
                {
                    if (day != AManager<TimeManager>.getInstance().day)
                    {
                        day = AManager<TimeManager>.getInstance().day;
                        hasRolledToday = false;
                        //GUIManager.getInstance().AddTextLine("Roll reset!");
                    }
                }
            }
        }

        public void Update()
        {
        }
    }
}