using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Devdog.General.Editors;
using Devdog.General.Editors.GameRules;
using EditorUtility = UnityEditor.EditorUtility;
using EditorStyles = Devdog.General.Editors.EditorStyles;

#if PLY_GAME
using Devdog.InventoryPro.Integration.plyGame.Editors;
#endif


namespace Devdog.InventoryPro.Editors
{
    using Devdog.General.ThirdParty.UniLinq;

    public class InventoryMainEditor : BetterEditorWindow
    {
        private static int toolbarIndex { get; set; }

        public static EmptyEditor itemEditor { get; set; }
        public static EmptyEditor equipEditor { get; set; }
        public static CraftingEmptyEditor craftingEditor { get; set; }
        public static LanguageEditor languageEditor { get; set; }
        public static SettingsEditor settingsEditor { get; set; }

        public static List<IEditorCrud> editors = new List<IEditorCrud>(8);

        private static IGameRule[] _gameRules = new IGameRule[0];
        private static InventoryMainEditor _window;
		//        private string[] _databasesInProject;
		private UnityEngine.SceneManagement.Scene previewScene;

		public static InventoryMainEditor window
        {
            get
            {
                if(_window == null)
                    _window = GetWindow<InventoryMainEditor>(false, "Inventory Pro Manager", false);

                return _window;
            }
        }

        protected string[] editorNames
        {
            get
            {
                string[] items = new string[editors.Count];
                for (int i = 0; i < editors.Count; i++)
                {
                    items[i] = editors[i].ToString();
                }

                return items;
            }
        }

        [MenuItem(InventoryPro.ToolsMenuPath + "Main editor", false, -99)] // Always at the top
        public static void ShowWindow()
        {
            _window = GetWindow<InventoryMainEditor>(false, "Inventory Pro Manager", true);
        }

        private void OnEnable()
        {
            minSize = new Vector2(600.0f, 400.0f);
            toolbarIndex = 0;

            //if (ItemManager.database == null)
            //    return;

            //            _databasesInProject = AssetDatabase.FindAssets("t:" + typeof(InventoryItemDatabase).Name);

            _gameRules = GameRulesWindow.GetAllActiveRules();
            GameRulesWindow.CheckForIssues();
            GameRulesWindow.OnIssuesUpdated += UpdateMiniToolbar;

            CreateEditors();
        }

        private void OnDisable()
        {
            GameRulesWindow.OnIssuesUpdated -= UpdateMiniToolbar;

#if UNITY_2018_3_OR_NEWER
			ItemManager.ResetItemDatabaseLookup();
			var rootGameObjects = (GameObject[])previewScene.GetRootGameObjects().Clone();
			foreach (var go in rootGameObjects)
			{
			    UnityEngine.Object.DestroyImmediate(go);
			}
#endif
		}

		internal static void UpdateMiniToolbar(List<IGameRule> issues)
        {
            window.Repaint();
        }

        public static void SelectTab(Type type)
        {
            int i = 0;
            foreach (var editor in editors)
            {
                var ed = editor as EmptyEditor;
                if (ed != null)
                {
                    bool isChildOf = ed.childEditors.Select(o => o.GetType()).Contains(type);
                    if (isChildOf)
                    {
                        toolbarIndex = i;
                        for (int j = 0; j < ed.childEditors.Count; j++)
                        {
                            if (ed.childEditors[j].GetType() == type)
                            {
                                ed.toolbarIndex = j;
                            }
                        }

                        toolbarIndex = i;
                        ed.Focus();
                        window.Repaint();
                        return;
                    }
                }

                if (editor.GetType() == type)
                {
                    toolbarIndex = i;
                    editor.Focus();
                    window.Repaint();
                    return;
                }

                i++;
            }

            Debug.LogWarning("Trying to select tab in main editor, but type isn't in editor.");
        }

        public virtual void CreateEditors()
        {
            editors.Clear();
            itemEditor = new EmptyEditor("Items editor", this);
            itemEditor.requiresDatabase = true;

#if UNITY_2018_3_OR_NEWER
			previewScene = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
			itemEditor.childEditors.Add(new ItemEditor("Item", "Items", this, previewScene));
#else
			itemEditor.childEditors.Add(new ItemEditor("Item", "Items", this));
#endif


			itemEditor.childEditors.Add(new ItemCategoryEditor("Item category", "Item categories", this) { canReOrderItems = true });
            itemEditor.childEditors.Add(new ItemStatEditor("Item stat", "Item stats", this) { canReOrderItems = true });
            itemEditor.childEditors.Add(new ItemRarityEditor("Item Rarity", "Item rarities", this) { canReOrderItems = true });
            editors.Add(itemEditor);

            var currencyEditor = new CurrencyEditor("Currency", "Currencies", this);
            currencyEditor.requiresDatabase = true;
            currencyEditor.canReOrderItems = true;
            editors.Add(currencyEditor);

            equipEditor = new EmptyEditor("Equipment editor", this);
            equipEditor.requiresDatabase = true;
#if PLY_GAME
            equipEditor.childEditors.Add(new plyStatsEditor("Ply stats", this));
#endif
            equipEditor.childEditors.Add(new EquipmentTypeEditor("EquippedItem type", "EquippedItem types", this) { canReOrderItems = true });
            editors.Add(equipEditor);

            craftingEditor = new CraftingEmptyEditor("Crafting editor", this);
            craftingEditor.requiresDatabase = true;
            editors.Add(craftingEditor);

            languageEditor = new LanguageEditor("Language", "Language categories", this);
            editors.Add(languageEditor);

            settingsEditor = new SettingsEditor("Settings", "Settings categories", this);
            editors.Add(settingsEditor);
        }

        protected virtual void DrawToolbar()
        {
            if (ItemManager.instance != null && ItemManager.itemDatabaseLookup.hasSelectedDatabase)
            {
                if (AssetDatabase.GetAssetPath(ItemManager.database) != AssetDatabase.GetAssetPath(ItemManager.instance.sceneItemDatabase))
                {
                    EditorGUILayout.HelpBox("This scene contains a different database than is currently selected.", MessageType.Warning);
                    if (GUILayout.Button("Select scene's database"))
                    {
                        ItemManager.itemDatabaseLookup.SetDatabase(ItemManager.instance.sceneItemDatabase);
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.grey;
            if (GUILayout.Button("< DB", EditorStyles.toolbarStyle, GUILayout.Width(60)))
            {
                var selected = ItemManager.itemDatabaseLookup.ManuallySelectDatabase();
                if(selected == false)
                {
                    // Create a database
                    var db = ScriptableObjectUtility.CreateAssetChooseSaveFolder<ItemDatabase>("ItemDatabase.asset", true);
                    if (db != null)
                    {
                        ItemManager.itemDatabaseLookup.SetDatabase(db);
                    }
                }

                toolbarIndex = 0;
            }
            GUI.color = Color.white;

            int before = toolbarIndex;
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, editorNames, EditorStyles.toolbarStyle);
            if (before != toolbarIndex)
                editors[toolbarIndex].Focus();
            
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawMiniToolbar()
        {
            GUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true));
            
            var issueCount = _gameRules.Sum(o => o.ignore == false ? o.issues.Count : 0);
            if (issueCount > 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.green;
            
            if (GUILayout.Button(issueCount + " issues found in scene.", "toolbarbutton", GUILayout.Width(300)))
            {
                GameRulesWindow.ShowWindow();
            }

            GUI.color = Color.white;

            if (ItemManager.itemDatabaseLookup.hasSelectedDatabase)
            {
                var style = UnityEditor.EditorStyles.centeredGreyMiniLabel;

                var r = new Rect(320, window.position.height - 18, window.position.width - 320, 20);
                GUI.Label(r, "Selected database: " + AssetDatabase.GetAssetPath(ItemManager.database), style);
            }

            GUILayout.EndVertical();
        }


        public override void OnGUI()
        {
            base.OnGUI();
            DrawToolbar();

            if (InventoryScriptableObjectUtility.isPrefabsSaveFolderSet == false)
            {
                if (settingsEditor == null)
                {
                    CreateEditors();
                }

                settingsEditor.Draw();
                return;
            }

            if (toolbarIndex < 0 || toolbarIndex >= editors.Count || editors.Count == 0)
            {
                toolbarIndex = 0;
                CreateEditors();
            }

            // Draw the editor
            editors[toolbarIndex].Draw();

            DrawMiniToolbar();
        }
    }
}