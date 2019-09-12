using System;
using System.Collections.Generic;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public interface IItemsLoader
    {
        void LoadItems(Action<object> callback);
    }
}