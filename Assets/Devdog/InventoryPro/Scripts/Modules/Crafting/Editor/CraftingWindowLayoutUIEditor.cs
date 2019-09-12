using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.General.Editors;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(CraftingWindowLayoutUI), true)]
    public class CraftingWindowLayoutUIEditor : InventoryEditorBase
    {
        private SerializedProperty _startCraftingCategory;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            _startCraftingCategory = serializedObject.FindProperty("_startCraftingCategory");
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraSpecific)
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(script);

            GUILayout.Label("Behavior", EditorStyles.titleStyle);
            ObjectPickerUtility.RenderObjectPickerForType<CraftingCategory>(_startCraftingCategory);

            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                "_startCraftingCategoryID",
            });

            serializedObject.ApplyModifiedProperties();
        }
    }
}