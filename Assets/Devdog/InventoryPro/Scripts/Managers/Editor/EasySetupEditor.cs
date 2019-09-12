using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using System.Reflection;
using Devdog.General;
using Devdog.General.Editors;
using Devdog.General.Editors.GameRules;
using Devdog.General.Localization;
using Devdog.InventoryPro.Dialogs;
using UnityEditor.Callbacks;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EditorStyles = UnityEditor.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    [InitializeOnLoad]
    public class EasySetupEditor : EditorWindow
    {
        private const string MenuItemPath = InventoryPro.ToolsMenuPath + "Easy Setup";
        private static int _stepIndex = 0;

        public EasySetupEditor()
        {

        }

        [MenuItem(MenuItemPath, false, 1)] // Always at bottom
        protected static void ShowWindowInternal()
        {
            var window = GetWindow<EasySetupEditor>();
            window.titleContent = new GUIContent("Inventory Pro - Easy Setup");
            window.minSize = new Vector2(400, 300);
            window.maxSize = new Vector2(400, 600);
            window.Show();
        }

        protected void OnGUI()
        {
            var r = new Rect(Vector2.zero, new Vector2(position.size.x, 20f));
            _stepIndex = GUI.Toolbar(r, _stepIndex, new string[] {"Step 1", "Step 2", "Step 3", "Step 4", "Step 5"}, "GUIEditor.BreadcrumbMid");
            
            GUI.BeginGroup(new Rect(0f, 30f, position.width, position.height - 30f));
            switch (_stepIndex)
            {
                case 0:
                    DrawStep1();
                    break;
                case 1:
                    DrawStep2();
                    break;
                case 2:
                    DrawStep3();
                    break;
                case 3:
                    DrawStep4();
                    break;
                case 4:
                    DrawStep5();
                    break;
                default:
                    break;
            }
            GUI.EndGroup();
        }

        private void GoToNextStep()
        {
            _stepIndex++;
            Repaint();
        }

        private void GoToPreviousStep()
        {
            _stepIndex--;
            Repaint();
        }

        private void DrawStep1()
        {
            if (GUILayout.Button("Add all managers"))
            {
                var managers = GameObject.Find("_Managers");
                if (managers == null)
                {
                    managers = new GameObject("_Managers");
                }

                managers.GetOrAddComponent<GeneralSettingsManager>();
                managers.GetOrAddComponent<AudioManager>();
                managers.GetOrAddComponent<InputManager>();
                managers.GetOrAddComponent<PlayerManager>();
                managers.GetOrAddComponent<TriggerManager>();
                managers.GetOrAddComponent<LocalizationManager>();

                managers.GetOrAddComponent<InventoryManager>();
                managers.GetOrAddComponent<InventoryPlayerManager>();
                managers.GetOrAddComponent<InventorySettingsManager>();
                managers.GetOrAddComponent<ItemManager>();

                GoToNextStep();
            }

            if (GUILayout.Button("Skip this step"))
            {
                GoToNextStep();
            }
        }

        private void DrawStep2()
        {
            var managers = GameObject.Find("_Managers");
            if(managers != null)
            {
                var inventoryManager = managers.GetOrAddComponent<InventoryManager>();
                var generalSettingsManager = managers.GetOrAddComponent<GeneralSettingsManager>();
                var localizationManager = managers.GetOrAddComponent<LocalizationManager>();
                var inventorySettingsManager = managers.GetOrAddComponent<InventorySettingsManager>();
                var itemManager = managers.GetOrAddComponent<ItemManager>();
                if (inventoryManager != null)
                {
                    if (GUILayout.Button("Generate all databases"))
                    {
                        var path = UnityEditor.EditorUtility.SaveFolderPanel("Save databases path", "Assets/", "");
                        if (string.IsNullOrEmpty(path) == false)
                        {
                            inventoryManager.sceneLangDatabase = ScriptableObjectUtility.CreateAsset<LangDatabase>(path, "LangDatabase.asset", false);
                            generalSettingsManager.settings = ScriptableObjectUtility.CreateAsset<GeneralSettings>(path, "GeneralSettings.asset", false);
                            var dbField = typeof(LocalizationManager).GetField("_databases", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            Assert.IsNotNull(dbField, "Couldn't find _databases on LocalizationManager!");
                            var enUs = ScriptableObjectUtility.CreateAsset<Devdog.General.Localization.LocalizationDatabase>(path, "enUS.asset", false);
                            enUs.lang = "en-US";
                            dbField.SetValue(localizationManager, new [] { enUs });

                            inventorySettingsManager.settings = ScriptableObjectUtility.CreateAsset<InventorySettingsDatabase>(path, "InventorySettings.asset", false);
                            itemManager.sceneItemDatabase = ScriptableObjectUtility.CreateAsset<ItemDatabase>(path, "ItemDatabase.asset", false);

                            inventorySettingsManager.settings.collectionSorter = ScriptableObjectUtility.CreateAsset<BasicCollectionSorter>(path, "BasicCollectionSorter.asset", false);
                            inventorySettingsManager.settings.itemDropHandler = ScriptableObjectUtility.CreateAsset<ItemDropHandler>(path, "ItemDropHandler.asset", false);
                            inventorySettingsManager.settings.itemButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Devdog/InventoryPro/Demos/Assets/UI/UI_Prefabs/UI_Item_PFB.prefab");

                            UnityEditor.EditorUtility.SetDirty(inventoryManager);
                            UnityEditor.EditorUtility.SetDirty(generalSettingsManager);
                            UnityEditor.EditorUtility.SetDirty(inventorySettingsManager);
                            UnityEditor.EditorUtility.SetDirty(itemManager);
                            UnityEditor.EditorUtility.SetDirty(inventorySettingsManager.settings);

                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            GoToNextStep();
                        }
                    }

                    if (GUILayout.Button("Skip this step"))
                    {
                        GoToNextStep();
                    }
                }
                else
                {
                    GoToPreviousStep();
                }
            }
        }

        private void DrawStep3()
        {
            if (GUILayout.Button("Generate Canvas (UI)"))
            {
                var c = new GameObject("Canvas");
                c.layer = 5; // UI Layer
                c.AddComponent<Canvas>();
                c.AddComponent<CanvasScaler>();
                c.AddComponent<GraphicRaycaster>();

                if (FindObjectOfType<EventSystem>() == null)
                {
                    var ev = new GameObject("EventSystem");
                    ev.AddComponent<EventSystem>();
                    ev.AddComponent<StandaloneInputModule>();
                }

                GoToNextStep();
            }

            if (GUILayout.Button("Skip this step"))
            {
                GoToNextStep();
            }
        }

        private void DrawStep4()
        {
            var inventoryManager = FindObjectOfType<InventoryManager>();
            if (GUILayout.Button("Link all UI references"))
            {
                inventoryManager.bank = FindObjectOfType<BankUI>();
                inventoryManager.loot = FindObjectOfType<LootUI>();
                inventoryManager.vendor = FindObjectOfType<VendorUI>();
                inventoryManager.notice = FindObjectOfType<NoticeUI>();
                inventoryManager.craftingStandard = FindObjectOfType<CraftingWindowStandardUI>();
                inventoryManager.craftingLayout = FindObjectOfType<CraftingWindowLayoutUI>();
                inventoryManager.contextMenu = FindObjectOfType<InventoryContextMenu>();

                inventoryManager.uiRoot = FindObjectOfType<Canvas>();

                inventoryManager.confirmationDialog = FindObjectOfType<ConfirmationDialog>();
                inventoryManager.buySellDialog = FindObjectOfType<ItemBuySellDialog>();
                inventoryManager.intValDialog = FindObjectOfType<IntValDialog>();
                inventoryManager.unstackDialog = FindObjectOfType<ItemIntValDialog>();

                GoToNextStep();
            }

            if (GUILayout.Button("Skip this step"))
            {
                GoToNextStep();
            }
        }

        private void DrawStep5()
        {
            // All done
            GUILayout.Label("All done!");
            if (GUILayout.Button("Check issue detector"))
            {
                GameRulesWindow.ShowWindow();
            }

            if (GUILayout.Button("Close window"))
            {
                Close();
            }
        }
    }
}