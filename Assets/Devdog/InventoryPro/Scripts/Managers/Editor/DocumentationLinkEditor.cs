using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace Devdog.InventoryPro.Editors
{
    public class DocumentationLinkEditor : EditorWindow
    {


        [MenuItem("Tools/Inventory Pro/Documentation", false, 99)] // Always at bottom
        public static void ShowWindow()
        {
            Application.OpenURL("http://inventory-pro-docs.readthedocs.io/en/latest/");
        }
    }
}