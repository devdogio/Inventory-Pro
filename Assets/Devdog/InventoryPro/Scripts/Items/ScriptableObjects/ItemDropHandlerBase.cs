using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class ItemDropHandlerBase : ScriptableObject
    {

        public abstract GameObject DropItem(InventoryItemBase item);
        public abstract GameObject DropItem(InventoryItemBase item, Vector3 position, Quaternion rotation);
        public abstract Vector3? CalculateDropPosition(InventoryItemBase item);

    }
}
