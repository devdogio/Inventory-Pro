using System;
using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Reflection;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to represent items that can be equipped by the user, this includes armor, weapons, etc.
    /// </summary>
    public partial class EquippableInventoryItem : InventoryItemBase
    {
        [SerializeField]
        [Required]
        private EquipmentType _equipmentType;
        public EquipmentType equipmentType
        {
            get { return _equipmentType; }
            protected set { _equipmentType = value; }
        }

        public AudioClipInfo playOnEquip;

        /// <summary>
        /// Consider this item for visual equipment?
        /// </summary>
        public bool equipVisually = true;

        /// <summary>
        /// The position / offset used when equipping the item visually
        /// </summary>
        [FormerlySerializedAs("equipPosition")]
        [SerializeField]
        private Vector3 _equipmentPosition;
        public Vector3 equipmentPosition
        {
            get { return _equipmentPosition; }
        }

        /// <summary>
        /// The rotation of the item when equipping the item visually
        /// </summary>
        [FormerlySerializedAs("equipRotation")]
        [SerializeField]
        private Quaternion _equipmentRotation;
        public Quaternion equipmentRotation
        {
            get { return _equipmentRotation; }
        }

        /// <summary>
        /// Is the item currently equipped or not?
        /// </summary>
        public bool isEquipped
        {
            get { return equippedToCollection != null; }
        }

        public ICharacterCollection equippedToCollection { get; set; }
        public CharacterEquipmentTypeBinder visuallyEquippedToBinder { get; set; }

        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        public virtual void NotifyItemEquipped(EquippableSlot equipSlot, uint amountEquipped)
        {
            this.equippedToCollection = equipSlot.characterCollection;
            Assert.IsNotNull(equippedToCollection, "ICharacterCollection's player reference not set! Forgot to assign to player?");

            foreach (var stat in stats)
            {
                stat.actionEffect = StatDecorator.ActionEffect.Add; // Force add effect for equippable stats.
            }

            equipSlot.characterCollection.character.stats.SetAll(stats, 1f);
            AudioManager.AudioPlayOneShot(playOnEquip);
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        public virtual void NotifyItemEquippedVisually(CharacterEquipmentTypeBinder binder)
        {

        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        public virtual void NotifyItemUnEquippedVisually(CharacterEquipmentTypeBinder binder)
        {

        }

        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public virtual void NotifyItemUnEquipped(ICharacterCollection equipTo, uint amountUnEquipped)
        {
            this.equippedToCollection = null;
            equipTo.character.stats.SetAll(stats, -1f);
        }

        public override GameObject Drop()
        {
            // Currently in a equip to collection?
            if (isEquipped)
            {
                bool unEquipped = equippedToCollection.UnEquipItem(this, true);
                if (unEquipped == false)
                {
                    return null;
                }
            }

            return base.Drop();
        }


        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;


            var prevCollection = itemCollection;
            var prevIndex = index;

            if (isEquipped)
            {
                bool unequipped = equippedToCollection.UnEquipItem(this, true);
                if (unequipped)
                {
                    NotifyItemUsed(currentStackSize, false);
                    if (prevCollection != null)
                    {
                        prevCollection.NotifyItemUsed(this, ID, prevIndex, currentStackSize);
                    }

                    return 1; // Used from inside the character, move back to inventory.
                }

                return -1; // Couldn't un-unequip
            }

            var equipTo = GetBestEquipToCollection();
            if (equipTo == null)
            {
//                Debug.LogWarning("No equip collection found for item " + name + ", forgot to assign the character collection to the player?", transform);
                return -2; // No equip to collections found
            }

            var equipSlot = GetBestEquipSlot(equipTo);
            if (equipSlot == null)
            {
                Debug.LogWarning("No suitable equip slot found for item " + name, transform);    
                return -2; // No slot found
            }

            // This actually equips the item.
            var equipped = equipTo.EquipItem(equipSlot, this);
            if (equipped)
            {
                NotifyItemUsed(currentStackSize, false);
                if (prevCollection != null)
                {
                    prevCollection.NotifyItemUsed(this, ID, prevIndex, currentStackSize);
                }

                return 1;
            }

            return -1;
        }

        protected virtual CharacterUI GetBestEquipToCollection()
        {
            CharacterUI last = null;
            foreach (var col in InventoryManager.GetEquipToCollections())
            {
                var best = GetBestEquipSlot(col);
                if (best != null && (best.slot.item == null || last == null))
                {
                    last = col;
                }
            }

            return last;
        }

        /// <summary>
        /// Verifies if the item can be equipped or not.
        /// This is validated after CanSetItem, so the item can be rejected before it gets here, if it doesn't match onlyAllowTypes.
        /// </summary>
        /// <returns>True if the item can be equipped, false if not.</returns>
        public virtual bool CanEquip(ICharacterCollection equipTo)
        {
            if(CanUse() == false)
            {
                return false;
            }
            
            return GetBestEquipSlot(equipTo) != null;
        }

        public virtual bool CanUnEquip(bool addToInventory)
        {
            if (addToInventory)
            {
                return InventoryManager.CanAddItem(this);
            }

            return true;
        }

        public bool Equip(EquippableSlot equipSlot)
        {
            return equipSlot.characterCollection.EquipItem(equipSlot, this);
        }

        public bool UnEquip(bool addItemToInventory)
        {
            return equippedToCollection.UnEquipItem(this, addItemToInventory);
        }

        public virtual EquippableSlot GetBestEquipSlot(ICharacterCollection equipCollection)
        {
            Assert.IsNotNull(equipCollection);

            var equipSlots = equipCollection.GetEquippableSlots(this);
            if (equipSlots.Length == 0)
            {
                DevdogLogger.LogVerbose("No equipment location found for item: #" + ID + " " + name);
                return null;
            }

            EquippableSlot equipSlot = equipSlots[0];
            foreach (var e in equipSlots)
            {
                if (equipCollection.equippableSlots[e.index].slot.item == null)
                {
                    equipSlot = e; // Prefer an empty slot over swapping a filled one.
                    break;
                }
            }

            return equipSlot;
        }
    }
}