using UnityEngine;
using System;
using System.Collections.Generic;
using Devdog.General;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to define categories for items, categories can have a global cooldown, this can be usefull to cooldown all potions for example.
    /// </summary>
    [System.Serializable]
    public partial class ItemCategory : ScriptableObject
    {
        public uint ID;

        [Required]
        public new string name;
        public Sprite icon;

        /// <summary>
        /// If you don't want a cooldown leave it at 0.0
        /// </summary>
        [Range(0,999)]
        public float cooldownTime;


        [NonSerialized]
        private float _lastUsageTime;
        public float lastUsageTime
        {
            get { return _lastUsageTime; }
            set { _lastUsageTime = value; }
        }
    }
}