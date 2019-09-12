using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.Editors;
using Devdog.General.Editors.ReflectionDrawers;
using UnityEditor;

namespace Devdog.InventoryPro.Editors
{
    public abstract class InventoryEditorCrudBase<T> : EditorCrudBase<T> where T : class
    {
        protected class ErrorLookup
        {
            public string message;
            public List<DrawerBase> drawers = new List<DrawerBase>();
        }

        protected Dictionary<T, ErrorLookup> itemsErrorLookup = new Dictionary<T, ErrorLookup>();

        protected InventoryEditorCrudBase(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {

        }

        public override void Focus()
        {
            base.Focus();
            ValidateItems();
        }

        public override void AddItem(T item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);
            ValidateItem(item);
        }

        /// <summary>
        /// Validate all items and make sure they're set up correctly. If not render a warning icon in the crud list.
        /// </summary>
        private void ValidateItems()
        {
            foreach (var item in crudList)
            {
                if (item == null)
                {
                    continue;
                }

                var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(item.GetType(), item);
                foreach (var drawer in drawers)
                {
                    var isValid = ValidateItem(item, drawer);
                    if (isValid == false)
                    {
                        break;
                    }
                }
            }
        }

        protected void ValidateItemFromCache(T item)
        {
            if (itemsErrorLookup.ContainsKey(item))
            {
                var lookup = itemsErrorLookup[item];
                for (int i = 0; i < lookup.drawers.Count; i++)
                {
                    var isValid = ValidateItem(item, lookup.drawers[i], true);
                    if (isValid == false)
                    {
                        break;
                    }
                }
            }
        }

        private void ValidateItem(T item)
        {
            var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(item.GetType(), item);
            foreach (var drawer in drawers)
            {
                ValidateItem(item, drawer, false, false);
            }
        }

        protected virtual bool ValidateItem(T item, DrawerBase drawer, bool refreshValues = false, bool isRoot = true)
        {
            if (itemsErrorLookup.ContainsKey(item) == false)
            {
                itemsErrorLookup[item] = new ErrorLookup();
            }
            else
            {
                if (isRoot)
                {
                    itemsErrorLookup[item].message = "";
                    itemsErrorLookup[item].drawers.Clear();
                }
            }

            if (refreshValues)
            {
                drawer.RefreshValue();
            }

            var childDrawer = drawer as IChildrenDrawer;
            if (childDrawer != null)
            {
                bool allValid = true;
                foreach (var c in childDrawer.children)
                {
                    var valid = ValidateItem(item, c, refreshValues, false);
                    if (valid == false)
                    {
                        allValid = false;
                    }
                }

                if (allValid == false)
                {
                    return false;
                }
            }
            else
            {
                if (drawer.isEmpty && drawer.required && drawer.isInArray == false)
                {
                    itemsErrorLookup[item].message += drawer.fieldName.text + " is empty" + '\n';
                    itemsErrorLookup[item].drawers.Add(drawer);
                    return false;
                }
            }

            return true;
        }

        protected virtual void DrawSidebarValidation(T item, int i)
        {
            if (itemsErrorLookup.ContainsKey(item) && itemsErrorLookup[item].drawers.Count > 0)
            {
                sidebarRowElementOffset.width = 20;
                Vector2 offset = new Vector2(15, 8);
                if (canReOrderItems)
                {
                    offset.x += 60;
                }

                sidebarRowElementOffset.position -= offset;

                EditorGUI.LabelField(sidebarRowElementOffset, string.Empty, (GUIStyle)"CN EntryError");
                if (sidebarRowElementOffset.Contains(Event.current.mousePosition))
                {
                    DrawTooltip(Event.current.mousePosition, itemsErrorLookup[item].message);
                }

                sidebarRowElementOffset.position += offset;
            }
        }


        private void DrawTooltip(Vector2 position, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var style = (GUIStyle)"SelectionRect";
            var height = 100f;

            GUI.color = new Color(1, 1, 1, 0.5f);
            using (new GroupBlock(new Rect(position.x, position.y, 240, height), GUIContent.none, style))
            {
                GUI.color = Color.white;
                EditorGUI.LabelField(new Rect(10, 10, 220, height - 20), message, UnityEditor.EditorStyles.wordWrappedLabel);
            }

            GUI.color = Color.white;
        }
    }
}