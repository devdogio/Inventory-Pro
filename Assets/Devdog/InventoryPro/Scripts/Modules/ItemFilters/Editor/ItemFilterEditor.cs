using UnityEngine;
using UnityEditor;
using System;
using Devdog.General.Editors;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{

    [CustomPropertyDrawer(typeof(ItemFilter), true)]
    public class ItemFilterEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var r = position;
            r.width = 100;

            EditorGUI.PropertyField(r, property.FindPropertyRelative("restrictionType"), new GUIContent(""));

            r.x += r.width + 5;
            EditorGUI.PropertyField(r, property.FindPropertyRelative("filterType"), new GUIContent(""));


            int restrictionTypeIndex = property.FindPropertyRelative("restrictionType").enumValueIndex;
            var restrictionType = (ItemFilter.RestrictionType)restrictionTypeIndex;

            int filterTypeIndex = property.FindPropertyRelative("filterType").enumValueIndex;
            ItemFilter.FilterType filterType = (ItemFilter.FilterType)filterTypeIndex;

            r.x += r.width + 5;
            r.width = position.width - 210;


            var categoryValue = property.FindPropertyRelative("categoryValue");
            var propertyValue = property.FindPropertyRelative("statDefinitionValue");
            var rarityValue = property.FindPropertyRelative("rarityValue");

            var stringValue = property.FindPropertyRelative("stringValue");
            var boolValue = property.FindPropertyRelative("boolValue");
            var floatValue = property.FindPropertyRelative("floatValue");
//            var intValue = property.FindPropertyRelative("intValue");

            switch (restrictionType)
            {
                case ItemFilter.RestrictionType.Type:

                    //r.width -= 65;
                    GUI.enabled = false;
                    var t = System.Type.GetType(stringValue.stringValue);
                    EditorGUI.LabelField(r, t != null ? t.Name : "(NOT SET)");
                    GUI.enabled = true;

                    r.x += r.width - 60;
                    r.width = 60;
                    r.height = 14;
                    if (GUI.Button(r, "Set", "minibutton"))
                    {
                        var typePicker = ScriptPickerEditor.Get(typeof(InventoryItemBase));
                        typePicker.Show();
                        typePicker.OnPickObject += type =>
                        {
                            stringValue.stringValue = type.AssemblyQualifiedName;
                            GUI.changed = true; // To save..
                            stringValue.serializedObject.ApplyModifiedProperties();
                        };
                    }

                    break;

                case ItemFilter.RestrictionType.Stat:

                    if (filterType == ItemFilter.FilterType.LessThan || filterType == ItemFilter.FilterType.GreatherThan)
                    {
                        r.width /= 2;

                        ObjectPickerUtility.RenderObjectPickerForType<StatDefinition>(r, "", propertyValue);

                        r.x += r.width;
                        floatValue.floatValue = EditorGUI.FloatField(r, floatValue.floatValue);
                    }
                    else
                    {
                        ObjectPickerUtility.RenderObjectPickerForType<StatDefinition>(r, "", propertyValue);
                    }

                    break;

                case ItemFilter.RestrictionType.Category:

                    ObjectPickerUtility.RenderObjectPickerForType<ItemCategory>(r, "", categoryValue);
                    break;

                case ItemFilter.RestrictionType.Rarity:

                    ObjectPickerUtility.RenderObjectPickerForType<ItemRarity>(r, "", rarityValue);

                    break;

                case ItemFilter.RestrictionType.Weight:
                    floatValue.floatValue = EditorGUI.FloatField(r, floatValue.floatValue);
                    break;

                case ItemFilter.RestrictionType.Sellable:
                case ItemFilter.RestrictionType.Storable:
                case ItemFilter.RestrictionType.Droppable:
                    boolValue.boolValue = EditorGUI.Toggle(r, boolValue.boolValue);
                    break;
                default:
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
}