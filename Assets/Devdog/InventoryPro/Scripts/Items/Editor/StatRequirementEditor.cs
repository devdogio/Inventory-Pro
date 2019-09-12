using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [CustomPropertyDrawer(typeof(StatRequirement), true)]
    public partial class StatRequirementEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            InventoryEditorUtility.DrawStatRequirement(rect, property, true, true, true);

            EditorGUI.EndProperty();
        }
    }
}
