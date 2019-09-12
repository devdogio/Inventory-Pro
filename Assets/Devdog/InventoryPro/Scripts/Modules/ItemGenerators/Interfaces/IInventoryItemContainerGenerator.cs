using System;
using System.Collections.Generic;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public partial interface IInventoryItemContainerGenerator
    {
        IInventoryItemContainer container { get; }

        IItemGenerator generator { get; }
    }
}