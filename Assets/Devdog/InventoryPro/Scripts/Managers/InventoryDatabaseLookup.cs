using System;
using System.Collections.Generic;
using System.IO;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Devdog.InventoryPro
{
    public class InventoryDatabaseLookup<T> where T: UnityEngine.ScriptableObject
    {
        public T defaultDatabase { get; set; }
        private string key;

        private T _database;


        public bool hasSelectedDatabase
        {
            get
            {
                if (defaultDatabase != null)
                {
                    return true;
                }

                if (_database != null)
                {
                    return true;
                }

#if UNITY_EDITOR

                if (EditorPrefs.HasKey(key))
                {
                    string path = EditorPrefs.GetString(key);
                    var db = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (db != null)
                    {
                        return true;
                    }
                }

#endif

                return false;
            }
        }




        public T GetDatabase()
        {
            if (_database != null)
            {
                return _database;
            }

            if (defaultDatabase != null)
            {
                return defaultDatabase;
            }

#if UNITY_EDITOR

            if (EditorPrefs.HasKey(key))
            {
                string path = EditorPrefs.GetString(key);
                bool set = SetDatabase(AssetDatabase.LoadAssetAtPath<T>(path));
                if (set == false)
                {
                    // Path is no longer valid
                    EditorPrefs.DeleteKey(key);
                    throw new DatabaseNotFoundException(typeof(T).Name + " could not be found at previously set path. Re-select a database.");
                }
            }

            // Still null after selecting?
            if (_database == null)
            {
                ManuallySelectDatabase();
            }

#endif

            if (_database == null)
            {
                throw new DatabaseNotFoundException("No " + typeof(T).Name + " found, can't continue.");
            }

            return _database;
        }
        
        public bool SetDatabase(T database)
        {
            var before = _database;
            _database = database;
            defaultDatabase = database;

#if UNITY_EDITOR

            if (_database != null)
            {
                if (AssetDatabase.GetAssetPath(before) != AssetDatabase.GetAssetPath(_database) && before != null)
                {
                    EditorUtility.DisplayDialog("Switched database", "Switched to a new database (" + _database.name + ")", "Ok");
                }

                string path = AssetDatabase.GetAssetPath(_database);
                EditorPrefs.SetString(key, path);
                return true;
            }

            EditorPrefs.DeleteKey(key);
#endif

            return false;
        }

        public void SetDatabaseTemp(T database)
        {
            _database = database;
        }


        public bool ManuallySelectDatabase()
        {
#if UNITY_EDITOR

//            if (AssetDatabase.FindAssets("t:" + typeof(T).Name).Length == 0)
//            {
//                throw new FileNotFoundException("No " + typeof(T).Name + " in project folder, create one first.");
//            }

            var select = EditorUtility.DisplayDialog("Select an " + typeof(T).Name, "No " + typeof(T).Name + " could be found, please select one manually.", "Select database", "Create database");
            if (select)
            {
                string absolutePath = EditorUtility.OpenFilePanel("No " + typeof(T).Name + " could be found, please select it manually.", "Assets/InventorySystem/Demos/Assets/Databases/", "asset");
                bool set = SetDatabase(AssetDatabase.LoadAssetAtPath<T>("Assets" + absolutePath.Replace(Application.dataPath, "")));

                if (set == false)
                {
                    var errorString = "Selected file at: ";
                    errorString += "Assets" + absolutePath.Replace(Application.dataPath, "");
                    errorString += "is not an item database.";

                    errorString += "\n";
                    errorString += "Data Path: " + Application.dataPath;
                    errorString += "\n";
                    errorString += "Absolute path: " + absolutePath;

                    EditorUtility.DisplayDialog("Whoops!", errorString, "Ok");
                }

                return true;
            }
            
#endif

            return false;
        }


        public InventoryDatabaseLookup(T defaultDatabase, string key)
        {
            this.defaultDatabase = defaultDatabase;
            this.key = key;
        } 
    }
}
