using System;
using UnityEngine;
using System.Collections;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class EquipmentType : ScriptableObject, IEquatable<EquipmentType>
    {
        [Required]
        public new string name;

        /// <summary>
        /// Disallow other types to be set while this one is active.
        /// For example when equipping a greatsword, you might want to un-equip the shield.
        /// </summary>
        public EquipmentType[] blockTypes = new EquipmentType[0];

        /// <summary>
        /// The equipment handler that handles the equipment of items in this equipment type.
        /// </summary>
        [Required]
        public ItemEquipmentHandlerBase equipmentHandler;

        public bool Equals(EquipmentType other)
        {
            return name == other.name;
        }
    }
}