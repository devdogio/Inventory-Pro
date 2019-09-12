using System;
using System.Collections.Generic;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public partial interface IItemGenerator
    {
        /// <summary>
        /// The items used to consider when generating the items
        /// </summary>
        InventoryItemGeneratorItem[] items { get; set; }

        /// <summary>
        /// The result of the last Generate() method.
        /// </summary>
        InventoryItemBase[] result { get; set; }

        /// <summary>
        /// Randomizing the seed to ensure a new set of items is generated.
        /// </summary>
        void RandomizeSeed();

        /// <summary>
        /// Generate n amount of items, results are stored in property result.
        /// </summary>
        /// <param name="amount">Amount of items to generate.</param>
        /// <returns>Array of items</returns>
        InventoryItemBase[] Generate(int amount, bool createInstances = false);

        /// <summary>
        /// Generate n amount of items, results are stored in property result.
        /// </summary>
        /// <param name="minAmount"></param>
        /// <param name="maxAmount"></param>
        /// <returns>Array of items</returns>
        InventoryItemBase[] Generate(int minAmount, int maxAmount, bool createInstances = false);


        /// <summary>
        /// Set a list of items at once
        /// </summary>
        /// <param name="items"></param>
        void SetItems(InventoryItemBase[] items, float chance = 1.0f);
    }
}