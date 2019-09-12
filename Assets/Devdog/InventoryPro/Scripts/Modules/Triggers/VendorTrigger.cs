using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.Dialogs;
using Devdog.General.ThirdParty.UniLinq;

namespace Devdog.InventoryPro
{

    /// <summary>
    /// Represents a vendor that sells / buys something
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Trigger))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Triggers/Vendor Trigger")]
    public partial class VendorTrigger : MonoBehaviour, IInventoryItemContainer, ITriggerCallbacks, ITriggerWindowContainer
    {
        [Header("Vendor")]
        //public string vendorName;
        public bool canSellToVendor;


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        public new string name
        {
            get { return uniqueName; }
        }

//        [Header("Items")]
        [SerializeField]
        private InventoryItemBase[] _items = new InventoryItemBase[0];
        public InventoryItemBase[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }



        /// <summary>
        /// All prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        [Header("Buying / Selling")]
        public float buyPriceFactor = 1.0f;

        /// <summary>
        /// All sell prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float sellPriceFactor = 1.0f;

        [Range(1, 999)]
        public uint maxBuyItemCount = 999;
        public bool removeItemAfterPurchase = false;

        /// <summary>
        /// Can items be bought back after they're sold?
        /// </summary>
        [Header("Buy back")]
        public bool enableBuyBack = true;

        /// <summary>
        /// When true the items will be stored in the buy back collection, when false the items will be added to the regular collection of the vendor.
        /// </summary>
        [Tooltip("When true the items will be stored in the buy back collection, when false the items will be added to the regular collection of the vendor.")]
        public bool addItemsSoldToVendorToBuyBack = true;

        /// <summary>
        /// How expensive is the item to buy back. item.sellPrice * buyBackCostFactor = the final price to buy back an item.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float buyBackPriceFactor = 1.0f;

        /// <summary>
        /// Max number of items in buy back window.
        /// </summary>
        public uint maxBuyBackItemSlotsCount = 10;

        /// <summary>
        /// Is buyback shared between all vendors with the same category?
        /// </summary>
        public bool buyBackIsShared = false;

        /// <summary>
        /// The category this vendor belongs to, used for sharing the buyback.
        /// Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.
        /// </summary>
        [Tooltip("Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.")]
        public string vendorCategory = "Default";

        /// <summary>
        /// Generator used to generate a random set of items for this vendor
        /// </summary>
        public IItemGenerator itemGenerator { get; set; }

        protected VendorUI vendorUI { get { return InventoryManager.instance.vendor; } }
        protected Animator animator;

        public UIWindow window {
            get { return vendorUI.window; }
        }


        public VendorBuyBackDataStructure<InventoryItemBase> buyBackDataStructure { get; protected set; }

        [NonSerialized]
        protected Transform buyBackParent;

        protected virtual void Start()
        {
            if (vendorUI == null)
            {
                Debug.LogWarning("No vendor UI found, yet there's a vendor in the scene.", transform);
                return;
            }

            for (int i = 0; i < items.Length; i++)
            {
                var t = Instantiate<InventoryItemBase>(items[i]);
                t.currentStackSize = items[i].currentStackSize;
                t.maxStackSize = 999;
                t.transform.SetParent(transform);
                t.gameObject.SetActive(false);
                items[i] = t;
            }

            animator = GetComponent<Animator>();
            buyBackDataStructure = new VendorBuyBackDataStructure<InventoryItemBase>((int)maxBuyBackItemSlotsCount, buyBackIsShared, vendorCategory, (int)maxBuyBackItemSlotsCount);

            buyBackParent = new GameObject("Vendor_BuyBackContainer").transform;
            buyBackParent.SetParent(InventoryManager.instance.collectionObjectsParent);
        }

        public virtual bool OnTriggerUsed(Player player)
        {
            // Set items
            vendorUI.Clear();
            if (items.Length > vendorUI.items.Length)
            {
                vendorUI.Resize((uint)items.Length);
            }

            for (uint i = 0; i < items.Length; i++)
            {
                vendorUI.AddItem(items[i], null, true, false);
            }

            vendorUI.currentVendor = this;
            vendorUI.window.Show();

            return false;
        }

        public virtual bool OnTriggerUnUsed(Player player)
        {
            var before = vendorUI.currentVendor;
            vendorUI.currentVendor = null;

            // If a differnet window was opened, don't hide it.
            if (before == this)
            {
                vendorUI.window.Hide();
            }

            return false;
        }

        /// <summary>
        /// Sell an item to this vendor.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual void SellItemToVendor(InventoryItemBase item)
        {
            uint max = InventoryManager.GetItemCount(item.ID, false);

            if (CanSellItemToVendor(item, max) == false)
            {
                InventoryManager.langDatabase.vendorCannotSellItem.Show(item.name, item.description, max);
                return;
            }

            if (InventoryManager.instance.buySellDialog != null)
            {
                InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform,
                    "Sell " + name, "Are you sure you want to sell " + name, 1, (int) max, item,
                    ItemBuySellDialogAction.Selling, this, (amount) =>
                    {
                        // Sell items
                        SellItemToVendorNow(item, (uint) amount);

                    }, (amount) =>
                    {
                        // Canceled

                    });
            }
            else
            {
                SellItemToVendorNow(item, item.currentStackSize);
            }
        }

        /// <summary>
        /// Sell item now to this vendor. The vendor doesn't sell the object, the user sells to this vendor.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool SellItemToVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanSellItemToVendor(item, amount) == false)
                return false;

            if (enableBuyBack)
            {
                var copy = Instantiate<InventoryItemBase>(item);
                copy.currentStackSize = amount;
                copy.maxStackSize = 999; // Stacking
                copy.transform.SetParent(buyBackParent.transform);

                if (addItemsSoldToVendorToBuyBack)
                {
                    buyBackDataStructure.Add(copy);
                }
                else
                {
                    vendorUI.AddItem(copy); // Add to regular collection.
                }
            }

            InventoryManager.AddCurrency(item.sellPrice.currency.ID, GetSellPrice(item, amount));
            InventoryManager.RemoveItem(item.ID, amount, false);

            vendorUI.NotifyItemSoldToVendor(item, amount);
            return true;
        }

        public virtual bool CanSellItemToVendor(InventoryItemBase item, uint amount)
        {
            if (canSellToVendor == false)
                return false;

            if (item.isSellable == false)
                return false;

            if (addItemsSoldToVendorToBuyBack == false)
            {
                if (vendorUI.CanAddItem(item) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool CanBuyItemBackFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyBackPrice(item, amount);

            if (buyBackDataStructure.ItemCount(item.ID) < amount)
                return false; // Something wen't wrong, we don't have that many items for buy-back.

            if (InventoryManager.CanRemoveCurrency(item.sellPrice.currency.ID, totalCost, true) == false)
            {
                string totalCostString = item.sellPrice.ToString(amount * buyBackPriceFactor);
                var c = InventoryManager.GetCurrencyCount(item.sellPrice.currency.ID, false);

                InventoryManager.langDatabase.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c);
                return false; // Not enough gold for this many
            }
            
            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.langDatabase.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual bool CanBuyItemFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyPrice(item, amount);

            if (totalCost > 0f)
            {
                if (InventoryManager.CanRemoveCurrency(item.buyPrice.currency.ID, totalCost, true) == false)
                {
                    string totalCostString = item.buyPrice.ToString(amount * buyPriceFactor);
                    var c = InventoryManager.GetCurrencyCount(item.buyPrice.currency.ID, false);

                    InventoryManager.langDatabase.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c);
                    return false; // Not enough gold for this many
                }
            }

            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.langDatabase.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual void BuyItemFromVendor(InventoryItemBase item, bool isBuyBack)
        {
            ItemBuySellDialogAction action = ItemBuySellDialogAction.Buying;
            uint maxAmount = removeItemAfterPurchase ? vendorUI.GetItemCount(item.ID) : maxBuyItemCount;
            if (isBuyBack)
            {
                action = ItemBuySellDialogAction.BuyingBack;
                maxAmount = item.currentStackSize;
            }

            if (InventoryManager.instance.buySellDialog != null)
            {
                InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform,
                    "Buy item " + item.name, "How many items do you want to buy?", 1, (int) maxAmount, item, action,
                    this, (amount) =>
                    {
                        // Clicked yes!
                        if (isBuyBack)
                            BuyItemBackFromVendorNow(item, (uint) amount);
                        else
                            BuyItemFromVendorNow(item, (uint) amount);

                    }, (amount) =>
                    {
                        // Clicked cancel...

                    });
            }
            else
            {
                if (isBuyBack)
                    BuyItemBackFromVendorNow(item, 1);
                else
                    BuyItemFromVendorNow(item, 1);
            }
        }


        public virtual bool BuyItemBackFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemBackFromVendor(item, amount) == false)
                return false;

            buyBackDataStructure.Remove(item, amount);


            var c1 = Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;
            c1.maxStackSize = ItemManager.database.items[c1.ID].maxStackSize; // Reset stack size from database.

            InventoryManager.RemoveCurrency(item.sellPrice.currency.ID, GetBuyBackPrice(item, amount));
            c1.PickupItem();

            //            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.

            vendorUI.NotifyItemBoughtBackFromVendor(item, amount);
            return true;
        }


        /// <summary>
        /// Buy an item from this vendor, this does not display a dialog, but moves the item directly to the inventory.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual bool BuyItemFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemFromVendor(item, amount) == false)
                return false;


            var c1 = Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;
            c1.maxStackSize = ItemManager.database.items[c1.ID].maxStackSize; // Reset stack size from database.

            var buyPrice = GetBuyPrice(item, amount);
            if (buyPrice > 0f)
            {
                InventoryManager.RemoveCurrency(item.buyPrice.currency.ID, buyPrice);
            }
            c1.PickupItem();
            //            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.

            if (removeItemAfterPurchase)
            {
                vendorUI.RemoveItem(item.ID, amount);
            }

            vendorUI.NotifyItemBoughtFromVendor(item, amount);
            return true;
        }


        /// <summary>
        /// Can this item * amount be added to the inventory, is there room?
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>True if items can be placed, false is not.</returns>
        protected virtual bool CanAddItemsToInventory(InventoryItemBase item, uint amount)
        {
            uint originalStackSize = item.currentStackSize;

            item.currentStackSize = amount;
            bool can = InventoryManager.CanAddItem(item);
            item.currentStackSize = originalStackSize; // Reset

            return can;
        }

        public virtual float GetBuyBackPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * buyBackPriceFactor;
        }
        public virtual float GetBuyPrice(InventoryItemBase item, uint amount)
        {
            return item.buyPrice.amount * amount * buyPriceFactor;
        }
        public virtual float GetSellPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * sellPriceFactor;
        }
    }
}