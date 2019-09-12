using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{

    [CustomPropertyDrawer(typeof(StatDecorator), true)]
    public class StatDecoratorEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            InventoryEditorUtility.DrawStatDecorator(rect, property, true, true, true, true);

            EditorGUI.EndProperty();
        }
    }
}