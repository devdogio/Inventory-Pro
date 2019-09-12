using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Character")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CharacterUI : ItemCollectionBase, ICollectionPriority, IInventoryDragAccepter, ICharacterCollection
    {
        [Range(0, 100)]
        [SerializeField]
        private int _collectionPriority;
        public int collectionPriority
        {
            get { return _collectionPriority; }
            set { _collectionPriority = value; }
        }

        public bool isSharedCollection = false;

        [Header("UI References")]
        public StatsCollectionUI statsCollectionUI;

        public EquippableSlot[] equippableSlots { get; protected set; }


        /// <summary>
        /// The player that this CharacterUI belongs to. (can be null)
        /// </summary>
        private IEquippableCharacter _character;
        public IEquippableCharacter character
        {
            get { return _character; }
            set
            {
                _character = value;

                if (statsCollectionUI != null && _character != null)
                {
                    statsCollectionUI.statsCollection = _character.stats;
                    statsCollectionUI.RepaintAll();
                }
            }
        }

        private UIWindow _window;

        public virtual UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        public override uint initialCollectionSize
        {
            get
            {
                return (uint)equippableSlots.Length;
            }
        }

        protected override void Awake()
        {
            Assert.IsTrue(manuallyDefineCollection, "Character collections should manually be defined in the editor. If you want to generate an editor equipment system at run-time inherit from this character collection or write your own.");
            base.Awake();
            UpdateEquippableSlots();
            UseDefaultDataProviders();

            if (isSharedCollection)
                InventoryManager.AddEquipCollection(this, collectionPriority);

        }

        protected override void Start()
        {
            base.Start();
            UpdateStats();
        }

        protected virtual void UpdateStats()
        {
            
        }

        public virtual void UpdateEquippableSlots()
        {
            equippableSlots = new EquippableSlot[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                var c = items[i] as UnityEngine.Component;
                if (c != null)
                {
                    equippableSlots[i] = c.gameObject.GetComponent<EquippableSlot>();
                    Assert.IsNotNull(equippableSlots[i], "CharacterUI manually defined collection contains gameObject without InventoryEquippableField component.");

                    if (equippableSlots[i].equipmentTypes.Any(o => o == null))
                    {
                        DevdogLogger.LogError("CharacterUI's equipTypes contains null (empty) field.", equippableSlots[i].gameObject);
                    }
                }
            }

            if (equippableSlots.Any(o => o == null))
            {
                DevdogLogger.LogError("This characterUI has an empty reference in the equip slot fields (EquippableFields has empty row)", gameObject);
            }
        }

        protected virtual void UseDefaultDataProviders()
        {
            
        }

        public bool AcceptsDragItem(InventoryItemBase item)
        {
            var equippable = item as EquippableInventoryItem;
            if (equippable == null)
            {
                return false;
            }

            return equippable.CanEquip(this);
        }

        /// <summary>
        /// Called by the InventoryDragAccepter, when an item is dropped on the window / a specific location, this method is called to add a custom behavior.
        /// </summary>
        /// <param name="item"></param>
        public bool AcceptDragItem(InventoryItemBase item)
        {
            if (AcceptsDragItem(item) == false)
                return false;
            
            var equippable = (EquippableInventoryItem) item;
            var bestSlot = equippable.GetBestEquipSlot(this);
            return EquipItem(bestSlot, equippable);
        }


        /// <summary>
        /// Get all slots where this item can be equipped.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns indices where the item can be equipped. collection[index] ... </returns>
        public EquippableSlot[] GetEquippableSlots(EquippableInventoryItem item)
        {
            if (item.equipmentType == null)
            {
                Debug.LogWarning("The item " + item.name + " you're trying to equip has no equipment type set. Item cannot be equipped and will be ingored.", item.gameObject);
                return new EquippableSlot[0];
            }

            if (this.equippableSlots.Length == 0)
            {
                Debug.LogWarning("This characterUI has no equipSlotFields, need to define some??", gameObject);
                return new EquippableSlot[0];
            }

            var equipSlots = new List<EquippableSlot>(4);
            foreach (var field in this.equippableSlots)
            {
                // Disabled fields are ignored.
                if (field.gameObject.activeSelf == false || field.enabled == false)
                {
                    continue;
                }

                foreach (var type in field.equipmentTypes)
                {
                    if (item.equipmentType == type)
                    {
                        bool canAdd = true;
                        // Can the item be equipped considering the usage requirement properties?
                        foreach (var prop in item.usageRequirement)
                        {
                            canAdd = prop.CanUse(character);
                            if (canAdd == false)
                            {
                                break;
                            }
                        }

                        if (canAdd)
                        {
                            equipSlots.Add(field);
                        }
                    }
                }
            }

            return equipSlots.ToArray();
        }

        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.CanSetItem(slot, item);
            if (set == false)
                return false;

            if (item == null)
                return true;

            var equippable = item as EquippableInventoryItem;
            if (equippable == null)
            {
                return false; // Can't equip this item type, only Equippable and anything that inherits from Equippable.
            }

            if (equippable.VerifyCustomUseConditionals() == false)
            {
                return false;
            }

            if (equippable.CanEquip(this) == false)
            {
                return false;
            }

            var slots = GetEquippableSlots(equippable);
            if (slots.Length == 0)
            {
                return false;
            }

            return slots.Any(o => o.index == slot);
        }

        /// <summary>
        /// Some item's require multiple slots, for example a 2 handed item forces the left handed item to be empty.
        /// </summary>
        /// <returns>true if items were removed, false if items were not removed.</returns>
        protected virtual bool HandleLocks(EquippableSlot equipSlot, EquippableInventoryItem equippable)
        {
            var toBeRemoved = new List<uint>(8);

            // Loop through things we want to block
            foreach (var blockType in equippable.equipmentType.blockTypes)
            {
                // Check every slot against this block type
                foreach (var field in equippableSlots)
                {
                    var item = items[field.index].item;
                    if (item != null)
                    {
                        var eq = (EquippableInventoryItem)item;
                        if (eq.equipmentType == blockType && field.index != equipSlot.index)
                        {
                            toBeRemoved.Add(field.index);
                            bool canAdd = InventoryManager.CanAddItem(eq);
                            if (canAdd == false)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            foreach (uint i in toBeRemoved)
            {
                bool added = InventoryManager.AddItemAndRemove(items[i].item);
                Assert.IsTrue(added, "Item could not be saved, even after check, please report this bug + stacktrace.");
            }

            return true;
        }

        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return SwapSlots(slot1, handler2, slot2, repaint);
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            var equippable = item as EquippableInventoryItem;
            if (equippable != null)
            {
                bool handled = HandleLocks(equippableSlots[slot], equippable);
                if (handled == false)
                {
                    return false;
                }
            }

            return base.SetItem(slot, item);
        }


        public override void NotifyItemAdded(IEnumerable<InventoryItemBase> items, uint amount, bool cameFromCollection)
        {
            base.NotifyItemAdded(items, amount, cameFromCollection);

            foreach (var item in items)
            {
                Assert.IsTrue(item is EquippableInventoryItem, "Non equippable item was added to character collection. This is not allowed.");

                var i = (EquippableInventoryItem) item;
                var slot = equippableSlots.FirstOrDefault(o => o.slot.item == i);
                i.NotifyItemEquipped(slot, amount);

                if (i.equipVisually)
                {
                    _character.equipmentHandler.EquipItemVisually(i, slot);
                }
            }
        }

        public override void NotifyItemRemoved(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            base.NotifyItemRemoved(item, itemID, slot, amount);
            var i = (EquippableInventoryItem) item;
            i.NotifyItemUnEquipped(this, amount);

            _character.equipmentHandler.UnEquipItemVisually(i);

            // Item was removed, stats changed, can other items still remain equipped?

            var beforeCanUseFromCollection = canUseFromCollection;
            canUseFromCollection = true;

            character.stats.SetAll(item.stats, -1f, false);

            foreach (var wrapper in items)
            {
                if (wrapper.item != null)
                {
                    var equip = (EquippableInventoryItem)wrapper.item;

                    // Un-use the item, then check if the stats are valid, if not unequip
                    bool canEquip = equip.CanEquip(this);
                    if (canEquip == false && equip.isEquipped)
                    {
                        UnEquipItem(equip, true);
                    }
                }
            }

            // Restore
            character.stats.SetAll(item.stats, 1f, false);
            canUseFromCollection = beforeCanUseFromCollection;
        }

        public bool EquipItem(EquippableSlot equipSlot, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);          

            bool handled = HandleLocks(equipSlot, item);
            if (handled == false)
            {
                return false;
            }

            // The values before the collection / slot changed.
            uint fromIndex = item.index;
            var fromCollection = item.itemCollection;

            // There was already an item in this slot, un-equip that one first
            if (items[equipSlot.index].item != null)
            {
                bool unEquipped = UnEquipItem((EquippableInventoryItem) items[equipSlot.index].item, true);
                if (unEquipped == false)
                {
                    return false;
                }
            }

            // EquippedItem the item -> Will swap as merge is not possible
            bool canSet = CanSetItem(equipSlot.index, item);
            if (canSet)
            {
                bool set = SetItem(equipSlot.index, item);
                if (set && fromCollection != null)
                {
                    if (useReferences == false)
                    {
                        fromCollection.SetItem(fromIndex, null);
                        fromCollection.NotifyItemRemoved(item, item.ID, fromIndex, item.currentStackSize);
                        fromCollection[fromIndex].Repaint();
                    }
                }

                NotifyItemAdded(item, item.currentStackSize, true);
                items[equipSlot.index].Repaint();

                return true;
            }
            
            return true;
        }

        public bool UnEquipItem(EquippableInventoryItem item, bool addToInventory)
        {
            Assert.IsNotNull(item);
            Assert.IsTrue(item.isEquipped, "Trying to unEquip item, but item wasn't equipped in the first place!");
            Assert.IsTrue(item.itemCollection == this || useReferences, "Trying to un-equip an item that wasn't equipped in this collection!");

            if (item.CanUnEquip(addToInventory) == false)
            {
                return false;
            }

            if (useReferences == false)
            {
                if (addToInventory)
                {
                    bool added = InventoryManager.AddItemAndRemove(item);
                    Assert.IsTrue(added);
                    return added;
                }

                SetItem(item.index, null);
                NotifyItemRemoved(item, item.ID, item.index, item.currentStackSize);
            }
            else
            {
                foreach (var wrapper in items)
                {
                    if (wrapper.item == item)
                    {
                        SetItem(wrapper.index, null, true);
                        NotifyItemRemoved(item, item.ID, wrapper.index, item.currentStackSize);
                    }
                }
            }

            return true;
        }

        protected override bool SwapSlots(uint fromSlot, ItemCollectionBase toCollection, uint toSlot, bool repaint = true, bool fireEvents = true)
        {
            var swapped = base.SwapSlots(fromSlot, toCollection, toSlot, repaint, fireEvents);
            if (swapped)
            {
                if (this != toCollection)
                {
                    return true;
                }

                _character.equipmentHandler.SwapItems(this, fromSlot, toSlot);
            }

            return swapped;
        }
    }
}