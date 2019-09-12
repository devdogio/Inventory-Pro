using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EditorUtility = UnityEditor.EditorUtility;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class ItemStatEditor : ScriptableObjectEditorCrud<StatDefinition>
    {
        protected enum LevelEditorProgressionStyle
        {
            Linear,
            EaseIn,
//            EaseOut,
//            EaseInOut
        }



        protected LevelEditorProgressionStyle selectedProgressStyle;
        private bool _isRightMouseButtonDown;
        private bool _isLeftMouseButtonDown;
        private int _startValue = 0;
        private int _endValue = 100;
        private int _showOption;
        private StatLevel _selectedLevel;

        protected override List<StatDefinition> crudList
        {
            get { return new List<StatDefinition>(ItemManager.database.statDefinitions); }
            set { ItemManager.database.statDefinitions = value.ToArray(); }
        }

        public ItemStatEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            window.wantsMouseMove = true;
        }

        protected override bool MatchesSearch(StatDefinition item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.statName.ToLower().Contains(search));
        }

        protected override void GiveItemNewID(StatDefinition item)
        {
            item.ID = crudList.Count > 0 ? crudList.Max(o => o.ID) + 1 : 0;
        }

        public override void RemoveItem(int i)
        {
            var allUsingStat = ItemManager.database.items.Where(o => o.stats.Any(s => s.stat == crudList[i])).ToArray();
            foreach (var item in allUsingStat)
            {
                var l = item.stats.ToList();
                l.RemoveAll(o => o.stat.ID == crudList[i].ID);
                item.stats = l.ToArray();

                EditorUtility.SetDirty(item);
            }

            base.RemoveItem(i);
        }

        protected override void DrawSidebarRow(StatDefinition item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.statName, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(StatDefinition statDef, int index)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                    _isLeftMouseButtonDown = true;

                if (Event.current.button == 1)
                    _isRightMouseButtonDown = true;

            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                    _isLeftMouseButtonDown = false;

                if (Event.current.button == 1)
                    _isRightMouseButtonDown = false;
            }

            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(statDef, statDef.category + "_" + statDef.statName);

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            EditorGUILayout.LabelField("ID", statDef.ID.ToString());
            EditorGUILayout.Space();

            statDef.enabled = EditorGUILayout.Toggle("Enabled", statDef.enabled);
            EditorGUILayout.Space();

            statDef.category = EditorGUILayout.DelayedTextField("Category", statDef.category);
            statDef.statName = EditorGUILayout.DelayedTextField("Name", statDef.statName);
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            using (new Devdog.General.Editors.ColorBlock(Color.grey, statDef.enabled == false))
            {

                statDef.showInUI = EditorGUILayout.Toggle("Show in UI", statDef.showInUI);
                if (statDef.showInUI)
                {
                    statDef.color = EditorGUILayout.ColorField("UI Color", statDef.color);
                    if (statDef.color.a == 0.0f)
                    {
                        EditorGUILayout.HelpBox("Color alpha is 0, color is transparent.\nThis might not be intended behavior.", MessageType.Warning);
                    }

                    statDef.icon = (Sprite)EditorGUILayout.ObjectField("Icon", statDef.icon, typeof (Sprite), false);
                    if (statDef.uiPrefab == null)
                    {
                        GUI.color = Color.red;
                    }
                    ObjectPickerUtility.RenderObjectPickerForType("UI Prefab", statDef.uiPrefab, typeof(StatRowUI), val =>
                    {
                        statDef.uiPrefab = (StatRowUI)val;
                    });
                    GUI.color = Color.white;

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    GUI.color = Color.yellow;
                    EditorGUILayout.LabelField("You can use string.Format elements to define the text formatting of the stat: ");
                    EditorGUILayout.LabelField("{0} = The current amount");
                    EditorGUILayout.LabelField("{1} = The max amount");
                    EditorGUILayout.LabelField("{2} = The stat name");
                    EditorGUILayout.LabelField("{3} = The stat level");
                    EditorGUILayout.LabelField("{4} = The stat experience");
                    EditorGUILayout.LabelField("{5} = The stat required experience to next level (empty if last level)");
                    GUI.color = Color.white;
                    statDef.valueStringFormat = EditorGUILayout.TextField("Value string format", statDef.valueStringFormat);

                    EditorGUILayout.LabelField("Format example: ", statDef.ToString(5.0f));
                    EditorGUILayout.LabelField("Format example: ", statDef.ToString(100.0f));
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();


                statDef.baseValue = EditorGUILayout.FloatField("Base (start) value", statDef.baseValue);
                statDef.maxValue = EditorGUILayout.FloatField("Max value", statDef.maxValue);
                if (statDef.baseValue > statDef.maxValue)
                {
                    statDef.baseValue = statDef.maxValue;
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Levels", EditorStyles.titleStyle);

                statDef.startLevel = EditorGUILayout.IntField("Start level", statDef.startLevel);
                statDef.autoProgressLevels = EditorGUILayout.Toggle("Auto progress levels", statDef.autoProgressLevels);

                _showOption = GUILayout.Toolbar(_showOption, new string[] { "Max values", "Experience required" });
                var currentLevelIndex = statDef.levels.ToList().IndexOf(_selectedLevel);

                float height = 300f;
                float width = 550f;
                if (_selectedLevel != null)
                {
                    GUILayout.Label("Level options: ", Devdog.General.Editors.EditorStyles.titleStyle);
                    GUILayout.Label("Level: " + (currentLevelIndex + 1));

                    GUILayout.Space(10f);

                    _selectedLevel.maxValue = EditorGUILayout.FloatField("Max value", _selectedLevel.maxValue);
                    _selectedLevel.experienceRequired = EditorGUILayout.FloatField("Experience requierd", _selectedLevel.experienceRequired);
                    _selectedLevel.effectOnUnlock = (GameObject)EditorGUILayout.ObjectField("Unlock effect prefab", _selectedLevel.effectOnUnlock, typeof(UnityEngine.GameObject), false);

                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete selected level (" + (currentLevelIndex + 1) + ")"))
                    {
                        var l = statDef.levels.ToList();
                        if (l.Count > 0)
                        {
                            l.RemoveAt(currentLevelIndex);
                        }

                        statDef.levels = l.ToArray();
                        _selectedLevel = null;
                        window.Repaint();
                    }
                    GUI.color = Color.white;
                }

                GUI.BeginGroup(GUILayoutUtility.GetRect(width, width, height + 50f, height + 50f));
                
                float max = 0f;
                if (_showOption == 0)
                {
                    max = statDef.levels.Length > 0 ? statDef.levels.Max(o => o.maxValue) : 1f;
                }
                else if (_showOption == 1)
                {
                    max = Mathf.Max(max, statDef.levels.Length > 0 ? statDef.levels.Max(o => o.experienceRequired) : 1f);
                }

                if (max <= 0f)
                {
                    max = height;
                }

                float pixelsPerUnit = height / max;
                var textStyle = new GUIStyle("PreVerticalScrollbar");
                textStyle.alignment = TextAnchor.MiddleCenter;
                for (int i = 0; i < statDef.levels.Length; i++)
                {
                    var elementWidth = width/statDef.levels.Length;
                    var maxValueHeight = statDef.levels[i].maxValue;
                    var experienceRequiredHeight = statDef.levels[i].experienceRequired;

                    var fullRect = new Rect(elementWidth*i, 0f, elementWidth, height);
                    if (fullRect.Contains(Event.current.mousePosition))
                    {
                        if (_isLeftMouseButtonDown)
                        {
                            _selectedLevel = statDef.levels[i];
                        }
                        else if (_isRightMouseButtonDown)
                        {
                            var offsetFromBottom = Event.current.mousePosition - fullRect.position;
                            Set(statDef.levels[i], (height - offsetFromBottom.y) / pixelsPerUnit);

                            window.Repaint();
                        }
                    }

                    if (_selectedLevel == statDef.levels[i])
                    {
                        GUI.color = Color.cyan;
                    }

                    int val = 0;
                    if (_showOption == 0)
                    {
                        GUI.Label(new Rect(elementWidth * i, height - (maxValueHeight * pixelsPerUnit), elementWidth, maxValueHeight * pixelsPerUnit), GUIContent.none, "CN Box");
                        val = Mathf.RoundToInt(statDef.levels[i].maxValue);
                    }
                    else if (_showOption == 1)
                    {
                        GUI.Label(new Rect(elementWidth * i, height - (experienceRequiredHeight * pixelsPerUnit), elementWidth, experienceRequiredHeight * pixelsPerUnit), GUIContent.none, "CN Box");
                        val = Mathf.RoundToInt(statDef.levels[i].experienceRequired);
                    }

                    GUI.color = Color.white;

                    fullRect.x += 5;
                    GUI.Label(fullRect, new GUIContent(val.ToString()), textStyle);
                }

                GUI.EndGroup();

                if (GUILayout.Button("Add level"))
                {
                    var l = statDef.levels.ToList();
                    l.Add(new StatLevel()
                    {
                        maxValue = statDef.maxValue
                    });
                    statDef.levels = l.ToArray();
                }

                if (GUILayout.Button("Remove (last) level"))
                {
                    var l = statDef.levels.ToList();
                    if (l.Count > 0)
                    {
                        l.RemoveAt(l.Count - 1);
                    }

                    statDef.levels = l.ToArray();
                }

                GUILayout.Label("Editor tools", EditorStyles.titleStyle);
                selectedProgressStyle = (LevelEditorProgressionStyle)EditorGUILayout.EnumPopup("Generate style", selectedProgressStyle);
                _startValue = EditorGUILayout.IntField("Start value", _startValue);
                _endValue = EditorGUILayout.IntField("End value", _endValue);

                if (GUILayout.Button("Generate"))
                {
                    switch (selectedProgressStyle)
                    {
                        case LevelEditorProgressionStyle.Linear:
                            {
                                var stepSize = (float)_endValue / statDef.levels.Length;
                                for (int i = 0; i < statDef.levels.Length; i++)
                                {
                                    Set(statDef.levels[i], _startValue + stepSize*(i + 1));
                                }

                                break;
                            }
                        case LevelEditorProgressionStyle.EaseIn:
                            {
                                if (statDef.levels.Length > 0)
                                {
                                    Set(statDef.levels[0], 0f);
                                }

                                for (int i = 1; i < statDef.levels.Length; i++)
                                {
                                    float t = (float) (i + 1) / statDef.levels.Length;
                                    Set(statDef.levels[i], _endValue * t * t + _startValue);
                                }

                                break;
                            }
//                        case LevelEditorProgressionStyle.EaseOut:
//                            {
//                                break;
//                            }
//                        case LevelEditorProgressionStyle.EaseInOut:
//                            {
//                                break;
//                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    window.Repaint();
                }
            }


            EditorGUILayout.EndVertical();

            ValidateItemFromCache(statDef);

            EditorGUIUtility.labelWidth = 0;
        }

        private void Set(StatLevel statLevel, float f)
        {
            if (_showOption == 0)
            {
                statLevel.maxValue = f;
            }
            else if (_showOption == 1)
            {
                statLevel.experienceRequired = f;
            }
        }
    }
}
