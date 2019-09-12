using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [HelpURL("http://devdog.nl/documentation/crafting/")]
    public abstract class CraftingWindowBase : MonoBehaviour, ICraftingActionValidator
    {
        [SerializeField]
        [Required]
        private CraftingCategory _startCraftingCategory;
        public CraftingCategory startCraftingCategory
        {
            get { return _startCraftingCategory; }
        }

        [SerializeField]
        private bool _cancelCraftingOnWindowClose = true;

        [SerializeField]
        private bool _forceSingleInstance = false;


        [Tooltip("The main item collection used to store items into. Required on layout crafting.")]
        public ItemCollectionBase mainItemsCollection;
        public ItemCollectionBase removeItemsFromCollection
        {
            get
            {
                if (overrideRemoveItemsFromCollection != null)
                    return overrideRemoveItemsFromCollection;

                return mainItemsCollection;
            }
        }
        public ItemCollectionBase overrideRemoveItemsFromCollection { get; protected set; }


        [Tooltip("All rewards will be stored in this collection. If the field is empty, the items will be added to the player's inventory auto.")]
        public ItemCollectionBase storeRewardUIItemsInCollection;       
        public ItemCollectionBase storeRewardItemsInCollection
        {
            get
            {
                if (overrideStoreRewardItemsInCollection != null)
                    return overrideStoreRewardItemsInCollection;

                return storeRewardUIItemsInCollection;
            }
        }
        public ItemCollectionBase overrideStoreRewardItemsInCollection { get; protected set; }


        public event CraftingDelegates.CraftStart OnCraftStart;
        public event CraftingDelegates.CraftSuccess OnCraftSuccess;
        public event CraftingDelegates.CraftFailed OnCraftFailed;
        public event CraftingDelegates.CraftProgress OnCraftProgress;
        public event CraftingDelegates.CraftCanceled OnCraftCancelled;


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

        /// <summary>
        /// The progress container contains all of the crafting progress. This reference can be shared between triggers, categories, etc; However you like.
        /// </summary>
        [NonSerialized]
        private CraftingProgressContainer _progressContainer;
        public CraftingProgressContainer progressContainer
        {
            get { return _progressContainer; }
            set
            {
                if (_progressContainer != null)
                {
                    progressContainer.OnCraftProgress -= NotifyOnCraftProgress;
                    progressContainer.OnCraftCancelled -= NotifyOnCraftCancelled;
                    progressContainer.OnCraftFailed -= NotifyOnCraftFailed;
                    progressContainer.OnCraftStart -= NotifyOnCraftStart;
                    progressContainer.OnCraftSuccess -= NotifyOnCraftSuccess;
                }

                _progressContainer = value;

                if (_progressContainer != null)
                {
                    progressContainer.OnCraftProgress += NotifyOnCraftProgress;
                    progressContainer.OnCraftCancelled += NotifyOnCraftCancelled;
                    progressContainer.OnCraftFailed += NotifyOnCraftFailed;
                    progressContainer.OnCraftStart += NotifyOnCraftStart;
                    progressContainer.OnCraftSuccess += NotifyOnCraftSuccess;
                }
            }
        }

        public CraftingCategory currentCategory { get; protected set; }
        public CraftingBlueprint currentBlueprint { get; protected set; }

        [Required]
        public RectTransform blueprintItemResultContainer;

        [Required]
        public ItemCollectionSlotUIBase blueprintItemResultPrefab;


        #region UI Elements

        [Header("UI elements")]
        public UnityEngine.UI.Text blueprintTitle;
        public UnityEngine.UI.Text blueprintDescription;

        public CurrencyGroupUI blueprintCraftCost;

        public UIShowValue blueprintCraftingProgress = new UIShowValue();
//        public UnityEngine.UI.Slider blueprintCraftProgressSlider;

        [Required]
        public UnityEngine.UI.Button blueprintCraftButton;

        #endregion

        
        [Header("Audio & Visuals")]
        public AnimationClip craftAnimation;
        //public AnimationClip successAnimation;
        //public AnimationClip failedAnimation;
        //public AnimationClip canceledAnimation;

        [NonSerialized]
        protected ItemCollectionBase[] subscribedToCollection = new ItemCollectionBase[0];

        protected virtual void Awake()
        {
            currentCategory = startCraftingCategory;
            progressContainer = new CraftingProgressContainer(this, GetInstanceID(), GetComponent<AudioSource>());

            window.OnShow += OnWindowShown;
            window.OnHide += OnWindowHidden;
            blueprintCraftButton.onClick.AddListener(OnCraftButtonClicked);

            PlayerManager.instance.OnPlayerChanged += InstanceOnPlayerChanged;
            if (PlayerManager.instance.currentPlayer != null)
            {
                InstanceOnPlayerChanged(null, PlayerManager.instance.currentPlayer);
            }
        }

        protected virtual void Start()
        {

        }

        protected virtual void InstanceOnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            if (oldPlayer != null)
            {
                foreach (var col in subscribedToCollection)
                {
                    col.OnCurrencyChanged -= OnPlayerCurrencyChanged;
                }
            }

            subscribedToCollection = InventoryManager.GetLootToCollections();
            foreach (var col in subscribedToCollection)
            {
                col.OnCurrencyChanged += OnPlayerCurrencyChanged;
            }
        }

        private void OnPlayerCurrencyChanged(float amountBefore, CurrencyDecorator decorator)
        {
            if (currentBlueprint != null)
            {
                SetCraftingBlueprint(currentBlueprint);
            }
        }

        protected virtual void OnWindowShown()
        {
            SetCraftingCategory(currentCategory);
        }

        protected virtual void OnWindowHidden()
        {
            if (_cancelCraftingOnWindowClose)
            {
                progressContainer.CancelActiveCraftAndClearQueue();
            }
        }

        protected abstract void OnCraftButtonClicked();

        public virtual void CraftCurrentlySelectedBlueprint(int amount)
        {
            if (progressContainer.isCrafting)
            {
                // TODO: Have to remove for QUEUE // Cancel queue item?
                progressContainer.CancelActiveCraftAndClearQueue();
                return;
            }

            if (currentBlueprint == null)
            {
                DevdogLogger.LogWarning("No blueprint selected, can't craft.", this);
                return;
            }

            progressContainer.AddBlueprintToCraftingQueue(PlayerManager.instance.currentPlayer.inventoryPlayer, currentCategory, currentBlueprint, amount, storeRewardItemsInCollection, removeItemsFromCollection);
        }


        protected virtual void NotifyOnCraftSuccess(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (OnCraftSuccess != null)
                OnCraftSuccess(craftInfo);
        }

        protected virtual void NotifyOnCraftStart(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (OnCraftStart != null)
                OnCraftStart(craftInfo);
        }

        protected virtual void NotifyOnCraftFailed(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (OnCraftFailed != null)
                OnCraftFailed(craftInfo);
        }

        protected virtual void NotifyOnCraftCancelled(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            if (OnCraftCancelled != null)
                OnCraftCancelled(craftInfo, progress);
        }

        protected virtual void NotifyOnCraftProgress(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            blueprintCraftingProgress.Repaint(progress, 1f);

            if (OnCraftProgress != null)
                OnCraftProgress(craftInfo, progress);
        }

        public virtual void UseWithTrigger(CraftingCategory category, CraftingProgressContainer progressContainer, ICraftingActionValidator validator, ItemCollectionBase removeItemsFromCollection, ItemCollectionBase storeRewardItemsInCollection)
        {
            Assert.IsNotNull(category);
            Assert.IsNotNull(progressContainer);

            overrideRemoveItemsFromCollection = removeItemsFromCollection;
            overrideStoreRewardItemsInCollection = storeRewardItemsInCollection;

            if (_forceSingleInstance)
            {
                this.progressContainer = new CraftingProgressContainer(this, GetInstanceID(), GetComponent<AudioSource>());
            }
            else
            {
                this.progressContainer = progressContainer;
                this.progressContainer.validator = validator;
            }

            SetCraftingCategory(category);
            window.Show();
        }

        public virtual void UnUseWithTrigger()
        {
            // In case someone would like to use it with a trigger and without.
            // Need a new object, otherwise progress would be stored in the last used trigger.
            progressContainer = new CraftingProgressContainer(this, GetInstanceID(), GetComponent<AudioSource>());
            overrideRemoveItemsFromCollection = null;
            overrideStoreRewardItemsInCollection = null;

            window.Hide();
        }

        public virtual void SetCraftingCategory(CraftingCategory category)
        {
            Assert.IsNotNull(category, "Given crafting category is null!");
            currentCategory = category;
        }

        public virtual void SetCraftingBlueprint(CraftingBlueprint blueprint)
        {
            currentBlueprint = blueprint;

            if (blueprint != null)
            {
                // Set all the details for the blueprint.
                if (blueprintTitle != null)
                    blueprintTitle.text = blueprint.name;

                if (blueprintDescription != null)
                    blueprintDescription.text = blueprint.description;

                if (blueprintCraftCost != null)
                    blueprintCraftCost.Repaint(blueprint.craftingCost);

            }
            else
            {
                // Set all the details for the blueprint.
                if (blueprintTitle != null)
                    blueprintTitle.text = string.Empty;

                if (blueprintDescription != null)
                    blueprintDescription.text = string.Empty;

                if (blueprintCraftCost != null)
                    blueprintCraftCost.Repaint(new CurrencyDecorator());
            }

            blueprintCraftingProgress.Repaint(0f, 1f);
            SetBlueprintResults(blueprint);
        }

        public virtual CraftingBlueprint[] GetBlueprints(CraftingCategory category)
        {
            return currentCategory.blueprints;
        }

        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        public virtual bool CanCraftBlueprint(CraftingProgressContainer.CraftInfo craftInfo)
        {
            return CanCraftBlueprint(PlayerManager.instance.currentPlayer.inventoryPlayer, craftInfo);
        }

        public virtual bool CanCraftBlueprint(InventoryPlayer player, CraftingProgressContainer.CraftInfo craftInfo)
        {
            // Required properties?
            if (player.characterUI != null)
            {
                foreach (var propLookup in craftInfo.blueprint.usageRequirement)
                {
                    if (propLookup.CanUse(player) == false)
                    {
                        InventoryManager.langDatabase.craftingCannotStatNotValid.Show(craftInfo.blueprint.name, craftInfo.blueprint.description, propLookup.stat.statName);
                        return false;
                    }
                }
            }

            if (CanRemoveRequiredItems(craftInfo) == false)
            {
                InventoryManager.langDatabase.craftingDontHaveRequiredItems.Show(craftInfo.blueprint.requiredItems[0].item.name, craftInfo.blueprint.requiredItems[0].item.description, craftInfo.blueprint.name);
                return false;
            }

            if (CanAddItemsRewardItems(craftInfo) == false)
            {
                InventoryManager.langDatabase.collectionFull.Show(craftInfo.blueprint.name, craftInfo.blueprint.description, "Reward collection");
                return false;
            }

            // Enough currency?
            if (CanRemoveRequiredCurrency(craftInfo) == false)
            {
                InventoryManager.langDatabase.userNotEnoughGold.Show(craftInfo.blueprint.name, craftInfo.blueprint.description, craftInfo.craftAmount, craftInfo.blueprint.craftingCost.ToString(craftInfo.craftAmount));
                return false;
            }

            return true;
        }

        private bool CanRemoveRequiredCurrency(CraftingProgressContainer.CraftInfo craftInfo)
        {
            return InventoryManager.CanRemoveCurrency(craftInfo.blueprint.craftingCost, true, craftInfo.category.alsoScanBankForRequiredItems);
        }

        protected virtual bool CanAddItemsRewardItems(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (craftInfo.storeItemsInCollection != null)
            {
                var before = craftInfo.storeItemsInCollection.canPutItemsInCollection;
                craftInfo.storeItemsInCollection.canPutItemsInCollection = true;


                var l = new List<ItemAmountRow>();
                for (int i = 0; i < craftInfo.craftAmount; i++)
                {
                    l.AddRange(craftInfo.blueprint.resultItems);
                }

                if (craftInfo.storeItemsInCollection.CanAddItems(l.ToArray()) == false)
                {
                    return false;
                }

                bool stored = craftInfo.storeItemsInCollection.CanAddItems(l.ToArray());

                craftInfo.storeItemsInCollection.canPutItemsInCollection = before;
                return stored;
            }

            if (InventoryManager.IsInventoryCollection(removeItemsFromCollection))
            {
                return InventoryManager.CanRemoveItemsThenAdd(craftInfo.blueprint.resultItems, craftInfo.blueprint.requiredItems);
            }

            return InventoryManager.CanAddItems(craftInfo.blueprint.resultItems);
        }

        protected virtual bool CanRemoveRequiredItems(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (craftInfo.removeItemsFromCollection != null && craftInfo.removeItemsFromCollection.useReferences == false)
            {
                foreach (var item in craftInfo.blueprint.requiredItems)
                {
                    uint count = craftInfo.removeItemsFromCollection.GetItemCount(item.item.ID);
                    if (count < item.amount * craftInfo.craftAmount)
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var item in craftInfo.blueprint.requiredItems)
            {
                uint count = InventoryManager.GetItemCount(item.item.ID, craftInfo.category.alsoScanBankForRequiredItems);
                if (count < item.amount * craftInfo.craftAmount)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void RemoveRequiredCraftItemsAndCurrency(CraftingProgressContainer.CraftInfo craftInfo)
        {
            craftInfo.removedCraftItems = true;

            if (craftInfo.removeItemsFromCollection != null && craftInfo.removeItemsFromCollection.useReferences == false)
            {
                foreach (var item in craftInfo.blueprint.requiredItems)
                {
                    craftInfo.removeItemsFromCollection.RemoveItem(item.item.ID, item.amount);
                }
            }
            else
            {
                foreach (var item in craftInfo.blueprint.requiredItems)
                {
                    InventoryManager.RemoveItem(item.item.ID, item.amount, craftInfo.category.alsoScanBankForRequiredItems);
                }
            }

            InventoryManager.RemoveCurrency(craftInfo.blueprint.craftingCost);
        }

        public virtual void GiveCraftReward(CraftingProgressContainer.CraftInfo craftInfo)
        {
            var itemsToAdd = InventoryItemUtility.RowsToItems(craftInfo.blueprint.resultItems, true);

#if UFPS_MULTIPLAYER
            foreach (var item in itemsToAdd)
            {
                var i = item as Devdog.InventoryPro.Integration.UFPS.UFPSInventoryItemBase;
                if (i != null && Devdog.InventoryPro.Integration.UFPS.Multiplayer.InventoryMPUFPSPickupManager.instance != null)
                {
                    Devdog.InventoryPro.Integration.UFPS.Multiplayer.InventoryMPUFPSPickupManager.instance.InstantiateAndRegisterPickupOnAllClients(i);
                }
            }
#endif

            if (craftInfo.storeItemsInCollection != null)
            {
                var before = craftInfo.storeItemsInCollection.canPutItemsInCollection;
                craftInfo.storeItemsInCollection.canPutItemsInCollection = true;

                bool added = craftInfo.storeItemsInCollection.AddItems(itemsToAdd);

                craftInfo.storeItemsInCollection.canPutItemsInCollection = before;

                Assert.IsTrue(added, "Couldn't add items even though check passed. Please report this error + stack trace.");
            }
            else
            {
                // Store crafted item
                bool added = InventoryManager.AddItems(itemsToAdd);
                Assert.IsTrue(added, "Couldn't add items even though check passed. Please report this error + stack trace.");
            }
        }

        protected virtual void SetBlueprintResults(CraftingBlueprint blueprint)
        {
            if (blueprintItemResultContainer != null)
            {
                if (blueprint != null)
                {
                    foreach (Transform child in blueprintItemResultContainer)
                    {
                        Destroy(child.gameObject);
                    }

                    foreach (var row in blueprint.resultItems)
                    {
                        var wrapper = Instantiate<ItemCollectionSlotUIBase>(blueprintItemResultPrefab);

                        wrapper.item = row.item;
                        wrapper.item.currentStackSize = row.amount;
                        wrapper.Repaint();
                        wrapper.item.currentStackSize = 1; // Reset

                        wrapper.transform.SetParent(blueprintItemResultContainer);
                        InventoryUtility.ResetTransform(wrapper.transform);
                    }
                }
                else
                {
                    foreach (Transform child in blueprintItemResultContainer)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }
}
