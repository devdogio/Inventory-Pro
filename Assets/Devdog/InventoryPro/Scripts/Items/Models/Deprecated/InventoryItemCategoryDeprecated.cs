using UnityEngine;
using System;
using System.Collections.Generic;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to define categories for items, categories can have a global cooldown, this can be usefull to cooldown all potions for example.
    /// </summary>
    [System.Serializable]
    [Obsolete("Replaced by ItemCategory")]
    public partial class InventoryItemCategoryDeprecated
    {
        public class OverrideCooldownRow
        {
            public float lastUsageTime;
            public uint itemID;
        }

        public uint ID;
        public string name;
        public Sprite icon;

        /// <summary>
        /// If you don't want a cooldown leave it at 0.0
        /// </summary>
        [Range(0,999)]
        public float cooldownTime;
    
        [HideInInspector]
        [NonSerialized]
        public float lastUsageTime;


        [HideInInspector]
        [NonSerialized]
        public List<OverrideCooldownRow> overrideCooldownList = new List<OverrideCooldownRow>();

    }
}