using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Devdog.General;
using Devdog.General.Editors;
using Devdog.General.Editors.ReflectionDrawers;
using Devdog.InventoryPro;
using UnityEditor;
using UnityEngine;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class EquipmentTypeEditor : ScriptableObjectEditorCrud<EquipmentType>
    {
        protected override List<EquipmentType> crudList
        {
            get { return new List<EquipmentType>(ItemManager.database.equipmentTypes); }
            set { ItemManager.database.equipmentTypes = value.ToArray(); }
        }

        private UnityEditorInternal.ReorderableList _restrictionList;
        private DrawerBase _equipmentHandlerDrawer;


        public EquipmentTypeEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {}


        public override void EditItem(EquipmentType item, int itemIndex)
        {
            base.EditItem(item, itemIndex);
            _equipmentHandlerDrawer = ReflectionDrawerUtility.BuildEditorHierarchy(ReflectionUtility.GetFieldInherited(item.GetType(), "equipmentHandler"), item);

            _restrictionList = new UnityEditorInternal.ReorderableList(selectedItem.blockTypes, typeof(int), false, true, true, true);
            _restrictionList.drawHeaderCallback += rect => GUI.Label(rect, "Restrictions");
            _restrictionList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                rect.width -= 30;
                rect.x += 30; // Some selection room

                if (index < 0 || index >= selectedItem.blockTypes.Length)
                {
                    return;
                }

                // Trying to block self, not allowed.
                if (selectedItem.blockTypes[index] == selectedItem)
                {
                    var t = rect;
                    t.width = 200;

                    GUI.backgroundColor = Color.red;
                    EditorGUI.LabelField(t, "Can't block self");

                    rect.x += 205; // +5 for margin
                    rect.width -= 205;
                }

                ObjectPickerUtility.RenderObjectPickerForType(rect, string.Empty, selectedItem.blockTypes[index], typeof(EquipmentType),
                    (val) =>
                    {
                        selectedItem.blockTypes[index] = (EquipmentType)val;
                    });

                GUI.backgroundColor = Color.white;
            };
            _restrictionList.onAddCallback += list =>
            {
                var l = new List<EquipmentType>(selectedItem.blockTypes);
                l.Add(null);
                selectedItem.blockTypes = l.ToArray();
                list.list = selectedItem.blockTypes;

                window.Repaint();
            };
            _restrictionList.onRemoveCallback += list =>
            {
                // Remove the element itself
                var l = new List<EquipmentType>(selectedItem.blockTypes);
                l.RemoveAt(list.index);
                selectedItem.blockTypes = l.ToArray();
                list.list = selectedItem.blockTypes;

                window.Repaint();
            };
        }

        protected override void GiveItemNewID(EquipmentType item)
        {

        }

        protected override bool MatchesSearch(EquipmentType item, string searchQuery)
        {
            if (item == null || item.name == null)
                return false;

            string search = searchQuery.ToLower();
            return (item.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }

        protected override void DrawSidebarRow(EquipmentType item, int i)
        {
            BeginSidebarRow(item, i);

            DrawSidebarRowElement(item.name, 300);
            DrawSidebarValidation(item, i);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(EquipmentType item, int itemIndex)
        {
            RenameScriptableObjectIfNeeded(item, item.name);

            using (new VerticalLayoutBlock(EditorStyles.boxStyle))
            {
                item.name = EditorGUILayout.DelayedTextField("Name", item.name);
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("You can force other fields to be empty when you set this. For example when equipping a greatsword, you might want to un-equip the shield.", EditorStyles.labelStyle);
                _restrictionList.DoLayoutList();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                var r = EditorGUILayout.GetControlRect();
                _equipmentHandlerDrawer.Draw(ref r);
            }

            ValidateItemFromCache(item);
        }
    }
}
