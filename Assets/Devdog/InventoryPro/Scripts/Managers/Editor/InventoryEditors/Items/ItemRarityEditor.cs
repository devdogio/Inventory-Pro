using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using UnityEditor;
using UnityEngine;
using EditorUtility = UnityEditor.EditorUtility;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class ItemRarityEditor : ScriptableObjectEditorCrud<ItemRarity>
    {

        protected override List<ItemRarity> crudList
        {
            get { return new List<ItemRarity>(ItemManager.database.rarities); }
            set { ItemManager.database.rarities = value.ToArray(); }
        }

        public ItemRarityEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        { }

        protected override bool MatchesSearch(ItemRarity item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }


        protected override void GiveItemNewID(ItemRarity item)
        {
            item.ID = crudList.Count > 0 ? crudList.Max(o => o.ID) + 1 : 0;
        }

        public override void RemoveItem(int i)
        {
            //            var l = new List<InventoryItemBase>(ItemManager.database.items);
            var allUsingRarity = ItemManager.database.items.Where(o => o.rarity == crudList[i]).ToArray();
            if (allUsingRarity.Length == 0)
            {
                base.RemoveItem(i);
            }
            else
            {
                var window = ReplaceWithDialog.Get((index, editorWindow) =>
                {
                    if (index == -1)
                    {
                        Debug.Log("Not replacing - Deleting rarity");
                    }
                    else
                    {
                        Debug.Log("Replace rarity with " + ItemManager.database.rarities[index].name);
                        foreach (var item in allUsingRarity)
                        {
                            item.rarity = ItemManager.database.rarities[index];
                            EditorUtility.SetDirty(item);
                        }
                    }

                    base.RemoveItem(i);
                    editorWindow.Close();

                }, "Rarity", allUsingRarity.Length, ItemManager.database.rarityStrings);
                window.Show();
            }
        }


        protected override void DrawSidebarRow(ItemRarity item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(ItemRarity item, int index)
        {
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(item, item.name);

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            EditorGUILayout.LabelField("ID", item.ID.ToString());
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The name of the rarity, is displayed in the tooltip in UI elements.", EditorStyles.infoStyle);

            item.name = EditorGUILayout.DelayedTextField("Name", item.name);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("The color displayed in the UI.", EditorStyles.labelStyle);
            if (item.color.a == 0.0f)
                EditorGUILayout.HelpBox("Color alpha is 0, color is transparent.\nThis might not be intended behavior.", MessageType.Warning);

            item.color = EditorGUILayout.ColorField("Rarity color", item.color);


            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("A custom object used when dropping this item like a pouch or chest.", EditorStyles.infoStyle);
            item.dropObject = (GameObject)EditorGUILayout.ObjectField("Drop object", item.dropObject, typeof(GameObject), false);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


            ValidateItemFromCache(item);

            EditorGUIUtility.labelWidth = 0;
        }
    }
}
