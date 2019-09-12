using UnityEngine;
using System.Collections;


namespace Devdog.InventoryPro
{

    public class CustomCollectionAddItemConditions : MonoBehaviour
    {

        public void Start()
        {
            // Register our custom check.
            GetComponent<ItemCollectionBase>().canAddItemToCollectionConditionals.Add(CustomCollectionConditions);
        }

        private bool CustomCollectionConditions(InventoryItemBase item)
        {
            // Only allow items that are less than 20 gold in the collections.
            if (item.buyPrice.amount < 20)
                return true; // The item can be added to the collection.

            return false; // The item cannot be  added to the collection.
        }
    }
}