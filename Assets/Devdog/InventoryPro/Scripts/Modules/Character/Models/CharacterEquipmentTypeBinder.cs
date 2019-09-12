using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using Devdog.InventoryPro;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class CharacterEquipmentTypeBinder
    {
        /// <summary>
        /// The equip field this is attached to
        /// </summary>
        public EquippableSlot equippableSlot;

        /// <summary>
        /// Use dynamic search for this field?
        /// </summary>
        public bool findDynamic = false;

        /// <summary>
        /// The path / gameObject name of the object to look for.
        /// </summary>
        public string equipTransformPath;

        /// <summary>
        /// The transform the item should be equipped to
        /// </summary>
        public Transform equipTransform;

        /// <summary>
        /// The root node for the equipped item. This is used for skinned meshes.
        /// </summary>
        public Transform rootBone;
        
        /// <summary>
        /// The item that is currently in this binder
        /// </summary>
        public GameObject currentItem { get; set; }

        public CharacterEquipmentTypeBinder()
        {
            
        }

        public CharacterEquipmentTypeBinder(EquippableSlot equippableSlot, Transform equipTransform)
        {
            this.equippableSlot = equippableSlot;
            this.equipTransform = equipTransform;
        }
    }
}
