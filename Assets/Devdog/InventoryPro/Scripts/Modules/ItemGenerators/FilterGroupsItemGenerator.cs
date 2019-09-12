using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public partial class FilterGroupsItemGenerator : IItemGenerator
    {
        public InventoryItemGeneratorItem[] items { get; set; }

        public InventoryItemBase[] result { get; set; }

        public InventoryItemGeneratorFilterGroup[] filterGroups = new InventoryItemGeneratorFilterGroup[0];

        protected static System.Random randomGen;

        
        public FilterGroupsItemGenerator(InventoryItemGeneratorFilterGroup[] filterGroups)
        {
            RandomizeSeed();
            this.filterGroups = filterGroups;
        }

        public void RandomizeSeed()
        {
            randomGen = new System.Random(System.DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Generate an array of items.
        /// InventoryItemGeneratorItem's chance is only affected after all the filters are applied, so the item might still be rejected by type, category, etc.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public InventoryItemBase[] Generate(int amount, bool createInstances = false)
        {
            return Generate(amount, amount, createInstances);
        }

        /// <summary>
        /// Generate an array of items.
        /// InventoryItemGeneratorItem's chance is only affected after all the filters are applied, so the item might still be rejected by type, category, etc.
        /// </summary>
        /// <param name="minAmount"></param>
        /// <param name="maxAmount"></param>
        /// <returns></returns>
        public InventoryItemBase[] Generate(int minAmount, int maxAmount, bool createInstances = false)
        {
            RandomizeSeed();

            var toReturn = new List<InventoryItemBase>(maxAmount);

            // No groups, fall back to the the basic generator.
            if (filterGroups.Length == 0)
            {
                var basic = new BasicItemGenerator();
                basic.items = items;
                result = basic.Generate(minAmount, maxAmount, createInstances);
                return result;
            }

            foreach (var filterGroup in filterGroups)
            {
                if (toReturn.Count >= maxAmount)
                    break;

                var l = new List<InventoryItemBase>(filterGroup.maxAmount);
                foreach (int i in Enumerable.Range(0, items.Count()).OrderBy(x => randomGen.Next()))
                {
                    if (Random.value > filterGroup.itemsChanceFactor)
                        continue;

                    if (l.Count > filterGroup.minAmount)
                    {
                        float dif = 1.0f / (filterGroup.maxAmount - filterGroup.minAmount); // Example:  10 - 8 = 2 --- 1.0f / 2 = 0.5f // 50% chance of stopping here
                        if (Random.value > dif)
                            break;

                    }

                    if (l.Count >= filterGroup.maxAmount)
                        break;

                    if (items[i].item == null)
                        continue;

                    
                    bool abbiding = filterGroup.IsItemAbidingFilters(items[i].item);
                    if (abbiding)
                    {
                        if (createInstances)
                        {
                            var item = GameObject.Instantiate<InventoryItemBase>(items[i].item);
                            item.currentStackSize = (uint)Random.Range(filterGroup.minStackSize, filterGroup.maxStackSize);
                            item.gameObject.SetActive(false);
                            l.Add(item);
                        }
                        else
                        {
                            l.Add(items[i].item);
                        }
                    }
                }

                toReturn.AddRange(l);
            }

            result = toReturn.ToArray();
            return result;
        }

        public void SetItems(InventoryItemBase[] toSet, float chance = 1.0f)
        {
            items = new InventoryItemGeneratorItem[toSet.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new InventoryItemGeneratorItem(toSet[i], chance);
            }
        }
    }
}