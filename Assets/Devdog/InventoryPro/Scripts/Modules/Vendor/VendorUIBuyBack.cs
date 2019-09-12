using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Vendor buy back")]
    public partial class VendorUIBuyBack : ItemCollectionBase
    {
        private UIWindowPage _window;
        public UIWindowPage window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindowPage>();

                return _window;
            }
            protected set { _window = value; }
        }

        [Required]
        public VendorUI vendorUI;
    

        [SerializeField]
        protected uint _initialCollectionSize = 10;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        private ItemCollectionBase[] _subscribedToCollection = new ItemCollectionBase[0];

        protected override void Awake()
        {
            base.Awake();

            PlayerManager.instance.OnPlayerChanged += InstanceOnPlayerChanged;
            if (PlayerManager.instance.currentPlayer != null)
            {
                InstanceOnPlayerChanged(null, PlayerManager.instance.currentPlayer);
            }
        }

        protected override void Start()
        {
            base.Start();

            window.OnShow += UpdateItems;
            vendorUI.OnSoldItemToVendor += (InventoryItemBase item, uint amount, VendorTrigger vendor) =>
            {
                UpdateItems();
            };

            vendorUI.OnBoughtItemBackFromVendor += (InventoryItemBase item, uint amount, VendorTrigger vendor) =>
            {
                UpdateItems();
            };
        }

        private void InstanceOnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            if (oldPlayer != null)
            {
                foreach (var col in _subscribedToCollection)
                {
                    col.OnCurrencyChanged -= OnPlayerCurrencyChanged;
                }
            }

            _subscribedToCollection = InventoryManager.GetLootToCollections();
            foreach (var col in _subscribedToCollection)
            {
                col.OnCurrencyChanged += OnPlayerCurrencyChanged;
            }
        }

        private void OnPlayerCurrencyChanged(float amountBefore, CurrencyDecorator decorator)
        {
            Repaint();
        }

        protected virtual void Repaint()
        {
            foreach (var item in items)
            {
                item.Repaint();
            }
        }

        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            vendorUI.currentVendor.BuyItemFromVendor(item, true);
            return true;
        }

        protected virtual void UpdateItems()
        {
            if (vendorUI.currentVendor == null)
                return;

            if (vendorUI.currentVendor.enableBuyBack)
            {
                SetItems(vendorUI.currentVendor.buyBackDataStructure.ToArray(), true);
            }
        }

        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            var before = canPutItemsInCollection;
            canPutItemsInCollection = true;
            bool can = base.CanSetItem(slot, item);
            canPutItemsInCollection = before;
            return can;
        }

        public override void SetItems(InventoryItemBase[] toSet, bool setParent, bool repaint = true)
        {
            if (vendorUI.currentVendor == null || vendorUI.currentVendor.enableBuyBack == false)
                return;

            base.SetItems(toSet, setParent, false);
            Repaint();
        }

        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return SwapSlots(slot1, handler2, slot2, repaint);    
        }
    }
}