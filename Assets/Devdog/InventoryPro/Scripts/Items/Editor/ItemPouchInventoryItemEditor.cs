using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.Editors
{
    [CustomEditor(typeof(ItemPouchInventoryItem), true)]
    public class ItemPouchInventoryItem : InventoryItemBaseEditor
    {

        public override void OnEnable()
        {
            base.OnEnable();

        }

        protected override void DrawItemStatLookup(Rect rect, SerializedProperty property, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.DrawItemStatLookup(rect, property, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {

            
            base.OnCustomInspectorGUI(extraOverride);
        }
    }
}