using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public class InventoryItemUtility
    {
        public enum SetItemPropertiesAction
        {
            Use,
            UnUse
        }

        [Obsolete("Moved to IEquippableCharacter.Stats.SetAll")]
        public static void SetItemProperties(IEquippableCharacter character, StatDecorator[] stats, SetItemPropertiesAction action, bool fireEvents = true)
        {
            // Use the item's properties.
            if (character != null)
            {
                foreach (var property in stats)
                {
                    SetItemProperty(character, property, action, fireEvents);
                }
            }
        }

        [Obsolete("Moved to IEquippableCharacter.Stats.Set")]
        public static void SetItemProperty(IEquippableCharacter character, StatDecorator stat, SetItemPropertiesAction action, bool fireEvents = true)
        {
            Assert.IsNotNull(character, "Null player object passed, make sure the InventoryPlayerManager.instance.currentPlayer is set!");

            float multiplier = GetMultiplier(action);
            character.stats.Set(stat, multiplier);
        }

        private static float GetMultiplier(SetItemPropertiesAction action)
        {
            float multiplier = 1.0f;
            switch (action)
            {
                case SetItemPropertiesAction.Use:
                    multiplier = 1.0f;
                    break;
                case SetItemPropertiesAction.UnUse:
                    multiplier = -1f;
                    break;
                default:
                    Debug.LogWarning("Action " + action + " not found (Going with default use)");
                    break;
            }

            return multiplier;
        }

        public static ItemAmountRow[] ItemsToRows(IList<InventoryItemBase> itemsToAdd)
        {
            var list = new List<ItemAmountRow>(itemsToAdd.Count);
            for (int i = 0; i < itemsToAdd.Count; i++)
            {
                if (itemsToAdd[i] == null)
                {
                    continue;
                }
                
                uint stackCount = itemsToAdd[i].currentStackSize;
                while (stackCount > 0)
                {
                    var row = new ItemAmountRow(itemsToAdd[i], stackCount);
                    if (stackCount > itemsToAdd[i].maxStackSize)
                    {
                        stackCount -= itemsToAdd[i].maxStackSize;
                        row.SetAmount(itemsToAdd[i].maxStackSize);
                    }
                    else
                    {
                        stackCount = 0;
                    }

                    list.Add(row);
                }
            }

            return list.ToArray();
        }

        public static InventoryItemBase[] RowsToItems(ItemAmountRow[] items, bool abideMaxStackSize)
        {
            var l = new List<InventoryItemBase>(items.Length);
            foreach (var row in items)
            {
                if (abideMaxStackSize)
                {
                    uint counter = row.amount;
                    var stackCount = Mathf.CeilToInt((float)row.amount / row.item.maxStackSize);

                    for (int i = 0; i < stackCount; i++)
                    {
                        var pickAmount = (uint)Mathf.Min(counter, row.item.maxStackSize);

                        var item = UnityEngine.Object.Instantiate<InventoryItemBase>(row.item);
                        item.currentStackSize = pickAmount;
                        l.Add(item);

                        counter -= pickAmount;
                    }
                }
                else
                {
                    var item = UnityEngine.Object.Instantiate<InventoryItemBase>(row.item);
                    item.currentStackSize = row.amount;
                    l.Add(item);
                }
            }

            return l.ToArray();
        }

        public static ItemAmountRow[] EnforceMaxStackSize(IEnumerable<ItemAmountRow> itemRows)
        {
            var list = new List<ItemAmountRow>();
            foreach (var row in itemRows)
            {
                uint stackCount = row.amount;
                while (stackCount > 0)
                {
                    var row2 = new ItemAmountRow(row.item, stackCount);
                    if (stackCount > row.item.maxStackSize)
                    {
                        stackCount -= row.item.maxStackSize;
                        row2.SetAmount(row.item.maxStackSize);
                    }
                    else
                    {
                        stackCount = 0;
                    }

                    list.Add(row2);
                }
            }

            return list.ToArray();
        }

        public static InventoryItemBase[] EnforceMaxStackSize(IEnumerable<InventoryItemBase> items)
        {
            var l = new List<InventoryItemBase>();

            foreach (var item in items)
            {
                l.AddRange(EnforceMaxStackSize(item));
            }

            return l.ToArray();
        }

        public static InventoryItemBase[] EnforceMaxStackSize(InventoryItemBase item)
        {
            if (item == null)
            {
                return new InventoryItemBase[0];
            }

            if (item.currentStackSize <= item.maxStackSize)
            {
                return new [] { item };
            }

            uint counter = item.currentStackSize;
            var stackCount = Mathf.CeilToInt((float)item.currentStackSize / item.maxStackSize);
            var l = new InventoryItemBase[stackCount];

            for (int i = 0; i < stackCount; i++)
            {
                var pickAmount = (uint)Mathf.Min(counter, item.maxStackSize);

                var itemInst = UnityEngine.Object.Instantiate<InventoryItemBase>(item);
                itemInst.currentStackSize = pickAmount;
                l[i] = itemInst;

                counter -= pickAmount;
            }

            if (item.IsInstanceObject())
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }

            return l.ToArray();
        }
    }
}
