using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;
using Devdog.InventoryPro;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Crafting standard")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowStandardUI : CraftingWindowBase
    {
        /// <summary>
        /// Crafting category title
        /// </summary>
        [Header("General UI references")]
        public Text currentCategoryTitle;

        /// <summary>
        /// Crafting category description
        /// </summary>
        public Text currentCategoryDescription;

        [Required]
        public RectTransform blueprintsContainer;


        [Required]
        [Header("Blueprint prefabs")]
        public InventoryCraftingCategoryUI blueprintCategoryPrefab;
        
        /// <summary>
        /// The button used to select the prefab the user wishes to craft.
        /// </summary>
        [Required]
        public InventoryCraftingBlueprintUI blueprintButtonPrefab;

        /// <summary>
        /// A single required item to be shown in the UI.
        /// </summary>
        [Required]
        public ItemCollectionSlotUI blueprintRequiredItemPrefab;


        #region Crafting item page

        [Header("Craft blueprint UI References")]

        [Required]
        public RectTransform blueprintRequiredItemsContainer;
        public InputField blueprintCraftAmountInput;

        #endregion

        [Header("UI window pages")]
        public UIWindowPage noBlueprintSelectedPage;
        public UIWindowPage blueprintCraftPage;

        [Header("Audio & Visuals")]
        public Color itemsAvailableColor = Color.white;
        public Color itemsNotAvailableColor = Color.red;

        #region Pools


        [NonSerialized]
        protected ComponentPool<InventoryCraftingCategoryUI> categoryPool;
        
        [NonSerialized]
        protected ComponentPool<InventoryCraftingBlueprintUI> blueprintPool;

        [NonSerialized]
        protected ComponentPool<ItemCollectionSlotUI> blueprintRequiredItemsPool;

        #endregion

        [NonSerialized]
        protected ItemCollectionBase[] subscribedToCollectionCurrency = new ItemCollectionBase[0];

        protected override void Awake()
        {
            if (blueprintCategoryPrefab != null)
            {
                categoryPool = new ComponentPool<InventoryCraftingCategoryUI>(blueprintCategoryPrefab, 16);
            }

            blueprintPool = new ComponentPool<InventoryCraftingBlueprintUI>(blueprintButtonPrefab, 128);
            blueprintRequiredItemsPool = new ComponentPool<ItemCollectionSlotUI>(blueprintRequiredItemPrefab, 8);

            base.Awake();
        }
        
        protected override void InstanceOnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            base.InstanceOnPlayerChanged(oldPlayer, newPlayer);

            if (oldPlayer != null)
            {
                foreach (var col in subscribedToCollectionCurrency)
                {
                    col.OnAddedItem -= OnAddedPlayerItem;
                    col.OnRemovedItem -= OnRemovedPlayerItem;
                }
            }

            subscribedToCollectionCurrency = InventoryManager.GetLootToCollections();
            foreach (var col in subscribedToCollectionCurrency)
            {
                col.OnAddedItem += OnAddedPlayerItem;
                col.OnRemovedItem += OnRemovedPlayerItem;
            }
        }

        private void OnRemovedPlayerItem(InventoryItemBase item, uint itemid, uint slot, uint amount)
        {
            if(currentBlueprint != null)
                SetCraftingBlueprint(currentBlueprint);
        }

        private void OnAddedPlayerItem(IEnumerable<InventoryItemBase> items, uint amount, bool camefromcollection)
        {
            if (currentBlueprint != null)
                SetCraftingBlueprint(currentBlueprint);
        }

        protected override void OnCraftButtonClicked()
        {
            CraftCurrentlySelectedBlueprint(GetCraftInputFieldAmount());
        }

        protected virtual int GetCraftInputFieldAmount()
        {
            if(blueprintCraftAmountInput != null)
                return int.Parse(blueprintCraftAmountInput.text);

            return 1;
        }

        protected virtual void ValidateCraftInputFieldAmount()
        {
            int amount = GetCraftInputFieldAmount();
            amount = Mathf.Clamp(amount, 1, 999);

            blueprintCraftAmountInput.text = amount.ToString();
        }

        public override CraftingBlueprint[] GetBlueprints(CraftingCategory category)
        {
            return currentCategory.blueprints.Where(o => o.playerLearnedBlueprint).ToArray();
        }

        public override void SetCraftingCategory(CraftingCategory category)
        {
//            if (currentCategory == category)
//            {
//                return;
//            }
            
            base.SetCraftingCategory(category);

            categoryPool.DestroyAll();
            blueprintPool.DestroyAll();
            if (blueprintCraftAmountInput != null)
                blueprintCraftAmountInput.text = "1"; // Reset
            
            if(currentCategoryTitle != null)
                currentCategoryTitle.text = category.name;
        
            if (currentCategoryDescription != null)
                currentCategoryDescription.text = category.description;

            if (noBlueprintSelectedPage != null)
                noBlueprintSelectedPage.Show();

//            var blueprints = GetBlueprints(category);
//            if (blueprintCraftPage != null && blueprints.Length > 0)
//            {
//                SetBlueprint(blueprints[0]); // Select first blueprint
//                blueprintCraftPage.Show();
//            }

            ItemCategory lastItemCategory = null;
            Button firstButton = null;
            foreach (var b in GetBlueprints(category))
            {
                if (b.playerLearnedBlueprint == false)
                    continue;

                var blueprintObj = blueprintPool.Get();
                blueprintObj.transform.SetParent(blueprintsContainer);
                InventoryUtility.ResetTransform(blueprintObj.transform);
                blueprintObj.Repaint(b);

                if (blueprintCategoryPrefab != null)
                {
                    Assert.IsTrue(b.resultItems.Length > 0, "No reward items set");
                    var item = b.resultItems.First().item;
                    Assert.IsNotNull(item, "Empty reward row on blueprint!");

                    if (lastItemCategory != item.category)
                    {
                        lastItemCategory = item.category;

                        var uiCategory = categoryPool.Get();
                        uiCategory.Repaint(category, item.category);

                        uiCategory.transform.SetParent(blueprintsContainer);
                        blueprintObj.transform.SetParent(uiCategory.container);

                        InventoryUtility.ResetTransform(uiCategory.transform);
                        InventoryUtility.ResetTransform(blueprintObj.transform);
                    }
                }

                if (firstButton == null)
                {
                    firstButton = blueprintObj.button;
                }

                var bTemp = b; // Store capture list, etc.
                blueprintObj.button.onClick.AddListener(() =>
                {
                    currentBlueprint = bTemp;
                    SetCraftingBlueprint(currentBlueprint);

                    if (blueprintCraftPage != null && blueprintCraftPage.isVisible == false)
                    {
                        blueprintCraftPage.Show();
                    }
                });
            }

            if (firstButton != null)
            {
                firstButton.Select();
            }
        }


        public override void SetCraftingBlueprint(CraftingBlueprint blueprint)
        {
            base.SetCraftingBlueprint(blueprint);

            if (window.isVisible == false)
            {
                return;
            }

            blueprintRequiredItemsPool.DestroyAll();
            foreach (var item in blueprint.requiredItems)
            {
                var ui = blueprintRequiredItemsPool.Get();
                item.item.currentStackSize = (uint)item.amount;
                ui.transform.SetParent(blueprintRequiredItemsContainer);
                InventoryUtility.ResetTransform(ui.transform);

                ui.item = item.item;
                if (InventoryManager.GetItemCount(item.item.ID, currentCategory.alsoScanBankForRequiredItems) >= item.amount)
                    ui.icon.color = itemsAvailableColor;
                else
                    ui.icon.color = itemsNotAvailableColor;

                ui.Repaint();
                item.item.currentStackSize = 1; // Reset
            }
        }
    }
}