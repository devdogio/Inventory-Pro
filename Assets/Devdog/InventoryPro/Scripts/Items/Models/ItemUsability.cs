using System;

namespace Devdog.InventoryPro
{
    public partial class ItemUsability
    {
        public string actionName;
        public Action<InventoryItemBase> useItemCallback;
        public bool isActive;

        public ItemUsability(string actionName, Action<InventoryItemBase> useItemCallback)
        {
            this.actionName = actionName;
            this.useItemCallback = useItemCallback;
        }
    }
}