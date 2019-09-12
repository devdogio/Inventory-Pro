using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{

    [CustomPropertyDrawer(typeof(InventoryItemFilters))]
    public class InventoryItemFiltersEditor : PropertyDrawer
    {
        
        protected UnityEditorInternal.ReorderableList list { get; set; }
        protected int lastLength { get; set; }


        public InventoryItemFiltersEditor()
        {
            lastLength = -1;
        }


        protected virtual void TryCreateList(SerializedProperty property)
        {
            if (property.FindPropertyRelative("filters").arraySize != lastLength)
            {
                CreateList(property);
            }
        }

        protected virtual void CreateList(SerializedProperty property)
        {
            property = property.FindPropertyRelative("filters");

            list = new UnityEditorInternal.ReorderableList(property.serializedObject, property, true, true, true, true);
            list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Inventory Item filters (items have to abide to these rules)");

            var p = property; // Capture list
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                var r = new ItemFilterEditor();
                rect.height = 18;
                rect.y += 2;
                r.OnGUI(rect, p.GetArrayElementAtIndex(index), new GUIContent(""));
            };

            lastLength = (property != null) ? property.arraySize : 0;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("filters").arraySize == 0)
                return 61;

            return ((EditorGUIUtility.singleLineHeight + 5) * property.FindPropertyRelative("filters").arraySize) + 40;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TryCreateList(property);
            
            EditorGUI.BeginProperty(position, label, property);

            list.DoList(position);

            var p = position;
            p.y += p.height - 18;
            p.height = EditorGUIUtility.singleLineHeight;
            p.width -= 60;

            EditorGUI.PropertyField(p, property.FindPropertyRelative("matchType"));

            EditorGUI.EndProperty();
        }
    }
}