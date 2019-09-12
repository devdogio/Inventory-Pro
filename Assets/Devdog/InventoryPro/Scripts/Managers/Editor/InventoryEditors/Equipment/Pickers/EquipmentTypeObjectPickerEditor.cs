using System;
using Devdog.General.Editors;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(EquipmentType))]
    public class EquipmentTypeObjectPickerEditor : InventoryProObjectPickerBase
    {
        protected override string GetObjectName(Object asset)
        {
            var c = (EquipmentType)asset;
            return c.name;
        }
    }
}
