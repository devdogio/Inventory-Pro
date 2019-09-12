using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [CustomPropertyDrawer(typeof(ItemAmountRow), true)]
    public partial class ItemAmountRowEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            InventoryEditorUtility.DrawItemAmountRow(rect, property);

            EditorGUI.EndProperty();
        }
    }
}
