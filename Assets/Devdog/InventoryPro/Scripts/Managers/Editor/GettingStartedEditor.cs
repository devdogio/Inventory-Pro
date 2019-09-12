using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Devdog.General.Editors;
using UnityEditor.Callbacks;

namespace Devdog.InventoryPro.Editors
{
    [InitializeOnLoad]
    public class GettingStartedEditor : GettingStartedEditorBase
    {
        private const string MenuItemPath = InventoryPro.ToolsMenuPath + "Getting started";
        private static bool _doInit = false;

        private const string IconRootPath = "Assets/Devdog/General/EditorStyles/";
        private const string TryRucksackUri = IconRootPath + "TryRucksack.png";
        protected static Texture tryRucksackImage;


        public GettingStartedEditor()
        {
            version = InventoryPro.Version;
            productName = InventoryPro.ProductName;
            documentationUrl = "http://inventory-pro-docs.readthedocs.io/en/latest/";
            youtubeUrl = "https://www.youtube.com/watch?v=kWeXmVIgqO4&list=PL_HIoK0xBTK4R3vX9eIT1QUl-fn78eyIM";
            reviewProductUrl = "https://www.assetstore.unity3d.com/en/content/66801";
        }

        static GettingStartedEditor()
        {
            // Init
            _doInit = true;
        }

        private void OnEnable()
        {
            if (_doInit)
            {
                if (EditorPrefs.GetBool(editorPrefsKey))
                {
                    ShowWindowInternal();
                }
            }

            _doInit = false;
        }

        [MenuItem(MenuItemPath, false, 1)] // Always at bottom
        protected static void ShowWindowInternal()
        {
            window = GetWindow<GettingStartedEditor>();
            tryRucksackImage = AssetDatabase.LoadAssetAtPath<Texture>(TryRucksackUri);

            window.GetImages();
            window.ShowUtility();
        }

        public override void ShowWindow()
        {
            ShowWindowInternal();
        }

        protected override void DrawGettingStarted()
        {
            DrawBox(0, 0, "Documentation", "The official documentation has a detailed description of all components and code examples.", documentationIcon, () =>
            {
                Application.OpenURL(documentationUrl);
            });

            DrawBox(1, 0, "Video tutorials", "The video tutorials cover all interfaces and a complete set up.", videoTutorialsIcon, () =>
            {
                Application.OpenURL(youtubeUrl);
            });

            DrawBox(2, 0, "Discord", "Join the community on Discord for support.", discordIcon, () =>
            {
                Application.OpenURL(discordUrl);
            });

            DrawBox(3, 0, "Integrations", "Combine the power of assets and enable integrations.", integrationsIcon, () =>
            {
                IntegrationHelperEditor.ShowWindow();
            });

            DrawBox(4, 0, "Rate / Review", "Like " + productName + "? Share the experience :)", reviewIcon, () =>
            {
                Application.OpenURL(reviewProductUrl);
            });

            GUILayout.EndArea();

            var tryRucksackRect = new Rect(window.position.width - 409, 200, 399, 252);
            EditorGUIUtility.AddCursorRect(tryRucksackRect, MouseCursor.Link);

            GUI.DrawTexture(tryRucksackRect, tryRucksackImage);
            Event e = Event.current;
            if(e.type == EventType.MouseDown && e.button == 0 && tryRucksackRect.Contains(e.mousePosition))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/114921?aid=1101lGjC");
            }

            GUILayout.BeginArea(new Rect(0, 0, SingleColWidth, window.position.height));

            base.DrawGettingStarted();
        }
    }
}