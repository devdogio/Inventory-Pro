using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.General.Editors;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{

    [CustomPropertyDrawer(typeof(CurrencyDecorator), true)]
    public class CurrencyDecoratorEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var amount = property.FindPropertyRelative("_amount");
            var currency = property.FindPropertyRelative("_currency");

            var w3 = position.width / 3f;
            position.width = w3 * 2;

            using (new ColorBlock(Color.red, amount.floatValue > 0f && currency.objectReferenceValue == null))
            {
                ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>(position, label.text, currency);
            }

            position.x += position.width + 10;
            position.width = w3;

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.PropertyField(position, amount);
            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (amount.floatValue < 0f)
            {
                amount.floatValue = 0f;
            }

            EditorGUI.EndProperty();
        }
    }
}