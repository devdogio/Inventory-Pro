using UnityEngine;
using System.Collections;


namespace Devdog.InventoryPro
{

    public class CustomItemUseConditions : MonoBehaviour
    {

        public void Awake()
        {
            // Register our custom check.
            InventoryItemBase.canUseItemConditionals.Add(CustomItemConditions);
        }

        private bool CustomItemConditions(InventoryItemBase item)
        {
            // Only allow the user to use an item if the buy price is higher than 5
            if (item.buyPrice.amount > 5)
                return true; // The item can be used.

            return false; // The item cannot be used.
        }

        private bool CustomItemConditions2(InventoryItemBase item)
        {
            // Only allow the user to use an item if the buy price is higher than 5
            if (item is EquippableInventoryItem)
            {
                var equippable = (EquippableInventoryItem) item;
                return !equippable.isEquipped; // Only allow the equippable to be used as long as it's not equipped.
            }

            return false; // The item cannot be used.
        }
    }
}