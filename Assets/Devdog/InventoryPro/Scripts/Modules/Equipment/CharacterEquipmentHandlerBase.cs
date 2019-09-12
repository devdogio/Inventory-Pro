using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class CharacterEquipmentHandlerBase : ScriptableObject
    {
        public ICharacterCollection characterCollection { get; protected set; }

        // To avoid accidentely creating an instance through new()
        protected CharacterEquipmentHandlerBase()
        {
            
        }

        public void Init(ICharacterCollection col)
        {
            characterCollection = col;
        }

        public abstract void SwapItems(ICharacterCollection collection, uint fromSlot, uint toSlot);

        public abstract EquippableSlot FindEquippableSlotForItem(EquippableInventoryItem equippable);
        public abstract CharacterEquipmentTypeBinder FindEquipmentLocation(EquippableSlot slot);

        public abstract void EquipItemVisually(EquippableInventoryItem item);
        public abstract void EquipItemVisually(EquippableInventoryItem item, EquippableSlot specificSlot);
        public abstract void UnEquipItemVisually(EquippableInventoryItem item);
    }
}
