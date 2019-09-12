using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Devdog.General;
using Devdog.General.Editors;
using UnityEditor;
using UnityEngine;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public partial class CreateNewItemEditor : EditorWindow
    {

        //private Editor itemPicker;
        private ScriptPickerEditor picker { get; set; }
        private int step = 0;
        private System.Type firstStepType { get; set; }

        private GameObject _step2Model;
        public bool forceFocus { get; private set; }

        private GameObject step2Model
        {
            get
            {
                return _step2Model;
            }
            set
            {
                if (_step2Model != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_step2Model)))
                    DestroyImmediate(_step2Model);

                _step2Model = value;
            }
        }
        public System.Action<System.Type, GameObject, EditorWindow> callback { get; set; }


        public static EditorWindow Get(System.Action<System.Type, GameObject, EditorWindow> callback, string windowTitle = "Create new item")
        {
            var window = EditorWindow.GetWindow<CreateNewItemEditor>(true, "Create a new item", true);
            window.minSize = new Vector2(400, 500);
            window.maxSize = new Vector2(400, 500);
            window.titleContent = new GUIContent(windowTitle);
            window.callback = callback;
            window.forceFocus = false;

            return window;
        }

        public void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            step = 0;
            firstStepType = null;
            step2Model = null;

            picker = ScriptPickerEditor.Get(typeof(InventoryItemBase));
            picker.Show(true);
            picker.Close();

            picker.OnPickObject += type =>
            {
                firstStepType = type;
                step++;

                Repaint();
            };

            Focus();
        }

        public void OnGUI()
        {
            // Recompiled or something?? No callback found
            if (callback == null)
                Close();

            if(forceFocus)
                EditorWindow.FocusWindowIfItsOpen<CreateNewItemEditor>();
            
            var r = new Rect(0,0,390,18);

            GUI.color = Color.gray;

            if (step == 0)
                GUI.color = Color.white;

            if (GUI.Button(r, "Step 1", "GUIEditor.BreadcrumbLeft"))
            {
                Reset();
            }
            GUI.color = Color.gray;

            r.width /= 2;
            r.x += r.width;
            if (step == 1)
                GUI.color = Color.white;
            
            if (GUI.Button(r, "Step 2", "GUIEditor.BreadcrumbMid"))
            { }

            GUI.color = Color.white;

            EditorGUILayout.BeginVertical();
            GUILayout.Space(30);

            if (step == 0)
                Step1();
            if (step == 1)
                Step2();

            EditorGUILayout.EndVertical();
        }


        public void Step1()
        {
            // Otherwise it repaints to late...
            if (Event.current.isKey)
            {
                picker.Repaint();
                Repaint();
            }


            // Draw inside ...
            picker.OnGUI();
        }

        public void Step2()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Selected type: " + firstStepType.Name, (GUIStyle)"BoldLabel");


            var style = new GUIStyle(EditorStyles.boxStyle);
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cube (Default)", GUILayout.ExpandWidth(true), GUILayout.Height(30)) || step2Model == null)
            {
                // Use a box
                step2Model = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            if (GUILayout.Button("None (Be warned...)", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                step2Model = new GameObject("__Empty");
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("2D sprite with trigger", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                // Use a box
                step2Model = new GameObject("2D Sprite");
                step2Model.GetOrAddComponent<SpriteRenderer>();
                var col = step2Model.GetOrAddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }



            ShowOr();


            EditorGUILayout.BeginVertical();
            var boxStyle = new GUIStyle("HelpBox");
            boxStyle.stretchWidth = true;
            boxStyle.fixedHeight = 200;
            boxStyle.alignment = TextAnchor.MiddleCenter;
            var rect = GUILayoutUtility.GetRect(390, 390, 200, 200);
            rect.x += 5;
            rect.width = 390;


            #region Accepting drag for box

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition) == false)
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        if (DragAndDrop.objectReferences.Length == 1)
                        {
                            step2Model = DragAndDrop.objectReferences[0] as GameObject;
                            var sprite = DragAndDrop.objectReferences[0] as Sprite;

                            if (step2Model != null)
                            {
                                DragAndDrop.AcceptDrag();
                            }
                            else
                            {
                                if (sprite != null)
                                {
                                    DragAndDrop.AcceptDrag();

                                    step2Model = new GameObject("2D Sprite");
                                    var spr = step2Model.GetOrAddComponent<SpriteRenderer>();
                                    spr.sprite = sprite;

                                    var col = step2Model.GetOrAddComponent<BoxCollider2D>();
                                    col.isTrigger = true;
                                }
                            }
                        }

                    }
                    break;
            }

            #endregion


            if (step2Model == null)
                GUI.Box(rect, "Drag object here", boxStyle);
            else
            {
                var rect2 = rect;
                rect2.width /= 2;

                Texture2D preview = AssetPreview.GetAssetPreview(step2Model);
                if (preview != null)
                {
                    EditorGUI.DrawPreviewTexture(rect2, preview);

                    rect.width -= rect2.width;
                    rect.x += rect2.width;
                }

                GUI.Box(rect, step2Model.name, boxStyle);
            }
            GUI.color = Color.white;


            ShowOr();


            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == 123)
                {
                    step2Model = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                    forceFocus = true;                    
                }
            }
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == 124)
                {
                    var sprite = (Sprite)EditorGUIUtility.GetObjectPickerObject();

                    step2Model = new GameObject("2D Sprite");
                    var spr = step2Model.GetOrAddComponent<SpriteRenderer>();
                    spr.sprite = sprite;

                    var col = step2Model.GetOrAddComponent<BoxCollider2D>();
                    col.isTrigger = true;

                    forceFocus = true;                    
                }
            }
            if (GUILayout.Button("Select model", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                EditorGUIUtility.ShowObjectPicker<GameObject>(step2Model, false, "", 123);
                forceFocus = false;
            }
            if (GUILayout.Button("Select sprite", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                EditorGUIUtility.ShowObjectPicker<Sprite>(step2Model, false, "", 124);
                forceFocus = false;
            }
            EditorGUILayout.EndVertical();

            if (step2Model == null)
                GUI.enabled = false;

            GUI.color = Color.green;
            if (GUILayout.Button("Create item", (GUIStyle) "LargeButton"))
            {
                if(step2Model != null)
                    CreateItem(firstStepType, step2Model);

            }
            GUI.color = Color.white;
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private void CreateItem(System.Type type, GameObject model)
        {
            if (callback != null)
                callback(type, model, this);
        }

        private void ShowOr()
        {
            GUILayout.BeginHorizontal();
            var r = GUILayoutUtility.GetRect(400, 400, 20, 20);
            r.width = 180;
            r.y += 8;
            GUI.Label(r, "", "sv_iconselector_sep");

            r.width = 30;
            r.y -= 8;
            r.x += 186;
            GUI.Label(r, "OR");

            r.width = 180;
            r.y += 8;
            r.x += 30;
            GUI.Label(r, "", "sv_iconselector_sep");
            GUILayout.EndHorizontal();
        }
    }
}
