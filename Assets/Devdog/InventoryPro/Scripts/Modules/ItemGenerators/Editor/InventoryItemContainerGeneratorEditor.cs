using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{
    [CustomPropertyDrawer(typeof(InventoryItemContainerGenerator))]
    public class InventoryItemContainerGeneratorEditor : PropertyDrawer
    {       

        private const int ExtraHeight = 22;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + ExtraHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var filters = property.FindPropertyRelative("filterGroups");
                       
            var minField = filters.FindPropertyRelative("minAmountTotal");
            var maxField = filters.FindPropertyRelative("maxAmountTotal");

            var p = position;
            p.y += ExtraHeight;
            p.height = EditorGUIUtility.singleLineHeight;

            var p2 = p;
            p2.y += 2;
            p2.width = 40;
            EditorGUI.LabelField(p2, minField.intValue.ToString());            

            p.width -= ((p2.width * 2) + 10);
            p.x += p2.width + 5;

            float min = minField.intValue;
            float max = maxField.intValue;

            EditorGUI.MinMaxSlider(p, ref min, ref max, 0, 25);

            minField.intValue = (int)min;
            maxField.intValue = (int)max;


            p2.x += p.width + 5 + p2.width;
            EditorGUI.LabelField(p2, maxField.intValue.ToString());            



        }
    }
}