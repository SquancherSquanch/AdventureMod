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
    public class PartyMenu : MonoBehaviour
    {
        private bool _open, bShowMilitary, bShowCivilian, bSeekTarget;
        private bool  openDraft;
        private ControlPlayer controller;
        QuestManager questManager = new QuestManager();
        PartyManager partyManager = new PartyManager();
        

        public Rect windowRect;
        private float intendedWindowWidth = 490f;
        private Vector2 horizontalScrollPosition;
        public Rect windowViewRect;
        public Rect location;
        private Vector2? lastPivot;
        private Vector2 scrollPosition;

        public PartyMenu()
        {
        }

        public void Start()
        {
            bShowMilitary = false;
            bShowCivilian = false;
            windowRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            windowViewRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            this.scrollPosition = Vector2.zero;
            this.horizontalScrollPosition = Vector2.zero;
            this.controller = GUIManager.getInstance().controllerObj.GetComponent<ControlPlayer>();
            PartyManager.PartySize = 0;
            openDraft = false;
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
            if (PartyManager.draftees.Count <= 0)
            {
                partyManager.ManageParty(0);
            }
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
            if ( bShowCivilian == true )
            {
                    bShowMilitary = false;
            }

            if (bShowMilitary == true)
            {
                    bShowCivilian = false;
            }

            float num = 25f;
            float num2 = 9f;
            
            location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
            if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }
            GUIManager.getInstance().DrawWindow(location, "Party Menu", false);
            if (GUI.Button(new Rect(location.xMax - 24f, location.yMin + 4f, 20f, 20f), string.Empty, GUIManager.getInstance().closeWindowButtonStyle))
            {
                if (openDraft)
                {
                    openDraft = false;
                    return;
                }
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
            partyManager.SortUnits(array);
            Rect rect = new Rect(0f + num3 - 45f, 0f + num4 - 50, 165f, 10f);
            this.Rotate(new Vector2(rect.xMin + rect.width / 2f, rect.yMin + rect.height / 2f));
            Rect location3 = new Rect(rect.x, rect.y, 60f, 2f);
            GUIManager.getInstance().DrawLineBlack(location3);
            this.EndRotate();
            GUIManager.getInstance().DrawCheckBox(new Rect(rect.x - 180f, rect.y + 280f, 200f, 24f), "Seek charge target enemies.", ref bSeekTarget);

            if (openDraft)
            {
                rect = new Rect(0f + num3 + 221f, 0f + num4 - 50, 165f, 10f);
                this.Rotate(new Vector2(rect.xMin + rect.width / 2f, rect.yMin + rect.height / 2f));
                location3 = new Rect(rect.x, rect.y, 60f, 2f);
                GUIManager.getInstance().DrawLineBlack(location3);
                this.EndRotate();
                GUIManager.getInstance().DrawCheckBox(new Rect(rect.x + 65f, rect.y + 40f, 200f, 24f), "Sort by military", ref bShowMilitary);
                GUIManager.getInstance().DrawCheckBox(new Rect(rect.x + 65f, rect.y + 20f, 200f, 24f), "Sort by civilian", ref bShowCivilian);
            }
            Rect rect2 = new Rect(7f, 0f + num4 - 20, 225f, 25f);
            GUIManager.getInstance().DrawTextCenteredWhite(rect2, "Party Members");
                
            GUIManager.getInstance().DrawLineBlack(new Rect(num2, num4 + 10f, this.windowRect.width, 2f));
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
                GUIManager.getInstance().DrawLineBlack(new Rect(num3 + 5f, num4 - 45f, this.windowViewRect.width - 20f, 2f));
                num3 += 236f;
                GUIManager.getInstance().DrawLineBlack(new Rect(num3 - 17f, num4 - 84f, 2f, this.windowViewRect.height - 5f));
                GUIManager.getInstance().DrawLineBlack(new Rect(num3 + 249f, num4 - 84f, 2f, this.windowViewRect.height - 5f));
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
            if (GUIManager.getInstance().DrawButton(new Rect(500f - 157f, 30f, 142f, 32f), "Draftees"))
            {
                openDraft = (openDraft == false) ? true : false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                if (aPlayableEntity.isAlive())
                {
                    if (PartyManager.draftees.Exists(x => x.uName == aPlayableEntity.unitName))
                    {
                        if (openDraft)
                        {
                            if (!PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted)
                            {
                                if (bShowMilitary && (aPlayableEntity.getProfession().getProfessionName() == "Infantry" || aPlayableEntity.getProfession().getProfessionName() == "Archer"))
                                {
                                    num3 = 2f;
                                    if (sizeincrease % 7 == 0)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 225f * (sizeincrease / 7);
                                    sizeincrease++;
                                    Rect location8 = new Rect(0f + num3 + 525f, 0f + num4 - 45f, 250f, 25f);
                                    Rect position4 = new Rect(0f + num3 + 495f, 0f + num4 - 45f, 22f, 22f);

                                    if (aPlayableEntity.hitpoints < 26)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.red);
                                    }
                                    if (aPlayableEntity.hitpoints > 25 && aPlayableEntity.hitpoints < 50)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.yellow);
                                    }
                                    if (aPlayableEntity.hitpoints > 49)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.white);
                                    }

                                    if (GUI.Button(location8, string.Empty, GUIManager.getInstance().hiddenButtonStyle))
                                    {
                                        if (PartyManager.PartySize < 6)
                                        {
                                            PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = true;
                                            PartyManager.PartySize++;
                                        }
                                    }
                                    GUI.DrawTexture(position4, AManager<AssetManager>.getInstance().GetSkillIconFromProfession(aPlayableEntity.getProfession().getProfessionName()));
                                    num4 += 32f;
                                }
                                else if (!bShowMilitary && !bShowCivilian)
                                {
                                    num3 = 2f;
                                    if (sizeincrease % 7 == 0)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 225f * (sizeincrease / 7);
                                    sizeincrease++;
                                    Rect location8 = new Rect(0f + num3 + 525f, 0f + num4 - 45f, 250f, 25f);
                                    Rect position4 = new Rect(0f + num3 + 495f, 0f + num4 - 45f, 22f, 22f);
                                    if (aPlayableEntity.hitpoints < 26)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.red);
                                    }
                                    if (aPlayableEntity.hitpoints > 25 && aPlayableEntity.hitpoints < 50)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.yellow);
                                    }
                                    if (aPlayableEntity.hitpoints > 49)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.white);
                                    }
                                    if (GUI.Button(location8, string.Empty, GUIManager.getInstance().hiddenButtonStyle))
                                    {
                                        if (PartyManager.PartySize < 6)
                                        {
                                            PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = true;
                                            PartyManager.PartySize++;
                                        }
                                    }
                                    GUI.DrawTexture(position4, AManager<AssetManager>.getInstance().GetSkillIconFromProfession(aPlayableEntity.getProfession().getProfessionName()));
                                    num4 += 32f;
                                }
                                else if (bShowCivilian && (aPlayableEntity.getProfession().getProfessionName() != "Infantry" && aPlayableEntity.getProfession().getProfessionName() != "Archer"))
                                {
                                    num3 = 2f;
                                    if (sizeincrease % 7 == 0)
                                    {
                                        num4 = 123f;
                                    }
                                    num3 += 225f * (sizeincrease / 7);
                                    sizeincrease++;
                                    Rect location8 = new Rect(0f + num3 + 525f, 0f + num4 - 45f, 250f, 25f);
                                    Rect position4 = new Rect(0f + num3 + 495f, 0f + num4 - 45f, 22f, 22f);

                                    if (aPlayableEntity.hitpoints < 26)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.red);
                                    }
                                    if (aPlayableEntity.hitpoints > 25 && aPlayableEntity.hitpoints < 50)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.yellow);
                                    }
                                    if (aPlayableEntity.hitpoints > 49)
                                    {
                                        GUIManager.getInstance().DrawTextLeftWhite(location8, aPlayableEntity.unitName, Color.white);
                                    }

                                    if (GUI.Button(location8, string.Empty, GUIManager.getInstance().hiddenButtonStyle))
                                    {
                                        if (PartyManager.PartySize < 6)
                                        {
                                            PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = true;
                                            PartyManager.PartySize++;
                                        }
                                    }
                                    GUI.DrawTexture(position4, AManager<AssetManager>.getInstance().GetSkillIconFromProfession(aPlayableEntity.getProfession().getProfessionName()));
                                    num4 += 32f;
                                }
                            }
                        }

                        if (PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted)
                        {
                            if (aPlayableEntity.getProfession().getProfessionName() == "Infantry" || aPlayableEntity.getProfession().getProfessionName() == "Archer")
                            {
                                aPlayableEntity.preferences["preference.attackchargetargets"] = bSeekTarget;
                            }
                            Rect location10 = new Rect(0f + num6 - 37f, 0f + num5 - 45f, 250f, 25f);

                            if (aPlayableEntity.hitpoints < 26)
                            {
                                GUIManager.getInstance().DrawTextRightWhite(location10, aPlayableEntity.unitName, Color.red);
                            }
                            if (aPlayableEntity.hitpoints > 25 && aPlayableEntity.hitpoints < 50)
                            {
                                GUIManager.getInstance().DrawTextRightWhite(location10, aPlayableEntity.unitName, Color.yellow);
                            }
                            if (aPlayableEntity.hitpoints > 49)
                            {
                                GUIManager.getInstance().DrawTextRightWhite(location10, aPlayableEntity.unitName, Color.white);
                            }
                            GUIManager.getInstance().DrawProgressBar(new Rect(num6 + 250f, num5 - 45f, 142f, 22f), aPlayableEntity.hitpoints / 100f, true);
                            Rect location6 = new Rect(num6 + 405f, num5 - 45f, 22f, 22f);
                            Rect position4 = new Rect(num6 + 440f, num5 - 45f, 22f, 22f);

                            if (GUI.Button(location6, string.Empty, GUIManager.getInstance().hiddenButtonStyle))
                            {
                                this.controller.GetComponent<ControlPlayer>().MoveToPosition(aPlayableEntity.transform.position);
                                this.controller.GetComponent<ControlPlayer>().SelectObject(aPlayableEntity.transform, false);
                            }

                            GUI.DrawTexture(location6, AManager<AssetManager>.getInstance().GetSkillIconFromProfession(aPlayableEntity.getProfession().getProfessionName()));
                            if (GUI.Button(new Rect(position4), string.Empty, GUIManager.getInstance().closeWindowButtonStyle))
                            {
                                if (!BattleManager.isFighting)
                                {
                                    if (PartyManager.PartySize > 0)
                                    {
                                        PartyManager.draftees.Find(x => x.uName == aPlayableEntity.unitName).isEnlisted = false;
                                        PartyManager.PartySize--;
                                    }
                                }
                            }
                            num5 += 32f;
                        }
                    }
                }
            }

            if (openDraft)
            {
                this.intendedWindowWidth = 800 + 200 * (sizeincrease / 7);
            }
            else
            {
                this.intendedWindowWidth = 490f;
            }

            if (GUIManager.getInstance().DrawButton(new Rect(500f - 157f, 280f, 142f, 32f), "Send Party"))
            {
                    
                if(PartyManager.PartySize <= 0)
                {
                    GUIManager.getInstance().AddTextLine("Can't leave for battle with no troops!");
                    return;
                }
                if (!QuestManager.isOnQuest)
                {
                    GUIManager.getInstance().AddTextLine("No quest to send party on.");
                    return;
                }
                if (Time.timeSinceLevelLoad < 12)
                {
                    float timeremaining = 12f - Time.timeSinceLevelLoad;
                    GUIManager.getInstance().AddTextLine("Please wait while game loads:" + timeremaining);
                    return;
                }
                    
                this.CloseWindow();
                GUIManager.getInstance().inGame = false;
                GUIManager.getInstance().inStartMenu = true;
                GUIManager.getInstance().startMenu = "";
                AManager<WorldManager>.getInstance().SaveGame();
                AManager<TimeManager>.getInstance().pause();
                TransitionScreen.OpenWindow();
                AdventureMap.OpenWindow();
                DesignManager designManager = new DesignManager();
                //designManager.farmZones.Clear();
                //designManager.livestockZones.Clear();
                //designManager.roadZones.Clear();
                //AManager<DesignManager>.getInstance().guardPositions.Clear();
                //AManager<DesignManager>.getInstance().selectedBlocks.Clear();
            }
        }

        public void OnGUI()
        {
            if (GUIManager.getInstance().unitList.isOpen())
            {
                CloseWindow();
                return;
            }
            if (!GUIManager.getInstance().inGame)
            {
                return;
            }
            if (!IsOpen())
            {
                return;
            }
            
            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(190, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            GUI.FocusWindow(190);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
        }

        public void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (this.controller.GetComponent<ControlPlayer>().selectedObject != null)
                {
                    
                    APlayableEntity entity = this.controller.GetComponent<ControlPlayer>().selectedObject.GetComponent<APlayableEntity>();
                    entity.hitpoints = 0;
                }
                MessengerManager.SpawnMessenger();
                //BattleManager.TransferLoot();
                //BattleOverMenu.OpenWindow();
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                toggle = (toggle == false) ? true : false;
                if (toggle)
                {
                    MessengerMenu.OpenWindow();
                }
                else
                {
                    MessengerMenu.CloseWindow();
                }
                //BattleManager.Reward(1);
            }
             

            if (Input.GetKeyDown(KeyCode.M))
            {
                toggle = (toggle == false) ? true : false;

                if (toggle)
                {
                    BattleManager.Reward();
                    BattleOverMenu.OpenWindow();
                }
                else
                {
                    BattleManager.Reward(1);
                    BattleOverMenu.CloseWindow();
                }
                for (int i = 0; i < BattleManager.rewards.Count; i++)
                {
                    GUIManager.getInstance().AddTextLine(""+ BattleManager.bonusLoot +" : " + BattleManager.rewards.Count +" : " + BattleManager.rewards.Find(x => x.id == i).type + " : " + +BattleManager.rewards.Find(x => x.id == i).itemid + " : " + BattleManager.rewards.Find(x => x.id == i).amount);
                }


                GUIManager.getInstance().AddTextLine("" + AManager<ResourceManager>.getInstance().getWealth());
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {

                foreach (ALivingEntity entity in UnitManager.getInstance().allUnits)
                {
                    if (WorldManager.getInstance().PlayerFaction.getAlignmentToward(entity.faction) != Alignment.Ally)
                    {
                        entity.hitpoints = 0;
                    }
                }
            }
            */
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (IsOpen())
                {
                    if (openDraft)
                    {
                        openDraft = false;
                        return;
                    }
                    CloseWindow();
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
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