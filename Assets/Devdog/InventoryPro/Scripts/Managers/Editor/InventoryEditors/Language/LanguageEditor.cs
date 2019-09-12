using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General;
using Devdog.General.Editors;
using UnityEditor;
using UnityEngine;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class LanguageEditor : EditorCrudBase<LanguageEditor.Lookup>
    {
        public class Lookup
        {
            public string name { get; set; }

            public List<SerializedProperty> serializedProperties = new List<SerializedProperty>(8);

            public Lookup()
            {
                
            }

            public Lookup(string name)
            {
                this.name = name;
            }

            public override bool Equals(object obj)
            {
                var o = obj as Lookup;
                return o != null && o.name == name;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private SerializedObject _serializedObject;
        public SerializedObject serializedObject
        {
            get
            {
                if (_serializedObject == null)
                    _serializedObject = new SerializedObject(language);

                return _serializedObject;
            }
        }



        private LangDatabase _language;
        protected LangDatabase language
        {
            get
            {
                if (_language == null)
                    _language = InventoryManager.langDatabase;

                return _language;
            }
        }


        protected override List<Lookup> crudList
        {
            get
            {
                var list = new List<Lookup>(8);
                if (language != null)
                {
                    var fields = language.GetType().GetFields();

                    Lookup currentCategory = null;
                    foreach (var field in fields)
                    {
                        var customAttributes = field.GetCustomAttributes(typeof(CategoryAttribute), true);
                        if (customAttributes.Length == 1)
                        {
                            // Got a category marker
                            currentCategory = new Lookup(customAttributes[0].ToString());
                            list.Add(currentCategory);
                        }

                        if (currentCategory != null)
                        {
                            var prop = serializedObject.FindProperty(field.Name);
                            if (prop != null)
                                currentCategory.serializedProperties.Add(prop);
                        }
                    }
                }

                return list;
            }
            set
            {
                // Doesn't do anything...
            }
        }

        public LanguageEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            this.canCreateItems = false;
            this.canDeleteItems = false;
            this.canDuplicateItems = false;
            this.canReOrderItems = false;
            this.hideCreateItem = true;
        }

        protected override void CreateNewItem()
        {

        }

        public override void DuplicateItem(int index)
        {

        }

        protected override bool MatchesSearch(Lookup category, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return category.name.ToLower().Contains(search) || category.serializedProperties.Any(o => o.displayName.ToLower().Contains(search));
        }

        protected bool MatchesSearch(SerializedProperty property, string searchQuery)
        {
            return property.displayName.ToLower().Contains(searchQuery.ToLower());
        }

        protected override void DrawSidebarRow(Lookup category, int i)
        {
            BeginSidebarRow(category, i);

            DrawSidebarRowElement(category.name, 400);

            EndSidebarRow(category, i);
        }


        protected override void DrawDetail(Lookup category, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = EditorStyles.labelWidth;


            SerializedProperty toHighlight = null;
            if(serializedObject != null)
            {
                serializedObject.Update();
                foreach (var setting in category.serializedProperties)
                {
                    EditorGUILayout.PropertyField(setting, true);
                    if (MatchesSearch(setting, searchQuery) && toHighlight == null)
                    {
                        toHighlight = setting;
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();
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
