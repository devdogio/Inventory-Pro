using System;
using Devdog.General.Editors;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(CraftingCategory))]
    public class CraftingCategoryObjectPickerEditor : InventoryProObjectPickerBase
    {
        protected override string GetObjectName(Object asset)
        {
            var c = (CraftingCategory)asset;
            return c.name;
        }
    }
}
