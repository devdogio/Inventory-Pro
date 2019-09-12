using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Devdog.General.Editors;
using Devdog.General.Editors.ReflectionDrawers;
using Devdog.InventoryPro;
using UnityEditor;
using UnityEngine;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class CraftingBlueprintEditor : ScriptableObjectEditorCrud<CraftingBlueprint>
    {
        protected override List<CraftingBlueprint> crudList
        {
            get { return new List<CraftingBlueprint>(category.blueprints); }
            set { category.blueprints = value.ToArray(); }
        }

        public CraftingCategory category { get; protected set; }

        protected Vector2 scrollPos;
//        private readonly EmptyEditor _parentEditor;
        private UnityEditorInternal.ReorderableList _requiredItemsList;
        private UnityEditorInternal.ReorderableList _resultItemsList;
        private UnityEditorInternal.ReorderableList _usageRequirementPropertiesList;


        public CraftingBlueprintEditor(string singleName, string pluralName, EditorWindow window, EmptyEditor parentEditor, CraftingCategory category)
            : base(singleName, pluralName, window)
        {
//            this._parentEditor = parentEditor;

			
            this.category = category;

            forceUpdateIDsWhenOutOfSync = false; // Don't sync ID's are global over all categories.
            canReOrderItems = true;
        }

        public override void EditItem(CraftingBlueprint item, int itemIndex)
        {
            base.EditItem(item, itemIndex);

            _requiredItemsList = new UnityEditorInternal.ReorderableList(item.requiredItems, typeof(ItemAmountRow), true, true, true, true);
            _requiredItemsList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Required items");
            _requiredItemsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;


                var r2 = rect;
                r2.width /= 2;
                r2.width -= 5;

                if (item.requiredItems[index].amount < 1)
                {
                    item.requiredItems[index].SetAmount(1);
                }

                item.requiredItems[index].SetAmount((uint)EditorGUI.IntField(r2, (int)item.requiredItems[index].amount));

                r2.x += r2.width + 5;

                if (item.requiredItems[index].item == null)
                {
                    GUI.backgroundColor = Color.red;
                }

                ObjectPickerUtility.RenderObjectPickerForType<InventoryItemBase>(r2, "", item.requiredItems[index].item,
                    newItem =>
                    {
                        item.requiredItems[index].SetItem(newItem);
                        GUI.changed = true; // To save..
//                        window.Repaint();
                    });

                GUI.backgroundColor = Color.white;
            };
            _requiredItemsList.onAddCallback += list =>
            {
				var l = new List<ItemAmountRow>(item.requiredItems)
				{
					new ItemAmountRow()
				};

				item.requiredItems = l.ToArray();
                list.list = item.requiredItems;

                window.Repaint();
            };
            _requiredItemsList.onRemoveCallback += list =>
            {
                var l = new List<ItemAmountRow>(item.requiredItems);
                l.RemoveAt(list.index);
                item.requiredItems = l.ToArray();
                list.list = item.requiredItems;

                window.Repaint();
            };



            _resultItemsList = new UnityEditorInternal.ReorderableList(item.resultItems, typeof(ItemAmountRow), true, true, true, true);
            _resultItemsList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Result items");
            _resultItemsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;



                var r2 = rect;
                r2.width /= 2;
                r2.width -= 5;

                if (item.resultItems[index].amount < 1)
                {
                    item.resultItems[index].SetAmount(1);
                }

                item.resultItems[index].SetAmount((uint)EditorGUI.IntField(r2, (int)item.resultItems[index].amount));

                r2.x += r2.width + 5;

                if (item.resultItems[index].item == null)
                {
                    GUI.backgroundColor = Color.red;
                }

                ObjectPickerUtility.RenderObjectPickerForType<InventoryItemBase>(r2, "", item.resultItems[index].item,
                    val =>
                    {
                        item.resultItems[index].SetItem(val);
                        GUI.changed = true; // To save..
//                        window.Repaint();
                    });

                GUI.backgroundColor = Color.white;
            };
            _resultItemsList.onAddCallback += list =>
            {
                var l = new List<ItemAmountRow>(item.resultItems);
                l.Add(new ItemAmountRow());
                item.resultItems = l.ToArray();
                list.list = item.resultItems;

                window.Repaint();
            };
            _resultItemsList.onRemoveCallback += list =>
            {
                var l = new List<ItemAmountRow>(item.resultItems);
                l.RemoveAt(list.index);
                item.resultItems = l.ToArray();
                list.list = item.resultItems;

                window.Repaint();
            };


            _usageRequirementPropertiesList = new UnityEditorInternal.ReorderableList(item.usageRequirement, typeof(StatRequirement), true, true, true, true);
            _usageRequirementPropertiesList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Stat requirements");
            _usageRequirementPropertiesList.elementHeight = 42;
            _usageRequirementPropertiesList.drawElementCallback += (rect, index, active, focused) =>
            {

                InventoryEditorUtility.DrawStatRequirement(rect, item.usageRequirement[index], active, focused, true);
            };
            _usageRequirementPropertiesList.onAddCallback += list =>
            {
                var l = new List<StatRequirement>(item.usageRequirement);
                l.Add(new StatRequirement());
                item.usageRequirement = l.ToArray();
                list.list = item.usageRequirement;

                window.Repaint();
            };
            _usageRequirementPropertiesList.onRemoveCallback += list =>
            {
                var l = new List<StatRequirement>(item.usageRequirement);
                l.RemoveAt(list.index);
                item.usageRequirement = l.ToArray();
                list.list = item.usageRequirement;

                window.Repaint();
            };
        }

        protected override bool MatchesSearch(CraftingBlueprint item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search) || item.description.ToLower().Contains(search));
        }

        protected override void GiveItemNewID(CraftingBlueprint item)
        {
            item.ID = ItemManager.database.craftingCategories.Length > 0 ? ItemManager.database.craftingCategories.Max(o => o.ID) + 1 : 0;
        }

        public override void AddItem(CraftingBlueprint item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);

            UnityEditor.EditorUtility.SetDirty(category);
        }

        public override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            UnityEditor.EditorUtility.SetDirty(category);
        }

        protected override void DrawSidebarRow(CraftingBlueprint item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override bool ValidateItem(CraftingBlueprint blueprint, DrawerBase drawer, bool refreshValues = false, bool isRoot = true)
        {
            var isValid = base.ValidateItem(blueprint, drawer, refreshValues, isRoot);
            if (isValid == false)
            {
                if (blueprint.requiredItems.Any(item => item.item == null) || 
                    blueprint.resultItems.Any(item => item.item == null) ||
                    blueprint.resultItems.Length == 0)
                {
                    itemsErrorLookup[blueprint].message += drawer.fieldName.text + " is empty" + '\n';
                    itemsErrorLookup[blueprint].drawers.Add(drawer);
                    return false;
                }
            }

            return isValid;
        }

        protected override void DrawDetail(CraftingBlueprint selectedBlueprint, int index)
        {
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(selectedBlueprint, selectedBlueprint.ID + "_" + selectedBlueprint.name.Replace(",", "_").Replace(" ", "_"));

            #region About craft

            EditorGUILayout.LabelField("Step 1. What are we crafting?", EditorStyles.titleStyle);


            var itemRow = selectedBlueprint.resultItems.FirstOrDefault();
            string name = "";
            string desc = "";
            string cat = "";
            if (itemRow.item != null)
            {
                name = itemRow.item.name;
                desc = itemRow.item.description;
                cat = itemRow.item.categoryName;
            }

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            selectedBlueprint.useItemResultNameAndDescription = EditorGUILayout.Toggle("Use result item's name", selectedBlueprint.useItemResultNameAndDescription);
            if (selectedBlueprint.useItemResultNameAndDescription == false)
            {
                selectedBlueprint.customName = EditorGUILayout.DelayedTextField("Blueprint name", selectedBlueprint.customName);
                selectedBlueprint.customDescription = EditorGUILayout.TextField("Blueprint description", selectedBlueprint.customDescription);
                GUI.enabled = false;

                EditorGUILayout.TextField("Category", cat);
            }
            else
            {
                GUI.enabled = false;


                EditorGUILayout.DelayedTextField("Blueprint name", name);
                EditorGUILayout.TextField("Blueprint description", desc);
                EditorGUILayout.TextField("Category", cat);
            }
            GUI.enabled = true;

            EditorGUILayout.EndVertical();

            #endregion


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region Crafting process

            EditorGUILayout.LabelField("Step 2. How are we crafting it?", EditorStyles.titleStyle);

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            selectedBlueprint.playerLearnedBlueprint = EditorGUILayout.Toggle("Player learned blueprint", selectedBlueprint.playerLearnedBlueprint);
            selectedBlueprint.successChanceFactor = EditorGUILayout.Slider("Chance factor", selectedBlueprint.successChanceFactor, 0.0f, 1.0f);
            selectedBlueprint.craftingTimeDuration = EditorGUILayout.FloatField("Crafting time duration (seconds)", selectedBlueprint.craftingTimeDuration);
            selectedBlueprint.craftingTimeSpeedupFactor = EditorGUILayout.FloatField("Speedup factor", selectedBlueprint.craftingTimeSpeedupFactor);
            selectedBlueprint.craftingTimeSpeedupMax = EditorGUILayout.FloatField("Max speedup", selectedBlueprint.craftingTimeSpeedupMax);



            if (selectedBlueprint.craftingTimeSpeedupFactor != 1.0f)
            {
                EditorGUILayout.Space();

                for (int i = 1; i < 16; i++)
                {
                    float f = Mathf.Clamp(Mathf.Pow(selectedBlueprint.craftingTimeSpeedupFactor, i * 5), 0.0f, selectedBlueprint.craftingTimeSpeedupMax);

                    if (f != selectedBlueprint.craftingTimeSpeedupMax)
                        EditorGUILayout.LabelField("Speedup after \t" + (i * 5) + " crafts \t" + System.Math.Round(f, 2) + "x \t(" + System.Math.Round(selectedBlueprint.craftingTimeDuration / f, 2) + "s per item)");
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Reached max after " + 1.0f / Mathf.Log(selectedBlueprint.craftingTimeSpeedupFactor, selectedBlueprint.craftingTimeSpeedupMax) + " crafts");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();



            EditorGUILayout.LabelField("Step 3. What items does the user need? (Ignore if using layouts)", EditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            if (selectedBlueprint.craftingCost != null)
                InventoryEditorUtility.CurrencyDecorator("Crafting cost", selectedBlueprint.craftingCost);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _requiredItemsList.DoLayoutList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _usageRequirementPropertiesList.DoLayoutList();

            #endregion


            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            #region Craft result

            EditorGUILayout.LabelField("Step 4. What's the result?", EditorStyles.titleStyle);

            EditorGUILayout.BeginVertical();
            _resultItemsList.DoLayoutList();
            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            #region Layout editor

            EditorGUILayout.LabelField("Step 5. (optional) Define the layouts to use", EditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            int counter = 0;
            foreach (var l in selectedBlueprint.blueprintLayouts)
            {
                EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
                EditorGUILayout.BeginHorizontal();

                l.enabled = EditorGUILayout.BeginToggleGroup("Layout #" + l.ID + "-" + (l.enabled ? "(enabled)" : "(disabled)"), l.enabled);
                EditorGUILayout.BeginHorizontal();

                GUI.color = Color.red;
                if (GUILayout.Button("Delete"))
                {
                    var t = new List<CraftingBlueprintLayout>(selectedBlueprint.blueprintLayouts);
                    t.RemoveAt(counter);
                    selectedBlueprint.blueprintLayouts = t.ToArray();

                    AssetDatabase.SaveAssets();
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                //EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginVertical();
                if (l.enabled)
                {
                    foreach (var r in l.rows)
                    {
                        EditorGUILayout.BeginHorizontal();
                        foreach (var c in r.columns)
                        {
                            if (c.item != null)
                                GUI.color = Color.green;

                            EditorGUILayout.BeginVertical("box", GUILayout.Width(80), GUILayout.Height(80));

                            EditorGUILayout.LabelField((c.item != null) ? c.item.name : string.Empty, EditorStyles.labelStyle);
                            c.amount = EditorGUILayout.IntField(c.amount);

                            if (GUILayout.Button("Set", GUILayout.Width(80)))
                            {
                                var clickedItem = c;
                                ObjectPickerUtility.GetObjectPickerForType<InventoryItemBase>(item =>
                                {
                                    clickedItem.item = item;
                                    clickedItem.amount = 1;
                                    GUI.changed = true;
                                    window.Repaint();
                                });

                                //layoutObjectPickerSetFor = c;
                                //EditorGUIUtility.ShowObjectPicker<UnityEngine.Object>(null, false, "l:InventoryItemPrefab", 61);
                            }
                            if (GUILayout.Button("Clear", UnityEditor.EditorStyles.miniButton))
                            {
                                c.amount = 0;
                                c.item = null;
                            }

                            EditorGUILayout.EndVertical();

                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                counter++;
            }


            if (GUILayout.Button("Add layout"))
            {
                var l = new List<CraftingBlueprintLayout>(selectedBlueprint.blueprintLayouts);
                var obj = new CraftingBlueprintLayout();

                obj.ID = l.Count;
                obj.rows = new CraftingBlueprintLayout.Row[category.rows];
                for (int i = 0; i < obj.rows.Length; i++)
                {
                    obj.rows[i] = new CraftingBlueprintLayout.Row();
                    obj.rows[i].index = i;
                    obj.rows[i].columns = new CraftingBlueprintLayout.Row.Cell[category.cols];

                    for (int j = 0; j < obj.rows[i].columns.Length; j++)
                    {
                        obj.rows[i].columns[j] = new CraftingBlueprintLayout.Row.Cell();
                        obj.rows[i].columns[j].index = j;
                    }
                }

                l.Add(obj);
                selectedBlueprint.blueprintLayouts = l.ToArray();
            }

            EditorGUILayout.EndVertical();
            #endregion


            GUI.enabled = true; // From layouts
            EditorGUIUtility.labelWidth = 0;
        }

        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {
            
        }
    }
}
