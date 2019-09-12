using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public partial class BasicItemGenerator : IItemGenerator
    {
        public InventoryItemGeneratorItem[] items { get; set; }

        public InventoryItemBase[] result { get; set; }

        ///// <summary>
        ///// Allow the same item multiple times
        ///// </summary>
        //public bool allowDoubles = false;

        public int minRequiredLevel = 0;
        public int maxRequiredLevel = 999999;

        public int minBuyPrice = 0;
        public int maxBuyPrice = 999999;

        public int minSellPrice = 0;
        public int maxSellPrice = 999999;

        public float minWeight = 0.0f;
        public float maxWeight = 999999.0f;

        //public int minStackSize = 0;
        //public int maxStackSize = 999999;

        public List<StatDefinition> onlyWithPoperty = new List<StatDefinition>();
        public List<ItemCategory> onlyOfCategory = new List<ItemCategory>();
        public List<ItemRarity> onlyOfRarity = new List<ItemRarity>();
        public List<System.Type> onlyOfType = new List<System.Type>();

        protected static System.Random randomGen;

        
        public BasicItemGenerator()
        {
            RandomizeSeed();
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
            var toReturn = new List<InventoryItemGeneratorItem>(maxAmount);

            foreach (int i in Enumerable.Range(0, items.Count()).OrderBy(x => randomGen.Next()))
            {
                var generatorItem = items[i];

                if(toReturn.Count > minAmount)
                {
                    float dif = 1.0f / (maxAmount - minAmount); // Example:  10 - 8 = 2 --- 1.0f / 2 = 0.5f // 50% chance of stopping here
                    if (Random.value > dif)
                        break;
                }
                if (toReturn.Count >= maxAmount)
                    break;

                if (Random.value > generatorItem.chanceFactor)
                    continue;

                var item = generatorItem.item;
                if (generatorItem.item == null)
                    continue;

                // First check all the types and rarity's, categories, as they affect the most items.
                if (onlyOfType.Count > 0 && onlyOfType.Contains(item.GetType()) == false)
                    continue;

                if (onlyOfRarity.Count > 0 && onlyOfRarity.Contains(item.rarity) == false)
                    continue;

                if (onlyOfCategory.Count > 0 && onlyOfCategory.Contains(item.category) == false)
                    continue;

                int hasProps = 0;
                foreach (var prop in onlyWithPoperty)
                {
                    if (onlyWithPoperty.Contains(prop))
                        hasProps++;
                }

                if (onlyWithPoperty.Count > 0 && hasProps < onlyWithPoperty.Count)
                    continue;


                // Check all other values
                if (item.requiredLevel < minRequiredLevel || item.requiredLevel > maxRequiredLevel)
                    continue;

                if (item.buyPrice != null)
                {
                    if (item.buyPrice.amount < minBuyPrice || item.buyPrice.amount > maxBuyPrice)
                    {
                        continue;
                    }
                }

                if (item.sellPrice != null)
                {
                    if (item.sellPrice.amount < minSellPrice || item.sellPrice.amount > maxSellPrice)
                    {
                        continue;
                    }
                }

                if (item.weight < minWeight || item.weight > maxWeight)
                    continue;

                //if(item.maxStackSize < minStackSize || item.maxStackSize > maxStackSize)
                    //continue;

                toReturn.Add(generatorItem);
            }


            if (createInstances)
            {
                result = new InventoryItemBase[toReturn.Count];
                for (int i = 0; i < toReturn.Count; i++)
                {
                    result[i] = Object.Instantiate<InventoryItemBase>(toReturn[i].item);
                    result[i].currentStackSize = (uint)Random.Range((int)toReturn[i].minStackSize, (int)toReturn[i].maxStackSize);
                    result[i].gameObject.SetActive(false);
                }

                return result;
            }

            result = toReturn.Select(o => o.item).ToArray();
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