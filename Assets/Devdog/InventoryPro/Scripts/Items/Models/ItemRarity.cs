using System;
using UnityEngine;
using System.Collections;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class ItemRarity : ScriptableObject
    {
        public uint ID;

        [Required]
        public new string name;
        public Color color = Color.white;

        /// <summary>
        /// The item that is used when dropping something, leave null to use the object model itself.
        /// </summary>
        [Tooltip("The item that is used when dropping something, leave null to use the object model itself.")]
        public GameObject dropObject;
    }
}