using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.General.Editors;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(CraftingStandardTrigger), true)]
    public class CraftingStandardTriggerEditor : InventoryEditorBase
    {
        //private CraftingStation item;
        private SerializedProperty _craftingCategory;
    

        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            //item = (CraftingStation)target;
            _craftingCategory = serializedObject.FindProperty("_craftingCategory");
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            serializedObject.Update();

            // Draws remaining items
            EditorGUILayout.BeginVertical();
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                _craftingCategory.name,
            });
            
            ObjectPickerUtility.RenderObjectPickerForType<CraftingCategory>(_craftingCategory);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}