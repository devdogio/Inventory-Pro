using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;


namespace Devdog.InventoryPro
{
    public partial class ExtendedInventoryItemSerializationModel : InventoryItemSerializationModel
    {
        public override void FromItem(InventoryItemBase item)
        {
            UnityEngine.Debug.Log("From extended...");
            base.FromItem(item);
        }

        public override InventoryItemBase ToItem()
        {
            UnityEngine.Debug.Log("From extended...");
            return base.ToItem();
        }
    }
}
