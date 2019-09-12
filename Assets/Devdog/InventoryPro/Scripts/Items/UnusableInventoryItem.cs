using UnityEngine;
using System.Collections;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to represent an item that cannot be used in any way.
    /// </summary>
    public partial class UnusableInventoryItem : InventoryItemBase
    {


        public override int Use()
        {
            base.Use();
            InventoryManager.langDatabase.itemCannotBeUsed.Show(name, description);

            return -2; // Can't use an unusable item... right?
        }
    }
}