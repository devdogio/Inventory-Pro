using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.General.Editors;
using Devdog.InventoryPro.UI;

namespace Devdog.InventoryPro.Editors
{
    using System.Linq;

    [CustomEditor(typeof(CurrencyUI), true)]
    [CanEditMultipleObjects()]
    public class CurrencyUIEditor : InventoryEditorBase
    {      
        private SerializedProperty _currency;
        
        public override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            _currency = serializedObject.FindProperty("_currency");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnCustomInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>(_currency);
            var doNotDrawList = new List<string>()
            {
                "m_Script",
                _currency.name
            };

            foreach (var extra in extraOverride)
            {
                extra.action();
                doNotDrawList.Add(extra.serializedName);
            }

            DrawPropertiesExcluding(serializedObject, doNotDrawList.ToArray());
        }
    }
}