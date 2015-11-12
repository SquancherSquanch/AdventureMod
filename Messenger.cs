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
    public class Messenger : MonoBehaviour
    {
        public static bool bAdventureMapOpen, isMapCreated, isAwaitingResults;
        public static HumanEntity MessengerEntity;
        private Messenger()
        {
        }

        public void Start()
        {

        }

        public static void SpawnMessenger()
        {
            Vector3 vector;

            vector = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            //UnitManager.getInstance().AddHumanUnit();
            Debug.Log(vector);
            HumanEntity humanEntity = AManager<AssetManager>.getInstance().InstantiateUnit<HumanEntity>();
            humanEntity.unitName = "Messenger";
            humanEntity.fatigue = 1f;
            humanEntity.hunger = 0f;
            humanEntity.maxHP = 100f;
            humanEntity.hitpoints = 100f;
            humanEntity.coordinate = Coordinate.FromWorld(vector);
            humanEntity.faction = AManager<WorldManager>.getInstance().MigrantFaction;
            humanEntity.spottedTimer = 600000f;
            humanEntity.addProfession(new Fisherman(humanEntity, 0));
            humanEntity.SetProfession(typeof(Fisherman));
            humanEntity.preferences["migrant.leaving"] = true;
            humanEntity.interruptTask(new TaskMoveToHall(humanEntity));
            //base.StartCoroutine(UnitManager.getInstance().ReportMerchant(humanEntity.transform));
            UnitManager.getInstance().visitors.Add(humanEntity.transform);
            MessengerEntity = humanEntity;
            isAwaitingResults = true;
        }

        public void OnGUI()
        {
            if(MessengerEntity!= null)
            { 
                if (MessengerEntity.getWhatImDoing().Contains("Leaving map") && isAwaitingResults)
                {
                    AManager<TimeManager>.getInstance().pause();
                    MessengerMenu.OpenWindow();
                    //GUIManager.getInstance().AddTextLine("Load gui here!");
                }
            } 
        }

        public void Update()
        {
        }
    }
}