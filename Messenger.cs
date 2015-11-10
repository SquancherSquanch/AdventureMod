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
        public static bool bAdventureMapOpen, isMapCreated;

        private Messenger()
        {
        }


        public void Migrate(Vector3 startPos)
        {
            Vector3 pos;
            if (startPos == Vector3.zero)
            {
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
                pos = AManager<DesignManager>.getInstance().edgeRoads[UnityEngine.Random.Range(0, AManager<DesignManager>.getInstance().edgeRoads.Count - 1)].world + Vector3.up * AManager<ChunkManager>.getInstance().voxelSize;
            }
            else
            {
                pos = startPos;
            }
            int num2 = UnityEngine.Random.Range(1, 12);
            Transform transform = null;
            switch (num2)
            {
                case 1:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("archer", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 2:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("blacksmith", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 3:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("builder", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 4:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("carpenter", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 5:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("farmer", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 6:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("engineer", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 7:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("forager", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 8:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("infantry", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 9:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("miner", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 10:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("stone mason", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
                case 11:
                    transform = AManager<UnitManager>.getInstance().AddHumanUnit("wood chopper", pos, true, true, UnityEngine.Random.Range(0, 2) == 1);
                    break;
            }
            AManager<UnitManager>.getInstance().AddMigrantResources(transform.GetComponent<APlayableEntity>());
            EventMigrant eventMigrant = new EventMigrant(transform.GetComponent<APlayableEntity>());
            EventManager.getInstance().InvokePre(eventMigrant);
            EventManager.getInstance().InvokePost(eventMigrant);
            if (eventMigrant.result == Result.Deny)
            {
                transform.GetComponent<APlayableEntity>().Destroy();
            }
        }


        public void OnGUI()
        {
        }

        public void Update()
        {
        }
    }
}