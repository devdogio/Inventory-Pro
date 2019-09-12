using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Other/Inventory item container generator")]
    public partial class InventoryItemContainerGenerator : MonoBehaviour, IInventoryItemContainerGenerator
    {
        public InventoryItemGeneratorFilterGroup[] filterGroups = new InventoryItemGeneratorFilterGroup[0];

        public IInventoryItemContainer container { get; protected set; }

        public bool generateAtGameStart = true;

        public int minAmountTotal = 2;
        public int maxAmountTotal = 5;

        public IItemGenerator generator { get; protected set; }

        protected void Awake()
        {
            container = GetComponent<IInventoryItemContainer>();
            
            generator = new FilterGroupsItemGenerator(filterGroups);
            generator.SetItems(ItemManager.database.items);

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
