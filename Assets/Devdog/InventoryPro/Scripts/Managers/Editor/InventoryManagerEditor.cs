using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace Devdog.InventoryPro.Editors
{

    [CustomEditor(typeof(InventoryManager))]
    public class InventoryManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Run-time info:", EditorStyles.boldLabel);

                //private static List<ItemCollectionPriority<ItemCollectionBase>> _lootToCollections = new List<ItemCollectionPriority<ItemCollectionBase>>();
                //private static List<ItemCollectionPriority<CharacterUI>> _equipToCollections = new List<ItemCollectionPriority<CharacterUI>>(4);
                //private static List<ItemCollectionBase> _bankCollections = new List<ItemCollectionBase>(4);
                var type = typeof(InventoryManager);
                var lootToCollections = (List<ItemCollectionPriority<ItemCollectionBase>>)type.GetField("_lootToCollections", System.Reflection.BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Loot to collections:", EditorStyles.boldLabel);
                foreach (var col in lootToCollections)
                {
                    if (GUILayout.Button(col.collection.collectionName))
                    {
                        Selection.activeObject = col.collection;
                    }
                }

                EditorGUILayout.EndVertical();


                var equipToCollections = (List<ItemCollectionPriority<CharacterUI>>)type.GetField("_equipToCollections", System.Reflection.BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Character collections:", EditorStyles.boldLabel);
                foreach (var col in equipToCollections)
                {
                    if (GUILayout.Button(col.collection.collectionName))
                    {
                        Selection.activeObject = col.collection;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}