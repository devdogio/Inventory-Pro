using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.Editors;
using UnityEditor;

namespace Devdog.InventoryPro.Editors
{
    public abstract class ScriptableObjectEditorCrud<T> : InventoryEditorCrudBase<T> where T : ScriptableObject
    {
        protected ScriptableObjectEditorCrud(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            
        }

        protected override void CreateNewItem()
        {
            var item = ScriptableObjectUtility.CreateAsset<T>(InventoryScriptableObjectUtility.GetSaveFolderForType(typeof(T)), DateTime.Now.ToFileTimeUtc() + ".asset");
            GiveItemNewID(item);
            AddItem(item, true);
        }

        public override void AddItem(T item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);
            UnityEditor.EditorUtility.SetDirty(ItemManager.database);
        }

        protected abstract void GiveItemNewID(T item);


        public override void Draw()
        {
            EditorGUI.BeginChangeCheck();
            base.Draw();
            if (EditorGUI.EndChangeCheck() && selectedItem != null)
            {
                UnityEditor.EditorUtility.SetDirty(selectedItem);
                UnityEditor.EditorUtility.SetDirty(ItemManager.database);
            }
        }

        public override void RemoveItem(int index)
        {
            var item = crudList[index];
            var path = AssetDatabase.GetAssetPath(item);
            if (string.IsNullOrEmpty(path) == false)
            {
                AssetDatabase.DeleteAsset(path);
            }

            base.RemoveItem(index);
            UnityEditor.EditorUtility.SetDirty(ItemManager.database);
        }

        public override void DuplicateItem(int index)
        {
            var item = Clone(index);
            GiveItemNewID(item);
            item.name += "(duplicate)";

            AddItem(item);
        }

        protected virtual void RenameScriptableObjectIfNeeded(T obj, string name)
        {
            if (obj == null)
            {
                return;
            }

            var nameWithExtension = name ?? "";
            if (nameWithExtension.EndsWith(".asset") == false)
            {
                nameWithExtension += ".asset";
            }

            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (assetPath.EndsWith(nameWithExtension) == false)
            {
                var saveFolder = InventoryScriptableObjectUtility.GetSaveFolderForType(typeof (T));
                var renamePath = saveFolder + "/" + nameWithExtension;
                if (AssetDatabase.LoadAssetAtPath<T>(renamePath) == null)
                {
                    RenameScriptableObject(obj, name ?? "nameless");
                }
            }
        }

        private void RenameScriptableObject(T obj, string name)
        {
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c.ToString(), "");
            }

            // An AssetDatabase.RenameAsset call doesn't need the extension, just the name.
            if (name.EndsWith(".asset"))
            {
                name = name.Replace(".asset", "");
            }

            var error = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), name);
            if (string.IsNullOrEmpty(error) == false)
            {
//                DevdogLogger.LogError(error);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {

        }
    }
}