using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.General.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventoryPro.Editors
{
    public class EmptyEditor : IEditorCrud
    {
        public List<IEditorCrud> childEditors = new List<IEditorCrud>(8);

        public IEditorCrud selectedEditor
        {
            get { return childEditors[toolbarIndex]; }
        }

        public string[] editorNames
        {
            get
            {
                string[] names = new string[childEditors.Count];
                for (int i = 0; i < childEditors.Count; i++)
                {
                    names[i] = childEditors[i].ToString();
                }

                return names;
            }
        }

        public int toolbarIndex;
        public string name { get; set; }
        public EditorWindow window { get; protected set; }
        public bool requiresDatabase { get; set; }

        public EmptyEditor(string name, EditorWindow window)
        {
            this.name = name;
            this.window = window;
            this.toolbarIndex = 0;
            this.requiresDatabase = false;
        }

        public virtual void Focus()
        {
            //if (selectedEditor != null)
            selectedEditor.Focus();
        }

        protected virtual void DrawToolbar()
        {
            if (toolbarIndex >= childEditors.Count || toolbarIndex < 0)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            int maxHorizontalTabs = Mathf.FloorToInt(window.position.width / 220);
            maxHorizontalTabs = Mathf.Max(2, maxHorizontalTabs);
            for (int i = 0; i < childEditors.Count; i++)
            {
                string buttonType = "LargeButtonMid";
                if (i == 0 || i % maxHorizontalTabs == 0)
                    buttonType = "LargeButtonLeft";
                else if (i == childEditors.Count - 1 || i % maxHorizontalTabs == maxHorizontalTabs - 1)
                    buttonType = "LargeButtonRight"; 

                if(i == toolbarIndex)
                    GUI.color = Color.gray;

                if (i % maxHorizontalTabs == 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                }

                if (GUILayout.Button(editorNames[i], buttonType))
                {
                    toolbarIndex = i;
                    selectedEditor.Focus();
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Empty editor only draws child options
        /// </summary>
        public virtual void Draw()
        {
            DrawToolbar();

            for (int i = 0; i < childEditors.Count; i++)
            {
                if (childEditors[i] == selectedEditor)
                    childEditors[i].Draw();
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
