using System;
using Devdog.General.Editors;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(StatDefinition))]
    public class ItemPropertyObjectPickerEditor : InventoryProObjectPickerBase
    {
        protected override string GetObjectName(Object asset)
        {
            var c = (StatDefinition)asset;
            return c.statName;
        }
    }
}
