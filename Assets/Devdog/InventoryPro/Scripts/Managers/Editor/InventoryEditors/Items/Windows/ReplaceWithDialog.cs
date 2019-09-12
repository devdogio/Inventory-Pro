using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class ReplaceWithDialog : EditorWindow
    {

        public System.Action<int, EditorWindow> callback { get; set; }

        protected int selectedIndex = 0;
        protected string[] names = new string[0];

        protected string typeName = "";
        protected int referenceCount = 0;

        public static EditorWindow Get(System.Action<int, EditorWindow> callback, string typeName, int referenceCount, string[] names, string windowTitle = "Replace type with")
        {
            var window = EditorWindow.GetWindow<ReplaceWithDialog>(true, "Replace type with", true);
            window.minSize = new Vector2(400, 260);
            window.maxSize = new Vector2(400, 260);
            window.titleContent = new GUIContent(windowTitle);
            window.callback = callback;
            window.names = names;

            window.typeName = typeName;
            window.referenceCount = referenceCount;

            Assert.IsNotNull(callback, "Callback is required");

            return window;
        }


        public virtual void OnGUI()
        {
            // Recompiled or something?? No callback found
            if (callback == null)
            {
                Close();
                return;
            }

            EditorGUILayout.LabelField("This " + typeName + " is referenced " + referenceCount + " times. You can replace it with a different " + typeName + " by selecting one from the list below.", EditorStyles.labelStyle);

            selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

            if (GUILayout.Button("Replace"))
            {
                callback(selectedIndex, this);
            }
            if (GUILayout.Button("Don't replace"))
            {
                callback(-1, this);
            }
        }
    }
}
