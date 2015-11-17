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
    public class Quest : IEquatable<Quest>
    {
        public int type { get; set; }
        public string name { get; set; }
        public string summary { get; set; }
        public bool conditions { get; set; }

        public override string ToString()
        {
            return "type: " + type + "   name: " + name + "   summary: " + summary + "   conditions: " + conditions;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Quest objAsPart = obj as Quest;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return type;
        }
        public bool Equals(Quest other)
        {
            if (other == null) return false;
            return (this.type.Equals(other.type));
        }
        // Should also override == and != operators.
    }
    public class QuestManager : MonoBehaviour
    {
        public int QuestType;
        public static List<Quest> quest = new List<Quest>();
        public static bool isOnQuest;

        public QuestManager()
        {

        }

        public void Start()
        {
            isOnQuest = false;
        }

        public void LoadQuest()
        {
            string name = "Playground Bullies!";
            string summary = "  Please, some bullies have beat me up and stolen my lunch money.\nI ask for revenge!  Slay them all, and you shall be rewarded!";
            quest.Add(new Quest() { type = QuestType, name = name, summary = summary, conditions = false});
        }



        public int GetQuest()
        {
            return QuestType;
        }

        public void OnGUI()
        {

        }

        public void Update()
        {

        }
    }
}