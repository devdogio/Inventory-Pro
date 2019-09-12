using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using UnityEngine.Assertions;
using EditorStyles = Devdog.General.Editors.EditorStyles;
using EditorUtility = UnityEditor.EditorUtility;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(EquippableInventoryItem), true)]
    public class EquippableInventoryItemEditor : InventoryItemBaseEditor
    {
        protected SerializedProperty equipmentType;
        protected SerializedProperty equipVisually;
        protected SerializedProperty equipPosition;
        protected SerializedProperty equipRotation;


        private static EquippableInventoryItem currentEquippable { get; set; }
        private static CharacterEquipmentTypeBinder currentBinder { get; set; }
        private static InventoryPlayer player { get; set; }

        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            equipmentType = serializedObject.FindProperty("_equipmentType");
            equipPosition = serializedObject.FindProperty("_equipmentPosition");
            equipRotation = serializedObject.FindProperty("_equipmentRotation");
            equipVisually = serializedObject.FindProperty("equipVisually");
        }

        protected override void DrawItemStatLookup(Rect rect, SerializedProperty property, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.DrawItemStatLookup(rect, property, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {

            var l = new List<CustomOverrideProperty>(extraOverride);
            l.Add(new CustomOverrideProperty(equipmentType.name, () =>
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("EquippedItem type", GUILayout.Width(EditorGUIUtility.labelWidth - 5));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("Edit types in the EquippedItem editor", MessageType.Info);
                ObjectPickerUtility.RenderObjectPickerForType<EquipmentType>(equipmentType);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }));
            l.Add(new CustomOverrideProperty(equipVisually.name, () =>
            {
                EditorGUILayout.PropertyField(equipVisually);
            }));
            l.Add(new CustomOverrideProperty(equipPosition.name, () =>
            {
                GUI.enabled = equipVisually.boolValue;

                if (currentEquippable == null)
                {
                    if (GUILayout.Button("Position now"))
                    {
                        player = GetPlayer();
                        if (player != null)
                        {
                            player.characterUI.character = player;
                            player.characterUI.IndexManuallyDefinedCollection();
                            player.characterUI.UpdateEquippableSlots();

                            UnityEngine.Object prefab = GetPrefabFor(target);

                            if (prefab == null)
                                Debug.LogError("prefab == null");

                            var instance = PrefabUtility.InstantiatePrefab(prefab);
                            if (instance == null)
                                Debug.LogError("instance == null");

                            currentEquippable = (EquippableInventoryItem)instance;
                            if (currentEquippable == null)
                                Debug.LogError("currentEquippable == null");

                            currentBinder = player.equipmentHandler.FindEquipmentLocation(player.equipmentHandler.FindEquippableSlotForItem(currentEquippable));
                            if (currentBinder == null)
                            {
                                Debug.Log("No suitable equip location found");
                                UnityEngine.Object.DestroyImmediate(currentEquippable.gameObject);
                                return;
                            }

                            currentBinder.equippableSlot.equipmentTypes.First(o => o == currentEquippable.equipmentType).equipmentHandler.Equip(currentEquippable, currentBinder, false);

                            currentEquippable.transform.localPosition = equipPosition.vector3Value;
                            currentEquippable.transform.localRotation = equipRotation.quaternionValue;

                            Selection.activeObject = currentEquippable.gameObject;
                            if (SceneView.currentDrawingSceneView != null)
                            {
                                SceneView.currentDrawingSceneView.LookAt(currentEquippable.transform.position);
                            }
                        }
                    }
                }
                else
                {
                    // Ugly workaround...
                    if (currentBinder != null && currentEquippable.transform.parent == null)
                    {
                        currentBinder.equippableSlot.equipmentTypes.First(o => o == currentEquippable.equipmentType).equipmentHandler.Equip(currentEquippable, currentBinder, false);
                    }

                    GUI.color = Color.green;
                    if (GUILayout.Button("Save state"))
                    {
                        equipPosition.vector3Value = currentEquippable.transform.localPosition;
                        equipRotation.quaternionValue = currentEquippable.transform.localRotation;

                        serializedObject.ApplyModifiedProperties();
#if UNITY_2018_3_OR_NEWER
                        PrefabUtility.ApplyPrefabInstance(currentEquippable.gameObject, InteractionMode.AutomatedAction);
#endif
                        AssetDatabase.SaveAssets(); // Save it

                        DestroyImmediate(currentEquippable.gameObject); // Get rid of positioning object
                        //SceneView.currentDrawingSceneView.SetSceneViewFiltering(false);
                    }
                    GUI.color = Color.white;
                }

                if (currentEquippable != null)
                    GUI.enabled = false;

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal(EditorStyles.boxStyle);

                EditorGUILayout.PropertyField(equipPosition);
                GUILayout.Space(20);
                equipRotation.quaternionValue = ToQuat(EditorGUILayout.Vector4Field("EquippedItem Rotation", ToVec4(equipRotation.quaternionValue)));

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUI.enabled = true;
            }));
            l.Add(new CustomOverrideProperty(equipRotation.name, null));

            base.OnCustomInspectorGUI(l.ToArray());
        }

        private UnityEngine.Object GetPrefabFor(UnityEngine.Object target)
        {
            UnityEngine.Object prefab = null;
#if UNITY_2019_1_OR_NEWER || UNITY_2018_3_OR_NEWER
            prefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
#else

            var prefabType = PrefabUtility.GetPrefabType(target);
            switch (prefabType)
            {
                case PrefabType.Prefab:
                    prefab = target;
                    break;

                case PrefabType.PrefabInstance:
#if UNITY_2018_2_OR_NEWER
                    prefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
#else
                    prefab = PrefabUtility.FindPrefabRoot((GameObject)target);
#endif
                    break;

                default:
                    Debug.LogError("Unhandled prefab type: " + prefabType, target);
                    break;
            }
#endif
            return prefab;
        }

        protected virtual InventoryPlayer GetPlayer()
        {
            var playersInScene = FindObjectsOfType<InventoryPlayer>();
            if (playersInScene.Length == 0)
            {
                Debug.LogWarning("No players found in scene to position model on");
                return null;
            }

            if (playersInScene.Length > 1)
            {
                Debug.LogWarning("Currently only supporting 1 player at a time (returning first player)");
            }

            playersInScene[0].equipmentHandler.Init(playersInScene[0].characterUI); // Needed for equipment, not created in editor...
            return playersInScene[0];
        }


        public EquippableSlot GetBestEquipSlot(EquippableInventoryItem item, EquippableSlot[] slots, InventoryPlayer player)
        {
            if (slots.Length > 0)
                return slots[0];

            return null;
        }

        public EquippableSlot[] GetEquippableSlots(EquippableInventoryItem item, InventoryPlayer player)
        {
            if (player == null || player.characterUI == null)
            {
                return new EquippableSlot[0];
            }

            player.characterUI.IndexManuallyDefinedCollection();
            player.characterUI.UpdateEquippableSlots();
            return player.characterUI.GetEquippableSlots(item);
        }

        private Vector4 ToVec4(Quaternion quat)
        {
            return new Vector4(quat.x, quat.y, quat.z, quat.w);
        }

        private Quaternion ToQuat(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
    }
}