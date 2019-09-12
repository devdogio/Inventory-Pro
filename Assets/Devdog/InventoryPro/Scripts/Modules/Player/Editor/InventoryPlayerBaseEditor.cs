using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.InventoryPro;
using UnityEditorInternal;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(InventoryPlayerBase), true)]
    public class InventoryPlayerBaseEditor : InventoryEditorBase
    {
        protected ReorderableList inventoriesList;
        protected ReorderableList inventoryCollectionsList;

        protected SerializedProperty dynamicallyFindUIElements;
        protected SerializedProperty characterCollectionName;
        protected SerializedProperty inventoryCollectionNames;
        protected SerializedProperty skillbarCollectionName;


        protected SerializedProperty characterCollection;
        protected SerializedProperty inventoryCollections;
        protected SerializedProperty skillbarCollection;


        protected ReorderableList equipLocationsList;
        protected SerializedProperty equipmentBinders;

        private InventoryPlayerBase _tar;
        private void ReSync()
        {
            if (_tar.dynamicallyFindUIElements)
            {
                _tar.FindUIElements();
            }

            if ((_tar.characterUI == null || _tar.characterUI.container == null))
            {
                Debug.LogWarning("Can't scan for wrappers, character collection not set.");
                return;
            }

            var newList = new List<CharacterEquipmentTypeBinder>();
            var equipSlotFields = _tar.characterUI.container.GetComponentsInChildren<EquippableSlot>(true);

            for (int i = 0; i < equipSlotFields.Length; i++)
            {
                var toAdd = new CharacterEquipmentTypeBinder(equipSlotFields[i], null);

                // Find in old data
                var found = _tar.equipmentBinders.FirstOrDefault(o => o.equippableSlot == equipSlotFields[i]);
                if (found != null)
                {
                    toAdd = found;
                    //toAdd = new InventoryPlayerEquipTypeBinder(t.equipLocations[i].associatedSlot, t.equipLocations[i].equipTransform);
                }

                newList.Add(toAdd);
            }


            _tar.equipmentBinders = newList.ToArray();
            GUI.changed = true; // To save

            equipLocationsList.list = _tar.equipmentBinders; // Update equipLocationsList
            Repaint();
        }


        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            _tar = (InventoryPlayerBase)target;

            dynamicallyFindUIElements = serializedObject.FindProperty("dynamicallyFindUIElements");
            characterCollectionName = serializedObject.FindProperty("characterCollectionName");
            inventoryCollectionNames = serializedObject.FindProperty("inventoryCollectionNames");
            skillbarCollectionName = serializedObject.FindProperty("skillbarCollectionName");

            characterCollection = serializedObject.FindProperty("_characterUI");
            inventoryCollections = serializedObject.FindProperty("_inventoryCollections");
            skillbarCollection = serializedObject.FindProperty("_skillbarCollection");

            inventoriesList = new ReorderableList(serializedObject, inventoryCollectionNames, false, true, true, true);
            inventoriesList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Inventory paths");
            };
            inventoriesList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var prop = inventoryCollectionNames.GetArrayElementAtIndex(index);
                if (_tar.FindElement<ItemCollectionBase>(prop.stringValue, false) == null)
                {
                    var r = rect;
                    r.height = 18;
                    r.width = 140;
                    r.y -= 2;
                    r.x += 5;

                    rect.width -= (r.width + 10);
                    rect.x += (r.width + 10);

                    var style = new GUIStyle((GUIStyle) "CN EntryWarn")
                    {
                        wordWrap = false,
                        fixedHeight = rect.height,
                        fontStyle = FontStyle.Bold
                    };
                    EditorGUI.LabelField(r, "(Not found)", style);
                }

                EditorGUI.PropertyField(rect, prop);

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_tar);
                }
            };
            
            inventoryCollectionsList = new ReorderableList(serializedObject, inventoryCollections, false, true, true, true);
            inventoryCollectionsList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Inventory collections");
            };
            inventoryCollectionsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                EditorGUI.PropertyField(rect, inventoryCollections.GetArrayElementAtIndex(index));

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_tar);
                }
            };



            equipmentBinders = serializedObject.FindProperty("_equipmentBinders");

            equipLocationsList = new UnityEditorInternal.ReorderableList(serializedObject, equipmentBinders, false, true, false, false);
            equipLocationsList.elementHeight = 66;
            equipLocationsList.drawHeaderCallback += rect2 =>
            {
                EditorGUI.LabelField(rect2, "Equipment binding");
            };
            equipLocationsList.drawElementCallback += (rect2, index2, active2, focused2) =>
            {
                rect2.height = 16;
                rect2.y += 2;

                DrawSingleEquipField(equipmentBinders.GetArrayElementAtIndex(index2), rect2);
            };
        }

        protected virtual void DrawSingleEquipField(SerializedProperty element, Rect rect)
        {
            var rectLeftField = rect;
            rectLeftField.width = 20;
            rectLeftField.x += 120f;

            var findDynamic = element.FindPropertyRelative("findDynamic");
            var equipTransformPath = element.FindPropertyRelative("equipTransformPath");
            var equipTransform = element.FindPropertyRelative("equipTransform");
            var equippableSlot = element.FindPropertyRelative("equippableSlot");
            var rootBone = element.FindPropertyRelative("rootBone");

            EditorGUI.PropertyField(rectLeftField, findDynamic, new GUIContent(""));

            if (findDynamic.boolValue)
            {
                EditorGUI.PropertyField(rect, equipTransformPath);
            }
            else
            {
                EditorGUI.PropertyField(rect, equipTransform);
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, equippableSlot, new GUIContent("Equippable slot"));

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, rootBone, new GUIContent("Root bone"));

            var t = element.FindPropertyRelative("equipTransform").objectReferenceValue as Transform;
            if (t != null && t.IsChildOf(((InventoryPlayerBase)target).transform) == false)
            {
                rect.y += 18;
                EditorGUI.HelpBox(rect, "EquippedItem transform has to be a child of this character.", MessageType.Error);
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnCustomInspectorGUI();

            if (GUILayout.Button("Force rescan"))
            {
                ReSync();
            }

            equipLocationsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new[]
            {
                "m_Script",
                "_equipmentBinders",
                "dynamicallyFindUIElements",
                "characterCollectionName",
                "inventoryCollectionNames",
                "skillbarCollectionName",
                "_characterUI",
                "_inventoryCollections",
                "_skillbarCollection"
            });

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Player's collections", EditorStyles.titleStyle);
            EditorGUILayout.PropertyField(dynamicallyFindUIElements);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            if (dynamicallyFindUIElements.boolValue)
            {
                EditorGUILayout.PropertyField(characterCollectionName, true);
                inventoriesList.DoLayoutList();
                EditorGUILayout.PropertyField(skillbarCollectionName, true);
            }
            
            if(dynamicallyFindUIElements.boolValue == false || EditorApplication.isPlaying)
            {
                GUI.enabled = !EditorApplication.isPlaying;

                EditorGUILayout.PropertyField(characterCollection, true);
                inventoryCollectionsList.DoLayoutList();
                EditorGUILayout.PropertyField(skillbarCollection, true);

                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
        }
    }
}