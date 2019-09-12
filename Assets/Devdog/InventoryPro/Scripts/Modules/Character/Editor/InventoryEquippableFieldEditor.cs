using UnityEngine;
using UnityEditor;
using System.Collections;
using Devdog.General.Editors;
using Devdog.InventoryPro;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(EquippableSlot))]
    public class InventoryEquippableFieldEditor : Editor
    {
        private SerializedProperty _equipTypes;
        private UnityEditorInternal.ReorderableList _list;

        public void OnEnable()
        {
            _equipTypes = serializedObject.FindProperty("_equipmentTypes");

            _list = new UnityEditorInternal.ReorderableList(serializedObject, _equipTypes, true, true, true, true);
            _list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Which types can be placed in this field?");
            _list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 3;

                var i = _equipTypes.GetArrayElementAtIndex(index);
                ObjectPickerUtility.RenderObjectPickerForType<EquipmentType>(rect, i);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                "ID",
                _equipTypes.name
            });
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            if (GUILayout.Button("Edit types"))
            {
                InventoryMainEditor.window.Show();
                InventoryMainEditor.SelectTab(typeof(EquipmentTypeEditor));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Define which types are allowed in this wrapper.\n\nFor example when selecting helmet and necklace both items with equipment type helmet and neckalce can be equipped to this slot.", EditorStyles.labelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

    }
}