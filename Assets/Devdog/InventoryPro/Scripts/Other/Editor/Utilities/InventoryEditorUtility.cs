using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using Devdog.General;
using Devdog.General.Editors;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    using Devdog.InventoryPro;

    using Object = UnityEngine.Object;

    public static class InventoryEditorUtility
    {
        [Obsolete("Use ReflectionUtility instead", true)]
        public static void GetAllFieldsInherited(Type startType, List<FieldInfo> appendList)
        { }

        public static void CurrencyDecorator(string name, CurrencyDecorator currencyDecorator)
        {
            EditorGUILayout.BeginHorizontal();

            if (currencyDecorator.amount > 0f && currencyDecorator.currency == null)
            {
                GUI.color = Color.red;
            }
            ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>(string.Empty, currencyDecorator.currency, (val) =>
            {
                currencyDecorator.currency = val;
            });

            GUI.color = Color.white;
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            currencyDecorator.amount = EditorGUILayout.FloatField("Amount", currencyDecorator.amount);
            EditorGUIUtility.labelWidth = prevLabelWidth;

            EditorGUILayout.EndHorizontal();
        }

        private static bool IsStatValueValid(object val)
        {
            float singleVal;
            bool isSingle = Single.TryParse(val.ToString(), out singleVal);
            if (isSingle)
            {
                float floatVal = float.Parse(val.ToString());
                return floatVal >= 0f;
            }
            
            return true;
        }

        public static void DrawStatDecorator(Rect rect, SerializedProperty stat, bool isActive, bool isFocused, bool drawRestore, bool drawPercentage)
        {
            rect.height = 16;
            rect.y += 2;

            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            var prop = stat.FindPropertyRelative("_stat");
            var propIsFactor = stat.FindPropertyRelative("isFactor");
            var propValue = stat.FindPropertyRelative("value");
            var propEffect = stat.FindPropertyRelative("actionEffect");

            ObjectPickerUtility.RenderObjectPickerForType<StatDefinition>(popupRect, string.Empty, prop);

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            if (propIsFactor.boolValue)
            {
                popupRect.width -= 65;

                propValue.stringValue = EditorGUI.TextField(popupRect, "", propValue.stringValue);

                var p = popupRect;
                p.x += popupRect.width + 5;
                p.width = 60;

                float floatVal;
                float.TryParse(propValue.stringValue, out floatVal);

                EditorGUI.LabelField(p, "(" + (floatVal - 1.0f) * 100.0f + "%)");
            }
            else
            {
                propValue.stringValue = EditorGUI.TextField(popupRect, "", propValue.stringValue);
            }

            if (drawRestore)
            {
                var r3 = r2;
                r3.width /= 2;
                r3.width -= 5;
                EditorGUI.LabelField(r3, "Action effect");

                r3.x += r3.width + 5;
                EditorGUI.PropertyField(r3, propEffect, new GUIContent(""));

                r2.x += r2.width + 5;
            }

            if (drawPercentage)
            {
                propIsFactor.boolValue = EditorGUI.Toggle(r2, "Is factor (multiplier 0...1)", propIsFactor.boolValue);
                r2.x += r2.width + 5;
            }


            GUI.enabled = true;
        }

        public static void DrawStatRequirement(Rect rect, SerializedProperty statLookup, bool isActive, bool isFocused, bool drawFilterType)
        {
            rect.height = 16;
            rect.y += 2;
            
            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            var stat = statLookup.FindPropertyRelative("stat");
            var statValue = statLookup.FindPropertyRelative("value");
            var statValueType = statLookup.FindPropertyRelative("statValueType");
            var filterType = statLookup.FindPropertyRelative("filterType");
            
            ObjectPickerUtility.RenderObjectPickerForType<StatDefinition>(popupRect, string.Empty, stat);
            popupRect.x += popupRect.width;
            popupRect.x += 5;

            if (IsStatValueValid(statValue.floatValue) == false)
            {
                GUI.color = Color.red;
            }

            statValue.floatValue = EditorGUI.FloatField(popupRect, "", statValue.floatValue);
            GUI.color = Color.white;

            if (drawFilterType)
            {
                var r3 = r2;
                EditorGUI.PropertyField(r3, statValueType, new GUIContent(""));

                r3.x += r3.width + 5;
                EditorGUI.PropertyField(r3, filterType, new GUIContent(""));
            }

            GUI.enabled = true;
        }

        public static void DrawStatRequirement(Rect rect, StatRequirement stat, bool isActive, bool isFocused, bool drawFilterType)
        {
            rect.height = 16;
            rect.y += 2;

            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            ObjectPickerUtility.RenderObjectPickerForType<StatDefinition>(popupRect, string.Empty, stat.stat, (val) =>
            {
                stat.stat = val;
            });

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            if (IsStatValueValid(stat.value) == false)
            {
                GUI.color = Color.red;
            }

            stat.value = EditorGUI.FloatField(popupRect, "", stat.value);
            GUI.color = Color.white;

            if (drawFilterType)
            {
                EditorGUI.LabelField(r2, "Filter type");

                r2.x += r2.width + 5;
                stat.filterType = (StatRequirement.FilterType)EditorGUI.EnumPopup(r2, GUIContent.none, stat.filterType);
            }

            GUI.enabled = true;
        }

        public static void DrawItemAmountRow(Rect rect, SerializedProperty property)
        {
            var r2 = rect;
            r2.width /= 2;
            r2.width -= 5;

            var amount = property.FindPropertyRelative("amount");
            var itemRef = property.FindPropertyRelative("item");
            var item = itemRef.objectReferenceValue as InventoryItemBase;

            amount.intValue = EditorGUI.IntField(r2, amount.intValue);
            if (amount.intValue < 1)
            {
                amount.intValue = 1;
            }

            r2.x += r2.width + 5;

            if (item == null)
            {
                GUI.backgroundColor = Color.red;
            }

            ObjectPickerUtility.RenderObjectPickerForType<InventoryItemBase>(r2, "", itemRef);
            GUI.backgroundColor = Color.white;
        }

        public static void AudioClipInfo(string name, AudioClipInfo clip)
        {
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            ObjectPickerUtility.RenderObjectPickerForType(name, clip.audioClip, typeof(AudioClip), val =>
            {
                clip.audioClip = (AudioClip) val;
            });
            clip.volume = EditorGUILayout.FloatField("Volume", clip.volume);
            clip.pitch = EditorGUILayout.FloatField("Pitch", clip.pitch);
            clip.loop = EditorGUILayout.Toggle("Loop", clip.loop);
             
            EditorGUILayout.EndVertical();
        }
    }
}