using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Bank")]
    [RequireComponent(typeof(UIWindow))]
    public partial class BankUI : ItemCollectionBase
    {
        [Header("Behavior")]
        public UnityEngine.UI.Button sortButton;

        [SerializeField]
        private uint _initialCollectionSize = 80;
        /// <inheritdoc />
        public override uint initialCollectionSize { get { return _initialCollectionSize; } }

        /// <summary>
        /// When the item is used from this collection, should the item be moved to the inventory?
        /// </summary>
        [Header("Item usage")]
        public bool useMoveToInventory = true;

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


        [Header("Audio & Visuals")]
        public General.AudioClipInfo swapItemAudioClip;
        public General.AudioClipInfo sortAudioClip;
        public General.AudioClipInfo onAddItemAudioClip; // When an item is added to the inventory


        protected override void Awake()
        {
            base.Awake();

            InventoryManager.AddBankCollection(this);

            if(sortButton != null)
            {
                sortButton.onClick.AddListener(() =>
                {
                    SortCollection();
                    AudioManager.AudioPlayOneShot(sortAudioClip);
                });
            }
        }

        protected override void Start()
        {
            base.Start();

            // Listen for events
            OnAddedItem += (item, amount, cameFromCollection) =>
            {
                AudioManager.AudioPlayOneShot(onAddItemAudioClip);
            };
            OnSwappedItems += (ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot) =>
            {
                AudioManager.AudioPlayOneShot(swapItemAudioClip);
            };
        }

        // Items from the bank go straight to the inventory
        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            if (InventorySettingsManager.instance.settings.useContextMenu)
                return false;

            if(useMoveToInventory)
                InventoryManager.AddItemAndRemove(item);

            return useMoveToInventory;
        }

        public override IList<ItemUsability> GetExtraItemUsabilities(IList<ItemUsability> basic)
        {
            var l = base.GetExtraItemUsabilities(basic);

            l.Add(new ItemUsability("To inventory", (item) =>
            {
                var oldCollection = item.itemCollection;
                uint oldIndex = item.index;

                bool added = InventoryManager.AddItem(item);
                if(added)
                {
                    oldCollection.SetItem(oldIndex, null);
                    oldCollection[oldIndex].Repaint();

                    oldCollection.NotifyItemRemoved(item, item.ID, oldIndex, item.currentStackSize);
                }
            }));

            return l;
        }
    
        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.CanSetItem(slot, item);
            if (set == false)
                return false;

            if (item == null)
                return true;

            return item.isStorable;
        }
    }
}