using System;
using UnityEngine;
using System.Collections;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to define rarity of items.
    /// </summary>
    [System.Serializable]
    [Obsolete("Replaced by ItemRarity")]
    public partial class InventoryItemRarityDeprecated
    {
        public uint ID;
        public string name;
        public Color color = Color.white;

        /// <summary>
        /// The item that is used when dropping something, leave null to use the object model itself.
        /// </summary>
        [Tooltip("The item that is used when dropping something, leave null to use the object model itself.")]
        public GameObject dropObject;


        public InventoryItemRarityDeprecated()
        {

        }
        public InventoryItemRarityDeprecated(uint id, string name, Color color, GameObject dropObject)
        {
            this.ID = id;
            this.name = name;
            this.color = color;
            this.dropObject = dropObject;
        }
    }
}