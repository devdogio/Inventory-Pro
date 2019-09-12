using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventoryPro.UI;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(ItemCollectionBase), true)]
    [CanEditMultipleObjects()]
    public class ItemCollectionBaseEditor : InventoryEditorBase
    {
        private ItemCollectionBase item;
        private SerializedObject serializer;

        private SerializedProperty collectionName;
        private SerializedProperty restrictByWeight;
        private SerializedProperty restrictMaxWeight;
        private SerializedProperty itemButtonPrefab;

        private SerializedProperty filters;

        private SerializedProperty useReferences;
        private SerializedProperty ignoreItemLayoutSizes;
        private SerializedProperty canContainCurrencies;
        private SerializedProperty canUseItemsFromReference;
        private SerializedProperty canDropFromCollection;
        private SerializedProperty canUseFromCollection;
        private SerializedProperty canDragInCollection;
        private SerializedProperty canPutItemsInCollection;
        private SerializedProperty canStackItemsInCollection;
        private SerializedProperty canUnstackItemsInCollection;
        private SerializedProperty manuallyDefineCollection;
        private SerializedProperty container;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            item = (ItemCollectionBase)target;
            //serializer = new SerializedObject(target);
            serializer = serializedObject;
            
            collectionName = serializer.FindProperty("collectionName");
            restrictByWeight = serializer.FindProperty("restrictByWeight");
            restrictMaxWeight = serializer.FindProperty("restrictMaxWeight");
            itemButtonPrefab = serializer.FindProperty("itemButtonPrefab");

            filters = serializer.FindProperty("filters");
            useReferences = serializer.FindProperty("_useReferences");
            ignoreItemLayoutSizes = serializer.FindProperty("ignoreItemLayoutSizes");
            canContainCurrencies = serializer.FindProperty("canContainCurrencies");
            canDropFromCollection = serializer.FindProperty("canDropFromCollection");
            canUseItemsFromReference = serializer.FindProperty("canUseItemsFromReference");
            canUseFromCollection = serializer.FindProperty("canUseFromCollection");
            canDragInCollection = serializer.FindProperty("canDragInCollection");
            canPutItemsInCollection = serializer.FindProperty("canPutItemsInCollection");
            canStackItemsInCollection = serializer.FindProperty("canStackItemsInCollection");
            canUnstackItemsInCollection = serializer.FindProperty("canUnstackItemsInCollection");

            manuallyDefineCollection = serializer.FindProperty("manuallyDefineCollection");
            container = serializer.FindProperty("container");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnCustomInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);
            
            GUILayout.Label("General settings", EditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            //GUILayout.Label("General", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(collectionName);
            EditorGUILayout.PropertyField(useReferences);
            EditorGUILayout.PropertyField(ignoreItemLayoutSizes);

            if (ignoreItemLayoutSizes.boolValue == false)
            {
                var t = (ItemCollectionBase) target;
                // Using layout stuff
                if (t.container != null)
                {
                    var layoutGroup = t.container.GetComponent<DynamicLayoutGroup>();
                    if (layoutGroup == null)
                    {
                        EditorGUILayout.HelpBox("Collection is using item layouts, but the container doesn't contain a DynamicLayoutGroup component.", MessageType.Error);
                    }
                }
            }

            GUILayout.Label("UI", EditorStyles.titleStyle);
            EditorGUILayout.PropertyField(itemButtonPrefab);
            EditorGUILayout.PropertyField(container);
            EditorGUILayout.PropertyField(manuallyDefineCollection);

            if (manuallyDefineCollection.boolValue)
            {
                if (item.container == null)
                {
                    EditorGUILayout.LabelField("Couldn't look for wrappers, container is null");
                }
                else
                {
                    var amount = item.container.GetComponentsInChildren<ItemCollectionSlotUIBase>().Length;
                    if (EditorApplication.isPlaying)
                    {
                        amount = item.items.Length;
                    }
                    EditorGUILayout.LabelField("Found " + amount + " wrappers in container.");
                }
            }

            EditorGUILayout.EndVertical();


            // Draws remaining items
            GUILayout.Label("Collection specific", EditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);

            var doNotDrawList = new List<string>()
            {
                "m_Script",
                "collectionName",
                "restrictByWeight",
                "restrictMaxWeight",
                "itemButtonPrefab",
                "_items",
                "_useReferences",
                "ignoreItemLayoutSizes",
                "canContainCurrencies",
                "canDropFromCollection",
                "canUseFromCollection",
                "canDragInCollection",
                "canPutItemsInCollection",
                "canStackItemsInCollection",
                "canUnstackItemsInCollection",
                "manuallyDefineCollection",
                "container",
                "onlyAllowTypes",
                "canUseItemsFromReference",
                "filters"
            };

            foreach (var extra in extraOverride)
            {
                extra.action();
                doNotDrawList.Add(extra.serializedName);
            }

            DrawPropertiesExcluding(serializer, doNotDrawList.ToArray());
            EditorGUILayout.EndVertical();


            GUILayout.Label("Restrictions", EditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
            EditorGUILayout.PropertyField(filters);

            GUILayout.Label("Other", EditorStyles.titleStyle);
            EditorGUILayout.PropertyField(restrictByWeight);
            if (restrictByWeight.boolValue)
                EditorGUILayout.PropertyField(restrictMaxWeight);

            EditorGUILayout.PropertyField(canContainCurrencies);
            EditorGUILayout.PropertyField(canDropFromCollection);
            EditorGUILayout.PropertyField(canUseFromCollection);
            GUI.enabled = canUseFromCollection.boolValue;
            EditorGUILayout.PropertyField(canUseItemsFromReference);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(canDragInCollection);
            EditorGUILayout.PropertyField(canPutItemsInCollection);
            EditorGUILayout.PropertyField(canStackItemsInCollection);
            EditorGUILayout.PropertyField(canUnstackItemsInCollection);

            EditorGUILayout.EndVertical();

        }
    }
}