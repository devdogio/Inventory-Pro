using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class CollectionSorterBase : ScriptableObject
    {

        public abstract IList<InventoryItemBase> Sort(IList<InventoryItemBase> items);

    }
}
