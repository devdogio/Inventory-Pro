using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Simple collection sorter that sorts all items based on type name, category name and item name (in that order).
    /// </summary>
    [CreateAssetMenu(menuName = InventoryPro.CreateAssetMenuPath + "Basic Collection Sorter")]
    public class BasicCollectionSorter : CollectionSorterBase
    {

        protected BasicCollectionSorter()
        {
            
        }


        public override IList<InventoryItemBase> Sort(IList<InventoryItemBase> items)
        {
            // Group them by ID, this will exceed stack limit.
            var test = items.OrderByDescending(o => o.layoutSize).GroupBy(o => o.ID).
                Select(o => new { item = o.First(), newStackSize = o.Sum(i => i.currentStackSize) }). // Get the items so we can re-assemble the items.
                ToList();
        
            // Move the items to a manageable array, still exceeding stack limits.
            var sortedList = new List<InventoryItemBase>(test.Count);
        
            for (int i = 0; i < test.Count; i++)
            {
                //sortedList.Add(test[i].item);
                var item = test[i].item;

                item.currentStackSize = (uint)test[i].newStackSize;

                // Keep going until the stack is divided into parts of maxStackSizes
                uint currentStackSize = item.currentStackSize;
                while (currentStackSize > item.maxStackSize)
                {
                    //// The old item becomes the max stack size, the remainder is pushed to the next index,
                    //// in the next loop, maxStackSize will be removed of that, and on and on until the stack is no longer to large.
                    var a = UnityEngine.Object.Instantiate<InventoryItemBase>(item); // Copy the item
                    a.currentStackSize = item.maxStackSize;
                    sortedList.Add(a);

                    currentStackSize -= item.maxStackSize;
                }
                if(currentStackSize > 0)
                {
                    // Got 1 pile left
                    var a = UnityEngine.Object.Instantiate<InventoryItemBase>(item); // Copy the item
                    a.currentStackSize = currentStackSize;
                    sortedList.Add(a);
                }
            }

            // Because the sorter creates copies we can remove the old
            test.ForEach(o => Object.Destroy(o.item.gameObject));
        

            // Orders by category.ID but can easilly be switched to anything else.
            // Simply change o => o.category.ID to any object variable to sort by.
            // For example: o => o.buyPrice will sort items based on the buying price.
            // Another examlpe: o => o.name will sort items on an alphabetical order.
            // If you want to go wild you can chain OrderBy's this allows you to filter on the first category first (for example category), then rarity. (check uncommented line below)
            return sortedList.OrderBy(o => o.GetType().Name).ThenByDescending(o => o.layoutSize).ThenBy(o => o.category.ID).ThenBy(o => o.name).ToArray(); // Order by and return
            //return sortedList.OrderBy(o => o.category.ID).ThenBy(o => o.rarity.ID).ToArray(); // Order by and return	    
        }
    }
}