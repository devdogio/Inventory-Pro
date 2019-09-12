using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    //[RequireComponent(typeof(LootableObject))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Other/Inventory item percentage container generator")]
    public partial class InventoryItemPercentageContainerGenerator : MonoBehaviour, IInventoryItemContainerGenerator
    {
        public InventoryItemGeneratorItem[] items;

        public IInventoryItemContainer container { get; protected set; }

        public bool generateAtGameStart = true;

        public int minAmountTotal = 2;
        public int maxAmountTotal = 5;

        public IItemGenerator generator { get; protected set; }

        protected void Awake()
        {
            container = GetComponent<IInventoryItemContainer>();
            
            generator = new PercentageItemGenerator();
//            generator.SetItems(ItemManager.database.items); // Not needed, items are serialized as _items
            generator.items = items;

            if (generateAtGameStart)
            {
                container.items = generator.Generate(minAmountTotal, maxAmountTotal, true); // Create instances is required to get stack size to work (Can't change stacksize on prefab)
                foreach (var item in container.items)
                {
                    item.transform.SetParent(transform);
                }
            }
        }
    }
}
