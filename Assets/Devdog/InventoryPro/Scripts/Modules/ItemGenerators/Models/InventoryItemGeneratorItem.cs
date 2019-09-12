using System;
using System.Collections.Generic;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class InventoryItemGeneratorItem
    {
        /// <summary>
        /// The item we want to submit
        /// </summary>
        [Required]
        [ForceCustomObjectPicker]
        public InventoryItemBase item;
    
        /// <summary>
        /// The chance this item will be chosen.
        /// Value from 0.0f to 1.0f.
        /// 0 = no chance
        /// 1.0f = 100% chance (filters still affect the object)
        /// </summary>
        [Range(0f, 1f)]
        public float chanceFactor = 1.0f;


        public uint minStackSize = 1;
        public uint maxStackSize = 2;


        public InventoryItemGeneratorItem(InventoryItemBase item, float chanceFactor)
        {
            this.item = item;
            this.chanceFactor = chanceFactor;
        }
    }
}