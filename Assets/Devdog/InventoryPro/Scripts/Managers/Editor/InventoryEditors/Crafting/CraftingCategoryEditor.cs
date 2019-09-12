using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using UnityEditor;
using UnityEngine;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class CraftingCategoryEditor : ScriptableObjectEditorCrud<CraftingCategory>
    {
        protected override List<CraftingCategory> crudList
        {
            get { return new List<CraftingCategory>(ItemManager.database.craftingCategories); }
            set { ItemManager.database.craftingCategories = value.ToArray(); }
        }

        protected CraftingEmptyEditor parentEditor { get; set; }

        public CraftingCategoryEditor(string singleName, string pluralName, EditorWindow window, CraftingEmptyEditor editor)
            : base(singleName, pluralName, window)
        {
            parentEditor = editor;
            canReOrderItems = true;
        }

        protected override bool MatchesSearch(CraftingCategory item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search) || item.description.ToLower().Contains(search));
        }

        protected override void GiveItemNewID(CraftingCategory item)
        {
            item.ID = crudList.Count > 0 ? crudList.Max(o => o.ID) + 1 : 0;
        }

        public override void AddItem(CraftingCategory item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);

            parentEditor.CreateEditors();
            parentEditor.toolbarIndex = parentEditor.childEditors.Count - 1;

            UnityEditor.EditorUtility.SetDirty(ItemManager.database);
        }

        public override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            parentEditor.CreateEditors();
            parentEditor.toolbarIndex = parentEditor.childEditors.Count - 1;

            UnityEditor.EditorUtility.SetDirty(ItemManager.database);
        }

        protected override void DrawSidebarRow(CraftingCategory item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(CraftingCategory category, int index)
        {
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;
            RenameScriptableObjectIfNeeded(category, category.name);

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            EditorGUILayout.LabelField("Note that this is not used for item categories but rather category types such as Smithing, Tailoring, etc.", EditorStyles.titleStyle);
            EditorGUILayout.Space();

            category.name = EditorGUILayout.DelayedTextField("Category name", category.name);
            category.description = EditorGUILayout.TextField("Category description", category.description);

            EditorGUILayout.Space();
            category.alsoScanBankForRequiredItems = EditorGUILayout.Toggle("Scan bank for craft items", category.alsoScanBankForRequiredItems);
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Audio clips", EditorStyles.titleStyle);
            EditorGUILayout.Space();

            InventoryEditorUtility.AudioClipInfo("Success Audio clip", category.successAudioClip);
            InventoryEditorUtility.AudioClipInfo("Crafting Audio clip", category.craftingAudioClip);
            InventoryEditorUtility.AudioClipInfo("Canceled Audio clip", category.canceledAudioClip);
            InventoryEditorUtility.AudioClipInfo("Failed Audio clip", category.failedAudioClip);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Layout crafting", EditorStyles.titleStyle);
            EditorGUILayout.Space();

            ObjectPickerUtility.RenderObjectPickerForType("Icon", category.icon, typeof (Sprite), val =>
            {
                category.icon = (Sprite)val;
            });

            category.rows = (uint)EditorGUILayout.IntField("Layout rows", (int)category.rows);
            category.cols = (uint)EditorGUILayout.IntField("Layout cols", (int)category.cols);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Category contains " + category.blueprints.Length + " blueprints.", EditorStyles.titleStyle);
            EditorGUILayout.EndVertical();


            ValidateItemFromCache(category);

            EditorGUIUtility.labelWidth = 0;
        }

        public override string ToString()
        {
            return singleName + " editor";
        }
    }
}
