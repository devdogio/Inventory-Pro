using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventoryPro;
using UnityEditorInternal;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(InventoryPlayerSpawner), true)]
    public class InventoryPlayerSpawnerEditor : InventoryPlayerBaseEditor
    {
        protected SerializedProperty playerPrefab;

        public override void OnEnable()
        {
            base.OnEnable();

            playerPrefab = serializedObject.FindProperty("playerPrefab");
        }

        protected override void DrawSingleEquipField(SerializedProperty element, Rect rect)
        {
            base.DrawSingleEquipField(element, rect);

            if (playerPrefab.objectReferenceValue != null)
            {
                var obj = playerPrefab.objectReferenceValue as GameObject;
                if (obj != null)
                {
                    if (element.FindPropertyRelative("findDynamic").boolValue == false)
                    {
                        return;
                    }

                    string path = element.FindPropertyRelative("equipTransformPath").stringValue;
                    Transform child = null;
                    InventoryUtility.FindChildTransform(obj.transform, path, ref child);
                    if (child == null || path == "")
                    {
                        rect.y += 36;
                        
                        EditorGUI.HelpBox(rect, "Object with path \"" + path + "\" was not found.", MessageType.Error);
                    }
                }
            }
        }
    }
}