using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventoryPro;
using UnityEngine.Assertions;
using Devdog.General.UI;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Crafting layout")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowLayoutUI : CraftingWindowBase
    {
        //        [Header("Audio & Visuals")]
        //        public Color enoughGoldColor;
        //        public Color notEnoughGoldColor;

        public Dictionary<uint, uint> currentBlueprintItemsDict { get; protected set; }
        public CraftingWindowLayoutUI()
        {
            currentBlueprintItemsDict = new Dictionary<uint, uint>(9);
        }
                
        protected override void OnCraftButtonClicked()
        {
            CraftCurrentlySelectedBlueprint(1);
        }

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(mainItemsCollection, "The main items collection has to be set on layout crafting.");

            mainItemsCollection.OnSetItem += MainItemsCollectionOnOnSetItem;
        }

        private void MainItemsCollectionOnOnSetItem(uint slot, InventoryItemBase item)
        {
            Assert.IsNotNull(currentCategory, "Set item in main collection, yet category is not set??");
            RepaintLayout(currentCategory);
        }

        protected override void OnWindowShown()
        {
            base.OnWindowShown();

            SetBlueprintResults(null);

            ValidateReferences(currentCategory);
            RepaintLayout(currentCategory);
        }

        public void RepaintLayout(CraftingCategory category)
        {
            if (window.isVisible == false)
                return;

            var blueprint = GetBlueprintFromCurrentLayout(mainItemsCollection, category);
            SetCraftingBlueprint(blueprint);
            RepaintMainItemsCollection();
        }

        public virtual void ValidateReferences(CraftingCategory category)
        {
            if (mainItemsCollection.useReferences == false)
                return;

            foreach (var wrapper in mainItemsCollection.items)
            {
                if (wrapper.item != null)
                {
                    uint count = InventoryManager.GetItemCount(wrapper.item.ID, category.alsoScanBankForRequiredItems);
                    if (count == 0)
                    {
                        wrapper.item = null;
                    }
                }
            }
        }

        private void RepaintMainItemsCollection()
        {
            foreach (var wrapper in mainItemsCollection.items)
            {
                wrapper.Repaint();
            }
        }

        public override void SetCraftingCategory(CraftingCategory category)
        {
            base.SetCraftingCategory(category);

            if (category.cols * category.rows > removeItemsFromCollection.items.Length)
            {
                Debug.Log("Increasing crafting layout UI slot count");
                removeItemsFromCollection.AddSlots((uint)(category.cols * category.rows - removeItemsFromCollection.items.Length));
            }
            else if (category.cols * category.rows < removeItemsFromCollection.items.Length)
            {
                Debug.Log("Decreasing crafting layout UI slot count");
                removeItemsFromCollection.RemoveSlots((uint)(removeItemsFromCollection.items.Length - category.cols * category.rows));
            }

            RepaintLayout(category);
        }


        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        public override bool CanCraftBlueprint(InventoryPlayer player, CraftingProgressContainer.CraftInfo craftInfo)
        {
            bool can = base.CanCraftBlueprint(player, craftInfo);
            if (can == false)
            {
                return false;
            }

            // No blueprint found
            if (GetBlueprintFromCurrentLayout(craftInfo.removeItemsFromCollection ?? mainItemsCollection, craftInfo.category) == null)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Tries to find a blueprint based on the current layout / items inside the UI item wrappers (items).
        /// </summary>
        /// <returns>Returns blueprint if found one, null if not.</returns>
        public virtual CraftingBlueprint GetBlueprintFromCurrentLayout(ItemCollectionBase collection, CraftingCategory category)
        {
            if (collection.items.Length != category.cols * category.rows)
            {
                Debug.LogWarning("Updating blueprint but blueprint layout cols/rows don't match the collection");
            }

            int totalItemCountInLayout = 0; // Nr of items inside the UI wrappers.
            foreach (var item in collection.items)
            {
                if (item.item != null)
                    totalItemCountInLayout++;
            }

            foreach (var b in GetBlueprints(category))
            {
                foreach (var a in b.blueprintLayouts)
                {
                    if (a.enabled)
                    {
                        var hasItems = new Dictionary<uint, uint>(); // ItemID, amount
                        //var requiredItems = new Dictionary<uint, uint>(); // ItemID, amount
                        currentBlueprintItemsDict.Clear();

                        int counter = 0; // Item index counter
                        int shouldHaveCount = 0; // Amount we should have..
                        int hasCount = 0; // How many slots in our layout
                        bool matchFailed = false;
                        foreach (var r in a.rows)
                        {
                            if (matchFailed)
                                break;

                            foreach (var c in r.columns)
                            {
                                if (c.item != null && c.amount > 0)
                                {
                                    if (currentBlueprintItemsDict.ContainsKey(c.item.ID) == false)
                                        currentBlueprintItemsDict.Add(c.item.ID, 0);

                                    currentBlueprintItemsDict[c.item.ID] += (uint)c.amount;
                                    shouldHaveCount++;

                                    if (collection.items[counter].item != null)
                                    {
                                        if (collection.items[counter].item.ID != c.item.ID)
                                        {
                                            matchFailed = true;
                                            break; // Item in the wrong place...
                                        }

                                        if (hasItems.ContainsKey(c.item.ID) == false)
                                        {
                                            uint itemCount = 0;
                                            if (collection.useReferences)
                                            {
                                                itemCount = InventoryManager.GetItemCount(c.item.ID, category.alsoScanBankForRequiredItems);
                                            }
                                            else
                                            {
                                                //                                                itemCount = items[counter].item.currentStackSize;
                                                itemCount = collection.GetItemCount(c.item.ID);
                                            }

                                            hasItems.Add(c.item.ID, itemCount);
                                        }

                                        hasCount++;
                                    }
                                    else if (collection.items[counter].item == null && c != null)
                                    {
                                        matchFailed = true;
                                        break;
                                    }
                                }

                                counter++;
                            }
                        }

                        if (matchFailed)
                            continue;

                        // Filled slots test
                        if (totalItemCountInLayout != hasCount || shouldHaveCount != hasCount)
                            continue;

                        // Check count
                        foreach (var item in currentBlueprintItemsDict)
                        {
                            if (hasItems.ContainsKey(item.Key) == false || hasItems[item.Key] < item.Value)
                                matchFailed = true;
                        }

                        if (matchFailed == false)
                        {
                            return b;
                        }
                    }
                }
            }

            return null; // Nothing found
        }

//
//        protected override bool CanRemoveRequiredItems(CraftingProgressContainer.CraftInfo craftInfo)
//        {
//            if (removeItemsFromCollection.useReferences)
//            {
//                if (removeItemsFromCollection != null)
//                {
//                    foreach (var item in craftInfo.blueprint.requiredItems)
//                    {
//                        uint count = removeItemsFromCollection.GetItemCount(item.item.ID);
//                        if (count < item.amount * craftInfo.craftAmount)
//                        {
//                            return false;
//                        }
//                    }
//
//                    return true;
//                }
//
//                foreach (var item in craftInfo.blueprint.requiredItems)
//                {
//                    uint count = InventoryManager.GetItemCount(item.item.ID, craftInfo.category.alsoScanBankForRequiredItems);
//                    if (count < item.amount * craftInfo.craftAmount)
//                    {
//                        return false;
//                    }
//                }
//
//                return true;
//            }
//
//            return true;
//        }
    }
}