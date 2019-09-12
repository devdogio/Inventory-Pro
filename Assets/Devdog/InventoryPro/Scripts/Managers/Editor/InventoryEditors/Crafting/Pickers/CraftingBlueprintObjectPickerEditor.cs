using System;
using Devdog.General.Editors;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(CraftingBlueprint))]
    public class CraftingBlueprintObjectPickerEditor : InventoryProObjectPickerBase
    {
        protected override string GetObjectName(Object asset)
        {
            var c = (CraftingBlueprint)asset;
            return c.name;
        }
    }
}
