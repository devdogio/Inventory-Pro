using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public interface IInventoryItemContainer
    {
        string uniqueName { get; }

        InventoryItemBase[] items { get; set; }
    }
}
