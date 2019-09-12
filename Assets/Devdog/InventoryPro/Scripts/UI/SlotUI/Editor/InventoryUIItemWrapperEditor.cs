using UnityEngine;
using System.Collections;
using System;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEditor;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(ItemCollectionSlotUIBase), true)]
    public partial class InventoryUIItemWrapperEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            if (EditorApplication.isPlaying)
            {
                var t = (ItemCollectionSlotUIBase) target;

                EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

                EditorGUILayout.LabelField("Run-time info", EditorStyles.titleStyle);

                EditorGUILayout.LabelField("Item: " + ((t.item != null) ? t.item.name : "(empty)"));
                EditorGUILayout.LabelField("Stacksize: " + ((t.item != null) ? t.item.currentStackSize.ToString() : "-"));
                EditorGUILayout.LabelField("Size: " + ((t.item != null) ? t.item.layoutSizeCols + " X " + t.item.layoutSizeRows : "-"));

                EditorGUILayout.EndVertical();
            }

            DrawDefaultInspector();
            
        }
    }
}