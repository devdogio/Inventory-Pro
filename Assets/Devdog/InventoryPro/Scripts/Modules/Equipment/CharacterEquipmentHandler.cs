using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [CreateAssetMenu(menuName = InventoryPro.CreateAssetMenuPath + "Character equipment handler")]
    public partial class CharacterEquipmentHandler : CharacterEquipmentHandlerBase
    {
        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="item"></param>
        public delegate void EquippedVisually(CharacterEquipmentTypeBinder binder, EquippableInventoryItem item);
        public event EquippedVisually OnEquippedVisually;
        public event EquippedVisually OnUnEquippedVisually;

        
        protected CharacterEquipmentHandler()
        {
            this.characterCollection = characterCollection;
        }


        public override void SwapItems(ICharacterCollection collection, uint fromSlot, uint toSlot)
        {
            // Items are already swapped here...
            var fromItem = (EquippableInventoryItem)collection.equippableSlots[fromSlot].slot.item;
            var toItem = (EquippableInventoryItem)collection.equippableSlots[toSlot].slot.item;

            UnEquipItemVisually(toItem);

            // Remove from old position
            if (fromItem != null)
            {
                UnEquipItemVisually(fromItem);
            }

            EquipItemVisually(toItem, characterCollection.equippableSlots[toSlot]);

            if (fromItem != null)
            {
                EquipItemVisually(fromItem, characterCollection.equippableSlots[fromSlot]);
            }
        }

        private void NotifyItemEquippedVisually(CharacterEquipmentTypeBinder binder, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);
            Assert.IsNotNull(binder);

            if (OnEquippedVisually != null)
                OnEquippedVisually(binder, item);

           item.NotifyItemEquippedVisually(binder);
        }

        private void NotifyItemUnEquippedVisually(CharacterEquipmentTypeBinder binder, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);
            Assert.IsNotNull(binder);

            if (OnUnEquippedVisually != null)
                OnUnEquippedVisually(binder, item);

           item.NotifyItemUnEquippedVisually(binder);
        }

        public override CharacterEquipmentTypeBinder FindEquipmentLocation(EquippableSlot slot)
        {
            foreach (var binder in characterCollection.character.equipmentBinders)
            {
                if (binder.equippableSlot == slot)
                {
                    return binder;
                }
            }

            return null;
        }

        public override void EquipItemVisually(EquippableInventoryItem item)
        {
            var slot = FindEquippableSlotForItem(item);
            EquipItemVisually(item, slot);
        }

        public override void EquipItemVisually(EquippableInventoryItem item, EquippableSlot slot)
        {
            if (item.equipVisually == false)
            {
                return;
            }

            var binder = FindEquipmentLocation(slot);
            if (binder != null)
            {
                var t = GetEquippableTypeFromItem(binder, item);
                item.visuallyEquippedToBinder = binder;
                var copy = t.equipmentHandler.Equip(item, binder, true);

                binder.currentItem = copy.gameObject;
                NotifyItemEquippedVisually(binder, copy);
            }
        }

        public override EquippableSlot FindEquippableSlotForItem(EquippableInventoryItem equippable)
        {
            if (characterCollection.useReferences)
            {
                foreach (var slot in characterCollection.equippableSlots)
                {
                    if (slot.slot.item == equippable)
                    {
                        return characterCollection.equippableSlots[slot.index];
                    }
                }
            }

            return characterCollection.equippableSlots.FirstOrDefault(o => o.equipmentTypes.Contains(equippable.equipmentType));
        }

        private EquipmentType GetEquippableTypeFromItem(CharacterEquipmentTypeBinder binder, EquippableInventoryItem item)
        {
            return binder.equippableSlot.equipmentTypes.FirstOrDefault(o => o == item.equipmentType);
        }

        public override void UnEquipItemVisually(EquippableInventoryItem item)
        {
            if (item.visuallyEquippedToBinder != null)
            {
                UnEquipItemVisually(item, item.visuallyEquippedToBinder);
            }
        }

        protected virtual void UnEquipItemVisually(EquippableInventoryItem item, CharacterEquipmentTypeBinder binder)
        {
            if (binder != null && binder.currentItem != null)
            {
                var t = GetEquippableTypeFromItem(binder, item);
                t.equipmentHandler.UnEquip(binder, true);
                NotifyItemUnEquippedVisually(binder, item);
            }
        }
    }
}
