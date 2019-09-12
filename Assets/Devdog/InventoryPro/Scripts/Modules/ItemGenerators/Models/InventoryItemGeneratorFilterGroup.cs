using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [Serializable]
    public partial class InventoryItemGeneratorFilterGroup : InventoryItemFilters
    {
        /// <summary>
        /// The min amount of items to generate
        /// </summary>
        public int minAmount = 1;

        /// <summary>
        /// The max amount of items to generate
        /// </summary>
        public int maxAmount = 2;


        /// <summary>
        /// The min stack size of a single item generated (1 apple or 100 apples)
        /// </summary>
        public int minStackSize = 1;

        /// <summary>
        /// The max stack size of a single item generated (1 apple or 100 apples)
        /// </summary>
        public int maxStackSize = 1;
        

        [Range(0.0f, 1.0f)]
        [Tooltip("The chance factor of each item in this group.")]
        public float itemsChanceFactor = 1.0f;
    }
}