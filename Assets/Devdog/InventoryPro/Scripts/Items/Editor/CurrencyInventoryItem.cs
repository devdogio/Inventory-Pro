using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General.Editors;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(CurrencyInventoryItem), true)]
    public class CurrencyInventoryItemEditor : InventoryItemBaseEditor
    {
        private SerializedProperty _currency;
        public override void OnEnable()
        {
            base.OnEnable();
            _currency = serializedObject.FindProperty("_currency");
        }

        protected override void DrawItemStatLookup(Rect rect, SerializedProperty property, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.DrawItemStatLookup(rect, property, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            var l = new List<CustomOverrideProperty>(extraOverride)
            {
                new CustomOverrideProperty(_currency.name, () =>
                {
                    ObjectPickerUtility.RenderObjectPickerForType<CurrencyDefinition>(_currency);
                })
            };

            base.OnCustomInspectorGUI(l.ToArray());
        }
    }
}