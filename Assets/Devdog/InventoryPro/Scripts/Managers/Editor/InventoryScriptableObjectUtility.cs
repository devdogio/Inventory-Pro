using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Devdog.General;
using EditorUtility = UnityEditor.EditorUtility;

namespace Devdog.InventoryPro.Editors
{
    public static class InventoryScriptableObjectUtility
    {
        public const string PrefabsSaveFolderSaveKey = "InventoryPro_PrefabSavePath";

        public static bool isPrefabsSaveFolderSet
        {
            get { return EditorPrefs.HasKey(PrefabsSaveFolderSaveKey); }
        }

        public static bool isPrefabsSaveFolderValid
        {
            get { return AssetDatabase.IsValidFolder(prefabsSaveFolder); }
        }

        public static string prefabsSaveFolder
        {
            get { return EditorPrefs.GetString(PrefabsSaveFolderSaveKey, "Assets/MyInventoryProItems"); }
            private set { EditorPrefs.SetString(PrefabsSaveFolderSaveKey, value); }
        }

        public static string GetSaveFolderForFolderName(string name)
        {
            if (isPrefabsSaveFolderSet == false)
            {
                DevdogLogger.LogWarning("Trying to grab folder for: " + name + " but no prefab folder is set.");
                return string.Empty;
            }

            var saveFolder = prefabsSaveFolder + "/" + name;
            if (AssetDatabase.IsValidFolder(saveFolder) == false)
            {
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, name);
                DevdogLogger.LogVerbose("Trying to grab folder for " + name + " but could not be found. Creating one...");
            }

            return saveFolder;
        }

        public static string GetSaveFolderForType(Type type)
        {
            return GetSaveFolderForFolderName(type.Name);
        }

        public static void SetPrefabSaveFolderIfNotSet()
        {
            if (isPrefabsSaveFolderSet == false)
            {
                SetPrefabSaveFolder();
            }
        }

        public static void SetPrefabSaveFolder()
        {
            string absolutePath = EditorUtility.SaveFolderPanel("Choose a folder to save your item prefabs", "", "");
            prefabsSaveFolder = "Assets" + absolutePath.Replace(Application.dataPath, "");

            if (isPrefabsSaveFolderValid)
            {
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, "Items");
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(StatDefinition).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(ItemCategory).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(ItemRarity).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(EquipmentType).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(CurrencyDefinition).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(CraftingCategory).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, typeof(CraftingBlueprint).Name);
                CreateFolderIfDoesNotExistAlready(prefabsSaveFolder, "Settings");
            }
        }

        private static void CreateFolderIfDoesNotExistAlready(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder(path + "/" + folderName) == false)
            {
                AssetDatabase.CreateFolder(path, folderName);
            }
        }
    }
}