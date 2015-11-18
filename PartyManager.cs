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
    public class Draftees : IEquatable<Draftees>
    {
        public int UnitId { get; set; }
        public string uName { get; set; }
        public float Health { get; set; }
        public int Experience { get; set; }
        public bool isEnlisted { get; set; }

        public override string ToString()
        {
            return "ID: " + UnitId + "   Name: " + uName + "   Health: " + Health + "   Experience: " + Experience + "   Enlisted: " + isEnlisted;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Draftees objAsPart = obj as Draftees;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return UnitId;
        }
        public bool Equals(Draftees other)
        {
            if (other == null) return false;
            return (this.UnitId.Equals(other.UnitId));
        }
        // Should also override == and != operators.
    }

    public class PartyManager : MonoBehaviour
    {
        private bool reverseSort;

        public enum SortType
        {
            NONE,
            NAME,
            PROFESSION
        }

        public UnitList.SortType sortType;
        public Type professionSort;
        public static List<Draftees> draftees = new List<Draftees>();
        public static int PartySize;

        public PartyManager()
        {

        }

        public void Start()
        {

        }

        public APlayableEntity[] SortUnits(APlayableEntity[] sortedUnits)
        {
            UnitList.SortType sortType = this.sortType;
            if (sortType != UnitList.SortType.NAME)
            {
                if (sortType == UnitList.SortType.PROFESSION)
                {
                    Array.Sort<APlayableEntity>(sortedUnits, delegate (APlayableEntity U1, APlayableEntity U2)
                    {
                        AProfession profession = U1.getProfession(this.professionSort);
                        AProfession profession2 = U2.getProfession(this.professionSort);
                        if (profession == null && profession2 == null)
                        {
                            return 0;
                        }
                        if (profession == null)
                        {
                            return -1;
                        }
                        if (profession2 == null)
                        {
                            return 1;
                        }
                        int num = profession.getLevel().CompareTo(profession2.getLevel());
                        if (num == 0)
                        {
                            AProfession profession3 = U1.getProfession();
                            AProfession profession4 = U2.getProfession();
                            if (profession3 == profession && profession4 == profession2)
                            {
                                return 0;
                            }
                            if (profession3 == profession)
                            {
                                return 1;
                            }
                            if (profession4 == profession2)
                            {
                                return -1;
                            }
                        }
                        return num;
                    });
                    if (!this.reverseSort)
                    {
                        Array.Reverse(sortedUnits);
                    }
                }
            }
            else
            {
                Array.Sort<APlayableEntity>(sortedUnits, (APlayableEntity U1, APlayableEntity U2) => U1.unitName.CompareTo(U2.unitName));
                if (this.reverseSort)
                {
                    Array.Reverse(sortedUnits);
                }
            }
            return sortedUnits;
        }

        public void ManageParty(int trade)
        {
            PartyManager partyManager = new PartyManager();
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            bool destroyed = false;
            partyManager.SortUnits(array);

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];

                if (aPlayableEntity.isAlive())
                {
                    switch (trade)
                    {
                        case 0:
                            if (i == 0)
                            {
                                draftees.Clear();
                            }
                            draftees.Add(new Draftees() { UnitId = i, uName = aPlayableEntity.unitName, Health = aPlayableEntity.hitpoints, Experience = aPlayableEntity.getProfession().currentXP, isEnlisted = false });
                            continue;
                        case 1:
                            draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience = aPlayableEntity.getProfession().currentXP;
                            draftees.Find(x => x.uName == aPlayableEntity.unitName).Health = aPlayableEntity.hitpoints;
                            continue;
                        case 2:
                            if (!draftees.Exists(x => x.uName == aPlayableEntity.unitName))//new Draftees { uName = aPlayableEntity.unitName }))
                            {
                                aPlayableEntity.Destroy();
                                destroyed = true;
                                continue;
                            }
                            if (draftees.Find(x => x.uName == aPlayableEntity.unitName).Health != aPlayableEntity.hitpoints)
                            {
                                aPlayableEntity.hitpoints = draftees.Find(x => x.uName == aPlayableEntity.unitName).Health;
                            }
                            if (draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience != aPlayableEntity.getProfession().currentXP)
                            {
                                aPlayableEntity.getProfession().setExperience(draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience);
                            }
                            continue;
                    }
                }
            }
            if (destroyed)
            {
                ManageParty(0);
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