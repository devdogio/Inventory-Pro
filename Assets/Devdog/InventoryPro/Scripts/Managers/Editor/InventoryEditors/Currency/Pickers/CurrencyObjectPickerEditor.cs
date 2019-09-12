using System;
using Devdog.General.Editors;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro.Editors
{
    [CustomObjectPicker(typeof(CurrencyDefinition))]
    public class CurrencyObjectPickerEditor : InventoryProObjectPickerBase
    {
        public override bool IsSearchMatch(Object asset, string searchQuery)
        {
            var c = (CurrencyDefinition)asset;
            searchQuery = searchQuery.ToLower();

            return c.singleName.ToLower().Contains(searchQuery) ||
                   c.pluralName.ToLower().Contains(searchQuery) ||
                   asset.GetType().Name.ToLower().Contains(searchQuery);
        }

        protected override string GetObjectName(Object asset)
        {
            var c = (CurrencyDefinition)asset;
            return c.singleName;
        }
    }
}
