using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Used to represent a pouch that contains items. When the user uses the item it will add items to the player's inventory.
    /// </summary>
    public partial class ItemPouchInventoryItem : InventoryItemBase, IInventoryItemContainer
    {
        /// <summary>
        /// When the item is used, play this sound.
        /// </summary>
        public General.AudioClipInfo audioClipWhenUsed;

        [SerializeField]
        [Header("Items (overwritten when generated)")]
        private InventoryItemBase[] _items = new InventoryItemBase[0];
        public InventoryItemBase[] items
        {
            get { return _items; }
            set { _items = value; }
        }

        public string uniqueName
        {
            get { return gameObject.name; }
        }


        // Generate a set of items, or use the defined ones?
        [Header("Generate")]
        public bool generateItems = true;
        public InventoryItemGeneratorFilterGroup[] filterGroups = new InventoryItemGeneratorFilterGroup[0];

        public int minAmountTotal = 2;
        public int maxAmountTotal = 5;

        protected IItemGenerator generator { get; set; }
        protected bool canGenerateItems = true;


        public override LinkedList<ItemInfoRow[]> GetInfo()
        {
            var basic = base.GetInfo();
            //basic.AddAfter(basic.First, new InfoBoxUI.Row[]{
            //    new InfoBoxUI.Row("Restore health", restoreHealth.ToString(), Color.green, Color.green),
            //    new InfoBoxUI.Row("Restore mana", restoreMana.ToString(), Color.green, Color.green)
            //});


            return basic;
        }

        public override void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            base.NotifyItemUsed(amount, alsoNotifyCollection);

            PlayerManager.instance.currentPlayer.inventoryPlayer.stats.SetAll(stats);
        }

        protected virtual InventoryItemBase[] GenerateItems()
        {
            canGenerateItems = false;
            generator = new FilterGroupsItemGenerator(filterGroups);
            generator.SetItems(ItemManager.database.items);

            var items = generator.Generate(minAmountTotal, maxAmountTotal, true); // Create instances is required to get stack size to work (Can't change stacksize on prefab)
            foreach (var item in items)
            {
                item.transform.SetParent(transform);
                item.gameObject.SetActive(false);
            }

            return items;
        }



        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
            {
                return used;
            }

            if (generateItems && canGenerateItems)
            {
                items = GenerateItems();
            }

            // Keep in current collection. If it's unpacked in an inventory use all inventories instead.
            if (itemCollection != null && InventoryManager.IsInventoryCollection(itemCollection) == false)
            {
                // Try in current collection
                bool added = itemCollection.RemoveItemsThenAdd(items, new ItemAmountRow(this, 1));
                if (added == false)
                {
                    InventoryManager.langDatabase.collectionFull.Show(items.Select(o => o.name).Aggregate((a, b) => a + ", " + b), "", itemCollection.name);
                    return -2;
                }
            }
            else
            {
                bool added = InventoryManager.RemoveItemsThenAdd(items, new ItemAmountRow(this, 1));
                if (added == false)
                {
                    InventoryManager.langDatabase.collectionFull.Show(items.Select(o => o.name).Aggregate((a, b) => a + ", " + b), "", "Inventory");
                    return -2;
                }
            }

            // Do something with item
//            currentStackSize--; // 1 removed by RemoveItemsThenAdd method.
            NotifyItemUsed(1, true);
            canGenerateItems = true; // Allowed to generate new items for the next pouch.

            AudioManager.AudioPlayOneShot(audioClipWhenUsed);
            return 1; // 1 item used
        }

    }
}