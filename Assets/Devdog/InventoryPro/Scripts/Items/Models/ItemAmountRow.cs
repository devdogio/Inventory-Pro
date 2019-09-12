using UnityEngine;
using System;
using System.Collections.Generic;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public struct ItemAmountRow
    {
        /// <summary>
        /// The item in this row
        /// </summary>
        [Required]
        public InventoryItemBase item;

        /// <summary>
        /// The amount of items required.
        /// </summary>
        public uint amount;


        public ItemAmountRow(InventoryItemBase item, uint amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public void SetItem(InventoryItemBase item)
        {
            this.item = item;
        }

        public void SetAmount(uint amount)
        {
            this.amount = amount;
        }
    }
}