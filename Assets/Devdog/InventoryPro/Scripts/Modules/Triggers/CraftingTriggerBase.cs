using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro;

#pragma warning disable 0067 // Events are considered unused by Unty, even though they are...

namespace Devdog.InventoryPro
{
    /// <summary>
    /// A physical representation of a crafting station.
    /// </summary>
    [RequireComponent(typeof(Trigger))]
    public abstract class CraftingTriggerBase<T> : MonoBehaviour, ICraftingActionValidator, ITriggerCallbacks where T : CraftingWindowBase
    {
        public event CraftingDelegates.CraftStart OnCraftStart;
        public event CraftingDelegates.CraftSuccess OnCraftSuccess;
        public event CraftingDelegates.CraftFailed OnCraftFailed;
        public event CraftingDelegates.CraftProgress OnCraftProgress;
        public event CraftingDelegates.CraftCanceled OnCraftCancelled;



        [SerializeField]
        [Required]
        [ForceCustomObjectPicker]
        private CraftingCategory _craftingCategory;
        public CraftingCategory craftingCategory
        {
            get { return _craftingCategory; }
            set { _craftingCategory = value; }
        }

        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
        }


        public ItemCollectionBase craftingItemsCollection;
        public ItemCollectionBase craftingRewardItemsCollection;


        [NonSerialized]
        protected T craftingWindow;

        [NonSerialized]
        protected Trigger trigger;

        public bool removeItemsOnCraftStart = false;

        [SerializeField]
        private CraftingProgressShareSetting _progressShareSetting = CraftingProgressShareSetting.Default;


        public CraftingProgressContainer progressContainer { get; protected set; }
        public static CraftingProgressSharingDataStructure<CraftingProgressContainer> progressContainerDataStructure { get; set; }
 

        private static CraftingProgressContainer CreateNewProgressContainer(ICraftingActionValidator validator, int id)
        {
            return new CraftingProgressContainer(validator, id, validator.gameObject.GetComponent<AudioSource>());
        }

        protected virtual void Awake()
        {
            SetWindow();

            if (progressContainerDataStructure == null)
            {
                progressContainerDataStructure = new CraftingProgressSharingDataStructure<CraftingProgressContainer>(CreateNewProgressContainer, CreateNewProgressContainer(this, GetInstanceID()));
            }

            progressContainer = progressContainerDataStructure.Get(this, GetInstanceID(), craftingCategory.ID + GetType().GetHashCode(), _progressShareSetting);
            progressContainer.removeItemsOnCraftStart = removeItemsOnCraftStart;
            progressContainer.OnCraftStart += NotifyOnCraftStart;
            progressContainer.OnCraftSuccess += NotifyOnCraftSuccess;
            progressContainer.OnCraftCancelled += NotifyOnCraftCancelled;
            progressContainer.OnCraftFailed += NotifyOnCraftFailed;
            progressContainer.OnCraftProgress += NotifyOnCraftProgress;


            trigger = GetComponent<Trigger>();
            trigger.window = new UIWindowField() { window = craftingWindow.window };
            if (trigger.window.window == null)
            {
                Debug.LogWarning("Crafting trigger created but no CraftingStandardUI found in scene, or not set in managers.", transform);
                return;
            }
        }

        private void NotifyOnCraftProgress(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            if (craftInfo.validator.gameObject != gameObject)
                return;

            if (OnCraftProgress != null)
                OnCraftProgress(craftInfo, progress);
        }

        private void NotifyOnCraftFailed(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (craftInfo.validator.gameObject != gameObject)
                return;

            if (OnCraftFailed != null)
                OnCraftFailed(craftInfo);
        }

        private void NotifyOnCraftCancelled(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            if (craftInfo.validator.gameObject != gameObject)
                return;

            if (OnCraftCancelled != null)
                OnCraftCancelled(craftInfo, progress);
        }

        private void NotifyOnCraftSuccess(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (craftInfo.validator.gameObject != gameObject)
                return;

            if (OnCraftSuccess != null)
                OnCraftSuccess(craftInfo);
        }

        private void NotifyOnCraftStart(CraftingProgressContainer.CraftInfo craftInfo)
        {
            if (craftInfo.validator.gameObject != gameObject)
                return;

            Debug.Log("On craft start on " + gameObject.name + " validator : " + craftInfo.validator.gameObject.name, gameObject);
            if (OnCraftStart != null)
                OnCraftStart(craftInfo);
        }

        public bool CanCraftBlueprint(InventoryPlayer player, CraftingProgressContainer.CraftInfo craftInfo)
        {
            return craftingWindow.CanCraftBlueprint(player, craftInfo);
        }

        public void RemoveRequiredCraftItemsAndCurrency(CraftingProgressContainer.CraftInfo craftInfo)
        {
            craftingWindow.RemoveRequiredCraftItemsAndCurrency(craftInfo);
        }

        public void GiveCraftReward(CraftingProgressContainer.CraftInfo craftInfo)
        {
            craftingWindow.GiveCraftReward(craftInfo);
        }

        private void CraftingWindowOnOnCraftFailed(CraftingProgressContainer.CraftInfo craftInfo)
        {
            UpdateTrigger(craftInfo);
        }

        private void CraftingWindowOnOnCraftCancelled(CraftingProgressContainer.CraftInfo craftInfo, float progress)
        {
            UpdateTrigger(craftInfo);
        }

        private void CraftingWindowOnCraftSuccess(CraftingProgressContainer.CraftInfo craftInfo)
        {
            UpdateTrigger(craftInfo);
        }

        private void UpdateTrigger(CraftingProgressContainer.CraftInfo craftInfo)
        {
            craftingWindow.UseWithTrigger(craftInfo.category, progressContainer, this, craftingItemsCollection, craftingRewardItemsCollection);
        }

        protected abstract void SetWindow();


        public virtual bool OnTriggerUsed(Player player)
        {
            CopyItemsFromDataCollectionToUI();
            craftingWindow.UseWithTrigger(craftingCategory, progressContainer, this, craftingItemsCollection, craftingRewardItemsCollection);

            RegisterEvents();

            return false;
        }

        public virtual bool OnTriggerUnUsed(Player player)
        {
            UnRegisterEvents();

            craftingWindow.UnUseWithTrigger();
            CopyItemsFromUIToDataCollection();

            return false;
        }

        private void SetItemInUIRewardsCollection(uint slot, InventoryItemBase item)
        {
            CopyItemsFromDataCollectionToUI();
        }

        private void SetItemInUICollection(uint slot, InventoryItemBase item)
        {
            CopyItemsFromUIToDataCollection();
        }

        private void RegisterEvents()
        {
            craftingWindow.OnCraftSuccess += CraftingWindowOnCraftSuccess;
            craftingWindow.OnCraftCancelled += CraftingWindowOnOnCraftCancelled;
            craftingWindow.OnCraftFailed += CraftingWindowOnOnCraftFailed;

            if (craftingWindow.mainItemsCollection != null)
            {
                craftingWindow.mainItemsCollection.OnSetItem += SetItemInUICollection;
            }

            if (craftingWindow.storeRewardUIItemsInCollection != null)
            {
                craftingWindow.storeRewardUIItemsInCollection.OnRemovedItem += StoreRewardUIItemsInCollectionOnOnRemovedItem;
            }

            if (craftingRewardItemsCollection != null)
            {
                craftingRewardItemsCollection.OnSetItem += SetItemInUIRewardsCollection;
            }
        }

        private void StoreRewardUIItemsInCollectionOnOnRemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            if (craftingRewardItemsCollection != null)
            {
                craftingRewardItemsCollection.items[slot].item = null;
                craftingRewardItemsCollection.items[slot].Repaint();
            }
        }

        private void UnRegisterEvents()
        {
            craftingWindow.OnCraftSuccess -= CraftingWindowOnCraftSuccess;
            craftingWindow.OnCraftCancelled -= CraftingWindowOnOnCraftCancelled;
            craftingWindow.OnCraftFailed -= CraftingWindowOnOnCraftFailed;

            if (craftingWindow.mainItemsCollection != null)
            {
                craftingWindow.mainItemsCollection.OnSetItem -= SetItemInUICollection;
            }

            if (craftingWindow.storeRewardUIItemsInCollection != null)
            {
                craftingWindow.storeRewardUIItemsInCollection.OnRemovedItem -= StoreRewardUIItemsInCollectionOnOnRemovedItem;
            }

            if (craftingRewardItemsCollection != null)
            {
                craftingRewardItemsCollection.OnSetItem -= SetItemInUIRewardsCollection;
            }
        }

        protected virtual void CopyItemsFromUIToDataCollection()
        {
            if (craftingItemsCollection != null && craftingWindow.mainItemsCollection != null)
            {
                var items = craftingWindow.mainItemsCollection.items.Select(o => o.item).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    craftingItemsCollection[i].item = items[i];
                    craftingItemsCollection[i].Repaint();
                }
            }

            if (craftingRewardItemsCollection != null && craftingWindow.storeRewardUIItemsInCollection != null)
            {
                var items = craftingWindow.storeRewardUIItemsInCollection.items.Select(o => o.item).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    craftingRewardItemsCollection.items[i].item = items[i];
                    craftingRewardItemsCollection.items[i].Repaint();
                }
            }
        }

        protected virtual void CopyItemsFromDataCollectionToUI()
        {
            if (craftingItemsCollection != null && craftingWindow.mainItemsCollection != null)
            {
                var items = craftingItemsCollection.items.Select(o => o.item).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    craftingWindow.mainItemsCollection[i].item = items[i];
                    craftingWindow.mainItemsCollection[i].Repaint();
                }
            }

            if (craftingRewardItemsCollection != null && craftingWindow.storeRewardUIItemsInCollection != null)
            {
                var items = craftingRewardItemsCollection.items.Select(o => o.item).ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    craftingWindow.storeRewardUIItemsInCollection[i].item = items[i];
                    craftingWindow.storeRewardUIItemsInCollection[i].Repaint();
                }
            }
        }
    }
}