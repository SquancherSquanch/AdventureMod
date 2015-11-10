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

    public class PartyMenu : MonoBehaviour
    {
        private bool _open;
        GUIManager guiMgr;
        private bool reverseSort, openDraft;
        private ControlPlayer controller;
        public static List<Draftees> draftees = new List<Draftees>();

        public enum SortType
        {
            NONE,
            NAME,
            PROFESSION
        }

        public UnitList.SortType sortType;
        public Type professionSort;
        public Rect windowRect;
        private float intendedWindowWidth = 490f;
        private Vector2 horizontalScrollPosition;
        public Rect windowViewRect;
        private Vector2? lastPivot;
        private Vector2 scrollPosition;
        public int PartySize;
        private PartyMenu()
        {
        }

        public void Start()
        {
            guiMgr = AManager< GUIManager>.getInstance();
            windowRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            windowViewRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            this.scrollPosition = Vector2.zero;
            this.horizontalScrollPosition = Vector2.zero;
            this.controller = this.guiMgr.controllerObj.GetComponent<ControlPlayer>();
            this.PartySize = 0;
            openDraft = false;
        }

        private APlayableEntity[] SortUnits(APlayableEntity[] sortedUnits)
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
        
        public static void ManageParty(int trade)
        {
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            PartyMenu partyMenu = new PartyMenu();
            partyMenu.SortUnits(array);

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                
                if (aPlayableEntity.isAlive())
                {
                    if (trade == 0)
                    {
                        if (i == 0)
                        {
                            draftees.Clear();
                        }
                        draftees.Add(new Draftees() { UnitId = i, uName = aPlayableEntity.unitName, Health = aPlayableEntity.hitpoints, Experience = aPlayableEntity.getProfession().currentXP, isEnlisted = false });
                        continue;
                    }

                    if (draftees.Contains(new Draftees { uName = aPlayableEntity.unitName }))
                    {
                        //set to for transfer
                        if (trade == 1)
                        {
                            draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience = aPlayableEntity.getProfession().currentXP;
                            draftees.Find(x => x.uName == aPlayableEntity.unitName).Health = aPlayableEntity.hitpoints;
                        }
                        //transfer
                        if (trade == 2)
                        {
                            if (draftees.Find(x => x.uName == aPlayableEntity.unitName).Health <= aPlayableEntity.hitpoints)
                            {
                                aPlayableEntity.hitpoints = draftees.Find(x => x.uName == aPlayableEntity.unitName).Health;
                            }
                            if (draftees.Find(x => x.uName == aPlayableEntity.unitName).Health >= aPlayableEntity.hitpoints)
                            {
                                aPlayableEntity.hitpoints = draftees.Find(x => x.uName == aPlayableEntity.unitName).Health;
                            }
                            if (draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience != aPlayableEntity.getProfession().currentXP)
                            {
                                aPlayableEntity.getProfession().setExperience(draftees.Find(x => x.uName == aPlayableEntity.unitName).Experience);
                            }
                        }
                    }
                }
            }
        }

        private void Rotate(Vector2 pivot)
        {
            this.lastPivot = new Vector2?(pivot);
            GUIUtility.RotateAroundPivot(-45f, pivot);
        }

        private void EndRotate()
        {
            Vector2? vector = this.lastPivot;
            if (!vector.HasValue)
            {
                return;
            }
            float arg_2A_0 = 45f;
            Vector2? vector2 = this.lastPivot;
            GUIUtility.RotateAroundPivot(arg_2A_0, vector2.GetValueOrDefault());
            this.lastPivot = default(Vector2?);
        }
        
        public void OpenWindow()
        {
            this._open = true;
        }

        public void CloseWindow()
        {
            this._open = false;
            openDraft = false;
        }

        public bool IsOpen()
        {
            return _open;
        }

        public void RenderWindow(int windowID)
        {
            float num = 25f;
            float num2 = 9f;
            if (IsOpen())
            {
                Rect location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
                if (location.Contains(Event.current.mousePosition))
                {
                    this.guiMgr.mouseInGUI = true;
                }
                this.guiMgr.DrawWindow(location, "Party Menu", false);
                if (GUI.Button(new Rect(location.xMax - 24f, location.yMin + 4f, 20f, 20f), string.Empty, this.guiMgr.closeWindowButtonStyle))
                {
                    this.CloseWindow();
                    return;
                }
                GUI.DragWindow(new Rect(0f, 0f, this.windowRect.width, 24f));
                if (this.windowRect.width < this.intendedWindowWidth)
                {
                    Rect position = new Rect(this.windowRect.x, this.windowRect.y - 25f, this.windowRect.width - num2, this.windowRect.height - num);
                    this.horizontalScrollPosition = GUI.BeginScrollView(position, this.horizontalScrollPosition, this.windowViewRect, true, false);
                }
                float num3 = 245f;
                float num4 = 35f;
                if (this.windowRect.width >= this.intendedWindowWidth)
                {
                    num4 += 22f;
                }

                APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
                this.SortUnits(array);
                Rect rect = new Rect(0f + num3 - 45f, 0f + num4 - 50, 165f, 10f);
                this.Rotate(new Vector2(rect.xMin + rect.width / 2f, rect.yMin + rect.height / 2f));
                Rect location3 = new Rect(rect.x, rect.y, 60f, 2f);
                this.guiMgr.DrawLineBlack(location3);
                this.EndRotate();
                rect = new Rect(0f + num3 + 221f, 0f + num4 - 50, 165f, 10f);
                this.Rotate(new Vector2(rect.xMin + rect.width / 2f, rect.yMin + rect.height / 2f));
                location3 = new Rect(rect.x, rect.y, 60f, 2f);
                this.guiMgr.DrawLineBlack(location3);
                this.EndRotate();
                Rect rect2 = new Rect(7f, 0f + num4 - 20, 225f, 25f);
                this.guiMgr.DrawTextCenteredWhite(rect2, "Party Members");
                
                this.guiMgr.DrawLineBlack(new Rect(num2, num4 + 10f, this.windowRect.width, 2f));
                num3 = 2f;
                num4 = 123f;
                if (this.windowRect.width >= this.intendedWindowWidth)
                {
                    num4 += 22f;
                }
                Rect position2 = new Rect(num3, num4 - 26f, this.windowViewRect.width - num3 - 25f, this.windowViewRect.height - num4 + 6f);
                if (this.windowRect.width >= this.intendedWindowWidth)
                {
                    position2.height += 37f;
                }
                Rect viewRect = new Rect(num3, num4, this.windowViewRect.width - num3 - num, (float)(array.Length * 32));
                //this.scrollPosition = GUI.BeginScrollView(position2, this.scrollPosition, viewRect);
                num4 += 5f;
                for (int i = 0; i < 6; i++)
                {
                    num3 = 2f;
                    Rect location5 = new Rect(0f + num3 - 17f, 0f + num4 - 70f, 225f, 25f);
                    this.guiMgr.DrawLineBlack(new Rect(num3 + 5f, num4 - 45f, this.windowViewRect.width - 20f, 2f));
                    num3 += 236f;
                    this.guiMgr.DrawLineBlack(new Rect(num3 - 17f, num4 - 84f, 2f, this.windowViewRect.height - 5f));
                    this.guiMgr.DrawLineBlack(new Rect(num3 + 249f, num4 - 84f, 2f, this.windowViewRect.height - 5f));
                    Rect rect4 = new Rect(num3, num4, 45f, 32f);
                    Rect position3 = rect4;
                    position3.y -= 5f;
                    num3 += 8f;
                    num4 += 32f;
                }
                num3 = 2f;
                num4 = 123f;
                float num5 = 123f;
                float num6 = num3;
                int sizeincrease = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    APlayableEntity aPlayableEntity = array[i];
                    if (aPlayableEntity.isAlive())
                    {
                        if (draftees.Contains(new Draftees { uName = aPlayableEntity.unitName }))
                        {
                            if (!draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted)
                            {
                                sizeincrease++;
                                num3 = 2f;
                                if (sizeincrease >= 8 && sizeincrease <= 14)
                                {
                                    if (sizeincrease == 8)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 225f;
                                }
                                if (sizeincrease >= 15 && sizeincrease <= 21)
                                {
                                    if (sizeincrease == 15)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 450f;
                                }
                                if (sizeincrease >= 22 && sizeincrease <= 28)
                                {
                                    if (sizeincrease == 22)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 675f;
                                }
                                Rect location8 = new Rect(0f + num3 + 525f, 0f + num4 - 45f, 225f, 25f);
                                Rect position4 = new Rect(0f + num3 + 500f, 0f + num4 - 45f, 22f, 22f);
                                this.guiMgr.DrawTextLeftWhite(location8, aPlayableEntity.unitName);
                                if (GUI.Button(location8, string.Empty, this.guiMgr.hiddenButtonStyle))
                                {
                                    if (PartySize < 6)
                                    {
                                        draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = true;
                                        PartySize++;
                                    }
                                }
                                if (aPlayableEntity.getProfession().getProfessionName() == "Infantry")
                                {
                                    Resource resource5 = Resource.FromID(this.guiMgr.weaponsSortType[1]);
                                    GUI.DrawTexture(position4, resource5.icon);
                                }
                                if (aPlayableEntity.getProfession().getProfessionName() == "Archer")
                                {
                                    Resource resource5 = Resource.FromID(this.guiMgr.weaponsSortType[5]);
                                    GUI.DrawTexture(position4, resource5.icon);
                                }

                                num4 += 32f;
                            }
                            else
                            {
                                Rect location10 = new Rect(0f + num6 - 17f, 0f + num5 - 45f, 225f, 25f);
                                this.guiMgr.DrawTextRightWhite(location10, aPlayableEntity.unitName);
                                this.guiMgr.DrawProgressBar(new Rect(num6 + 250f, num5 - 45f, 142f, 22f), aPlayableEntity.hitpoints / 100f, true);
                                Rect location6 = new Rect(num6 + 400f, num5 - 45f, 22f, 22f);
                                Rect position4 = new Rect(num6 + 440f, num5 - 45f, 22f, 22f);
                                if (aPlayableEntity.getProfession().getProfessionName() == "Infantry")
                                {
                                    Resource resource5 = Resource.FromID(this.guiMgr.weaponsSortType[1]);
                                    GUI.DrawTexture(location6, resource5.icon);
                                }
                                if (aPlayableEntity.getProfession().getProfessionName() == "Archer")
                                {
                                    Resource resource5 = Resource.FromID(this.guiMgr.weaponsSortType[5]);
                                    GUI.DrawTexture(location6, resource5.icon);
                                }
                                if (GUI.Button(location6, string.Empty, this.guiMgr.hiddenButtonStyle))
                                {
                                    this.controller.GetComponent<ControlPlayer>().MoveToPosition(aPlayableEntity.transform.position);
                                    this.controller.GetComponent<ControlPlayer>().SelectObject(aPlayableEntity.transform, false);
                                }
                                if (GUI.Button(new Rect(position4), string.Empty, this.guiMgr.closeWindowButtonStyle))
                                {
                                    if (!BattleManager.isFighting)
                                    {
                                        if (PartySize > 0)
                                        {
                                            draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = false;
                                            PartySize--;
                                        }
                                    }
                                }
                                num5 += 32f;
                            }
                        }
                    }
                }
                
                if (this.guiMgr.DrawButton(new Rect(500f - 157f, 30f, 142f, 32f), "Draftees"))
                {
                    openDraft = (openDraft == false) ? true : false;
                }

                if (openDraft)
                {
                    if (sizeincrease <= 7)
                    {
                        this.intendedWindowWidth = 780f;
                    }
                    if (sizeincrease >= 8 && sizeincrease <= 14)
                    {
                        this.intendedWindowWidth = 980f;
                    }
                    if (sizeincrease >= 15 && sizeincrease <= 21)
                    {
                        this.intendedWindowWidth = 1180f;
                    }
                    if (sizeincrease >= 22 && sizeincrease <= 28)
                    {
                        this.intendedWindowWidth = 1380f;
                    }
                    if (sizeincrease >= 29 && sizeincrease <= 35)
                    {
                        this.intendedWindowWidth = 1580f;
                    }
                }
                else
                {
                    this.intendedWindowWidth = 490f;
                }

                if (this.guiMgr.DrawButton(new Rect(500f - 157f, 280f, 142f, 32f), "Send Party"))
                {
                    this.CloseWindow();
                    if (Time.timeSinceLevelLoad < 12)
                    {
                        float timeremaining = 12f - Time.timeSinceLevelLoad;
                        GUIManager.getInstance().AddTextLine("Please wait while game loads:" + timeremaining);
                    }
                    else
                    {
                        AManager<WorldManager>.getInstance().SaveGame();
                        AManager<TimeManager>.getInstance().pause();
                        TransitionScreen.OpenWindow();
                        AdventureMap.OpenWindow();
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (GUIManager.getInstance().unitList.isOpen())
            {
                CloseWindow();
                return;
            }
            if (!this.guiMgr.inGame)
            {
                return;
            }
            if (!IsOpen())
            {
                return;
            }
            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(190, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, this.guiMgr.hiddenButtonStyle);
            GUI.FocusWindow(190);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                ResourceManager.getInstance().AddResource(0, 10);
                ResourceManager.getInstance().AddResource(4, 10);
                //ResourceManager.getInstance().currentStored[ResourceManager.getInstance().resources[0].storageIndex] += (float)this.materials[j] * this.resources[j].mass;
                //BattleOverMenu.OpenWindow();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                GUIManager.getInstance().AddTextLine( ResourceManager.getInstance().resources[0].storageIndex+" : "+ ResourceManager.getInstance().resources[0].value);
                //BattleManager.Reward(1);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                GUIManager.getInstance().AddTextLine("" + draftees.Count);
                /*BattleManager.Reward();
                for (int i = 0; i < BattleManager.bonusLoot; i++)
                {
                    GUIManager.getInstance().AddTextLine(""+ BattleManager.bonusLoot +" : " + BattleManager.rewards.Count +" : " + BattleManager.rewards.Find(x => x.id == i).type + " : " + +BattleManager.rewards.Find(x => x.id == i).itemid + " : " + BattleManager.rewards.Find(x => x.id == i).amount);
                }*/
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (ALivingEntity entity in UnitManager.getInstance().allUnits)
                {
                    if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(entity.faction) != Alignment.Ally)
                    {
                        entity.Destroy();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (IsOpen())
                {
                    CloseWindow();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (GUIManager.getInstance().inGame == true)
                {
                    if (this.IsOpen())
                        CloseWindow();
                    else
                        OpenWindow();
                }
            }
        }
    }
}