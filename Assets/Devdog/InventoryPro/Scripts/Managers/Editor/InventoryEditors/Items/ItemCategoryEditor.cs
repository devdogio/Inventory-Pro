using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using UnityEditor;
using UnityEngine;
using EditorUtility = UnityEditor.EditorUtility;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class ItemCategoryEditor : ScriptableObjectEditorCrud<ItemCategory>
    {
        protected override List<ItemCategory> crudList
        {
            get { return new List<ItemCategory>(ItemManager.database.categories); }
            set { ItemManager.database.categories = value.ToArray(); }
        }

        public Editor itemEditorInspector;




        public ItemCategoryEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            
        }

        protected override bool MatchesSearch(ItemCategory item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }

        protected override void GiveItemNewID(ItemCategory item)
        {
            item.ID = crudList.Count > 0 ? crudList.Max(o => o.ID) + 1 : 0;
        }

        public override void RemoveItem(int i)
        {
//            var l = new List<InventoryItemBase>(ItemManager.database.items);
            var allUsingCategory = ItemManager.database.items.Where(o => o.category == crudList[i]).ToArray();

            if (allUsingCategory.Length == 0)
            {
                base.RemoveItem(i);
            }
            else
            {
                var window = ReplaceWithDialog.Get((index, editorWindow) =>
                {
                    if (index == -1)
                    {
                        Debug.Log("Not replacing - Deleting category");
                    }
                    else
                    {
                        Debug.Log("Replace category with " + ItemManager.database.categories[index].name);
                        foreach (var item in allUsingCategory)
                        {
                            item.category = ItemManager.database.categories[index];
                            EditorUtility.SetDirty(item);
                        }
                    }

                    base.RemoveItem(i);
                    editorWindow.Close();

                }, "Category", allUsingCategory.Length, ItemManager.database.itemCategoriesStrings);
                window.Show();
            }
        }

        protected override void DrawSidebarRow(ItemCategory item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(ItemCategory item, int index)
        {
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(item, item.name);

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            EditorGUILayout.LabelField("ID", item.ID.ToString());
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The name of the category, is displayed in the tooltip in UI elements.", EditorStyles.labelStyle);
            item.name = EditorGUILayout.DelayedTextField("Name", item.name);
            ObjectPickerUtility.RenderObjectPickerForType("Icon", item.icon, typeof(Sprite), val =>
            {
                item.icon = (Sprite)val;
            });

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Items can have a 'global' cooldown. Whenever an item of this category is used, all items with the same category will go into cooldown.", EditorStyles.labelStyle);
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("Note, that items can individually override the timeout.", EditorStyles.labelStyle);
            GUI.color = Color.white;
            item.cooldownTime = EditorGUILayout.Slider("Cooldown time (seconds)", item.cooldownTime, 0.0f, 999.0f);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


            ValidateItemFromCache(item);

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
