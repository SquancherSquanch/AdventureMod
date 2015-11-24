using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.Profession.Undead;
using System.Collections.Generic;
using Timber_and_Stone.API.Event;
using System.Linq;

namespace Plugin.Squancher.AdventureMod
{
    public class DrafteesMenu : MonoBehaviour
    {
        private Vector2? lastPivot;
        private static bool _open, bShowMilitary, bShowCivilian;
        public Rect windowRect;
        private float intendedWindowWidth = 490f;
        private Vector2 horizontalScrollPosition;
        public Rect windowViewRect;
        private Vector2 scrollPosition;

        public DrafteesMenu()
        {
        }

        public void Start()
        {
            bShowMilitary = false;
            bShowCivilian = false;
            windowRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            windowViewRect = new Rect(0f, 0f, this.intendedWindowWidth, 320f);
            scrollPosition = Vector2.zero;
            horizontalScrollPosition = Vector2.zero;
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

        public static void OpenWindow()
        {
            _open = true;
        }

        public static void CloseWindow()
        {
            _open = false;
        }

        public static bool IsOpen()
        {
            return _open;
        }

        public void RenderWindow(int windowID)
        {
            if (bShowCivilian == true)
            {
                bShowMilitary = false;
            }

            if (bShowMilitary == true)
            {
                bShowCivilian = false;
            }

            float num = 25f;
            float num2 = 9f;

            Rect location = new Rect(0f, 0f, this.windowRect.width, this.windowRect.height);
            /*if (location.Contains(Event.current.mousePosition))
            {
                GUIManager.getInstance().mouseInGUI = true;
            }*/
            GUIManager.getInstance().DrawWindow(new Rect(0, 0f, this.windowRect.width, this.windowRect.height), "Draftees", false);
            if (GUI.Button(new Rect(location.xMax - 24f, location.yMin + 4f, 20f, 20f), string.Empty, GUIManager.getInstance().closeWindowButtonStyle))
            {
                CloseWindow();
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
            num3 = 5f;
            APlayableEntity[] array = Enumerable.ToArray<APlayableEntity>(Enumerable.Where<APlayableEntity>(Enumerable.OfType<APlayableEntity>(AManager<WorldManager>.getInstance().controllerObj.GetComponent<ControlPlayer>().units), (APlayableEntity unit) => unit.isAlive()));
            PartyManager.getInstance().SortUnits(array);
            Rect rect = new Rect(num3, 0f + num4 - 50, 165f, 10f);
            GUIManager.getInstance().DrawCheckBox(new Rect(rect.x, rect.y + 40f, 200f, 24f), "Sort by military", ref bShowMilitary);
            GUIManager.getInstance().DrawCheckBox(new Rect(rect.x, rect.y + 20f, 200f, 24f), "Sort by civilian", ref bShowCivilian);

            GUIManager.getInstance().DrawLineBlack(new Rect(num2, num4 + 10f, this.windowRect.width, 2f));
            num3 = 2f;
            num4 = 123f;
            if (this.windowRect.width >= this.intendedWindowWidth)
            {
                num4 += 22f;
            }
            Rect position2 = new Rect(num3 + 300f, num4 - 26f, this.windowViewRect.width - num3 - 25f, this.windowViewRect.height - num4 + 6f);
            if (this.windowRect.width >= this.intendedWindowWidth)
            {
                position2.height += 37f;
            }
            Rect viewRect = new Rect(num3, num4, this.windowViewRect.width - num3 - num, (float)(array.Length * 32));
            this.scrollPosition = GUI.BeginScrollView(position2, this.scrollPosition, viewRect);
            num4 += 5f;
            num3 = 2f;
            num4 = 123f;
            int sizeincrease = 0;

            for (int i = 0; i < array.Length; i++)
            {
                APlayableEntity aPlayableEntity = array[i];
                if (aPlayableEntity.isAlive())
                {
                    if (PartyManager.draftees.Exists(x => x.uName == aPlayableEntity.unitName))
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

                                sizeincrease++;
                                Rect location8 = new Rect(0f + num3 + 35f, 0f + num4 - 45f, 250f, 25f);
                                Rect position4 = new Rect(0f + num3 + 5f, 0f + num4 - 45f, 22f, 22f);
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
                }
            }
        }

        public void OnGUI()
        {
            if (!GUIManager.getInstance().inGame)
            {
                return;
            }
            this.windowRect.width = Mathf.Min(this.intendedWindowWidth, (float)(Screen.width - 4));
            this.windowRect = GUI.Window(197, this.windowRect, new GUI.WindowFunction(this.RenderWindow), string.Empty, GUIManager.getInstance().hiddenButtonStyle);
            //GUI.FocusWindow(197);
            this.windowRect.x = Mathf.Clamp(this.windowRect.x, 2f, (float)Screen.width - this.windowRect.width - 2f);
            this.windowRect.y = Mathf.Clamp(this.windowRect.y, 40f, (float)Screen.height - this.windowRect.height - 2f);
        }

        public void Update()
        {
        }

    }
}