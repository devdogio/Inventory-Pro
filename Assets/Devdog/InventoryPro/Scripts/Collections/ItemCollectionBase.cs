using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.InventoryPro.UI;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// An abstract base class for collections. When creating a new collection, extend from this class.
    /// (not marked abstract because of 3rd party integrations and unity serialization not handling abstract classes).
    /// </summary>
    public partial class ItemCollectionBase : MonoBehaviour, IEnumerable<ICollectionItem>
    {
        #region Delegates

        /// <summary>
        /// Called when an item was added to this collection
        /// </summary>
        /// <param name="item">A list of the item that was added, all slots where the item was stored. If the item doesn't fit 1 slot it's broken up into multiple slots.</param>
        /// <param name="amount">The amount of items that have been added (stack size).</param>
        /// <param name="cameFromCollection">Did the item come from another collection or is it looted?</param>
        public delegate void AddedItem(IEnumerable<InventoryItemBase> items, uint amount, bool cameFromCollection);

        /// <summary>
        /// Called whenever the currency changes inside of this collection.
        /// </summary>
        /// <param name="amountBefore"></param>
        /// <param name="decorator"></param>
        public delegate void CurrencyChanged(float amountBefore, CurrencyDecorator decorator);

        /// <summary>
        /// Called when an item is dropped from this collection.
        /// </summary>
        /// <param name="item">The item that has been dropped.</param>
        /// <param name="slot">Slot from where the item was dropped.</param>
        /// <param name="droppedObj">The actual 3D object that got dropped. This can either be the "DropObject" that has been set in the cateogry or the actual model.</param>
        public delegate void DroppedItem(InventoryItemBase item, uint slot, GameObject droppedObj);

        /// <summary>
        /// Called when an reference is removed from this collection. Note that it will only be called if "Use references" is enabled in the inspector.
        /// </summary>
        /// <param name="item">The reference that was removed.</param>
        /// <param name="slot">The slot from where the reference was removed.</param>
        public delegate void RemovedReference(InventoryItemBase item, uint slot);

        /// <summary>
        /// Called when an item was removed from this collection, either by dropping or selling.
        /// <b>Item is not fired when an item of a stack is used, only the last item, as this destroys the stack.</b>
        /// </summary>
        /// <param name="item">The item that was removed, note that it can be null if the item was destroyed in the process.</param>
        /// <param name="itemID">ItemID of the item that was removed from the inventory. ID instead of an actual item, because it could have been destroyed in the process.</param>
        /// <param name="slot">The slot from where the item was removed.</param>
        /// <param name="amount">Amount of items that were removed of this type.</param>
        public delegate void RemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);

        /// <summary>
        /// Called when a stack is un-stacked, aka broken down into 2 smaller stacks.
        /// </summary>
        /// <param name="startSlot">The original location where the unstack started</param>
        /// <param name="endSlot">The 2nd slot used to store the 2nd stack.</param>
        /// <param name="amount">Amount of items that were unstacked</param>
        public delegate void UnstackedItem(ItemCollectionBase fromCollection, uint startSlot, ItemCollectionBase toCollection, uint endSlot, uint amount);


        /// <summary>
        /// Called when 2 slots of the same type are merged together into a single slot.
        /// </summary>
        /// <param name="fromCollection"></param>
        /// <param name="fromSlot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        public delegate void MergedSlots(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot);

        /// <summary>
        /// Called when an item was used from this collection.
        /// </summary>
        /// <param name="item">The item that was used, note that it can be null if the item was destroyed in the process.</param>
        /// <param name="itemID">ID of the item used.</param>
        /// <param name="slot">Slot of the item used.</param>
        /// <param name="amount">Amount of items used.</param>
        public delegate void UsedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);


        /// <summary>
        /// Called when a reference is used inside this collection.
        /// </summary>
        /// <param name="actualItem">The item used (original item) that the reference is bound to.</param>
        /// <param name="itemID"></param>
        /// <param name="referenceSlot"></param>
        /// <param name="amountUsed"></param>
        public delegate void UsedReference(InventoryItemBase actualItem, uint itemID, uint referenceSlot, uint amountUsed);



        public delegate void SetItemDelegate(uint slot, InventoryItemBase item);


        /// <summary>
        /// Tried to add an item to the collection, but it was full, item was denied.
        /// </summary>
        /// <param name="item">Item tried to add</param>
        /// <param name="cameFromCollection">Did this item came from a collection (loot)?</param>
        public delegate void AddedItemCollectionFull(InventoryItemBase item, bool cameFromCollection);

        /// <summary>
        /// Called when this collection is resized.
        /// </summary>
        /// <param name="fromSize">Old size</param>
        /// <param name="toSize">New size (can be smaller than fromSize)</param>
        public delegate void CollectionResized(uint fromSize, uint toSize);

        /// <summary>
        /// Called when the collection is sorted.
        /// </summary>
        public delegate void Sorted();
        
        /// <summary>
        /// Called when 2 items were swapped inside this collection.
        /// Called when 2 items were swapped between 2 different collections.
        /// </summary>
        /// <param name="fromCollection">Start collection</param>
        /// <param name="fromSlot">Start slot</param>
        /// <param name="toCollection">End collection</param>
        /// <param name="toSlot">End slot</param>
        public delegate void SwappedItems(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot);


        /// <summary>
        /// Returns true if the item can be added to the collection, and false when the item cannot be added.
        /// Allows you to add your own conditions to collections.
        /// </summary>
        /// <param name="item"></param>
        public delegate bool CanAddItemDelegate(InventoryItemBase item);

        #endregion


        #region Variables 
        

        private ICollectionItem[] _items = new ICollectionItem[0];
        /// <summary>
        /// Cache of the items / visuals.
        /// Only set this if you know what you're doing...
        /// </summary>
        public virtual ICollectionItem[] items
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
        
        public CurrencyDecoratorCollection currenciesGroup { get; set; }
        

        /// <summary>
        /// Amount of items / slots in this collection, empty items are also counted.
        /// </summary>
        public uint collectionSize
        {
            get
            {
                return (uint)_items.Length;
            }
        }

        /// <summary>
        /// Easy accessor, also prevents users from changing the InventoryUIItemWrapper object.
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public ICollectionItem this[uint i]
        {
            get
            {
                return items[i];
            }
        }
        public ICollectionItem this[int i]
        {
            get
            {
                return items[i];
            }
        }

        /// <summary>
        /// The name of this collection used in notifications or custom code
        /// </summary>
        public string collectionName = "MyCollection";

        ///// <summary>
        ///// Does this collection belong to a player?
        ///// </summary>
        //public InventoryPlayer[] belongsToPlayers;

        /// <summary>
        /// Use weight calculations??
        /// </summary>
        public bool restrictByWeight = false;

        /// <summary>
        /// Restrict this collection to a max amount of weight
        /// </summary>
        //[Range(0.0f, 999.0f)]
        public float restrictMaxWeight = 100.0f;


        /// <summary>
        /// If true references will be used, and the objects won't be moved. Useful for skill bars and ... well references.
        /// </summary>
        [SerializeField]
        private bool _useReferences = false;
        public bool useReferences
        {
            get { return _useReferences; }
            protected set { _useReferences = value; }
        }


        /// <summary>
        /// Can this collection contain currencies?
        /// </summary>
        public bool canContainCurrencies = true;
        
        /// <summary>
        /// Can an item be dropped directly from this collection?
        /// Auto disabled if useReferences is set to true.
        /// </summary>
        public bool canDropFromCollection = false;

        /// <summary>
        /// Does the player or a player carry this collection around?
        /// </summary>
        public bool canUseItemsFromReference = false;

        /// <summary>
        /// Can an item be used directly from this collection?
        /// </summary>
        public bool canUseFromCollection = false;

        /// <summary>
        /// Can items be dragged inside the collection?
        /// </summary>
        public bool canDragInCollection = true;

        /// <summary>
        /// Can items be stored into this collection / moved into?
        /// </summary>
        public bool canPutItemsInCollection = true;

        /// <summary>
        /// Can items be stacked in this collection, if false items will be broken up in stacks of 1.
        /// </summary>
        public bool canStackItemsInCollection = true;

        /// <summary>
        /// Can items be unstacked inside this collection?
        /// </summary>
        public bool canUnstackItemsInCollection = true;
        
        /// <summary>
        /// Manually create the collection, either through code or the inspector.
        /// </summary>
        public bool manuallyDefineCollection = false;
        

        public bool moveObjectsToChildren { get; protected set; }


        /// <summary>
        /// The container that holds the wrappers of this collection.
        /// </summary>
        [Required]
        public Transform container;

        /// <summary>
        /// Item button prefab, used to create UI elements.
        /// </summary>
        [Tooltip("The prefab used to create UI elements, if left empty the default will be chosen from the settings.")]
        public GameObject itemButtonPrefab;


        public InventoryItemFilters filters = new InventoryItemFilters();
        private ItemCollectionBaseAddCounter _counter = new ItemCollectionBaseAddCounter();
        public ItemCollectionBaseAddCounter counter
        {
            get { return _counter; }
        }

        /// <summary>
        /// The parent of the items in this collection
        /// </summary>
        protected Transform containerItemsParent;

        /// <summary>
        /// The max size of the collection, this many empty item slots will be created.
        /// </summary>
        public virtual uint initialCollectionSize { get; set; }

        public virtual bool isEmpty
        {
            get { return items.All(o => o.item == null && currenciesGroup.lookups.All(x => x.amount <= 0f)); }
        }


        [NonSerialized]
        private uint _colsCount = 6;
        public uint colsCount
        {
            get { return (uint)Mathf.Min(items.Length, _colsCount); }
            protected set { _colsCount = value; }
        }
        public uint rowsCount
        {
            get
            {
                return (uint)Mathf.Max(1, Mathf.CeilToInt((float) items.Length/colsCount));
            }
        }

        private DynamicLayoutGroup _layoutGroup;

        public DynamicLayoutGroup layoutGroup
        {
            get
            {
                if (_layoutGroup == null)
                {
                    _layoutGroup = container.GetComponent<DynamicLayoutGroup>();
                    if (_layoutGroup != null)
                    {
                        colsCount = (uint)_layoutGroup.columnsCount;
                    }
                }

                return _layoutGroup;
            }
            protected set
            {
                _layoutGroup = value;
                if (_layoutGroup != null)
                {
                    colsCount = (uint)_layoutGroup.columnsCount;
                }
            }
        }

        public bool ignoreItemLayoutSizes = false;

        private List<CanAddItemDelegate> _canAddItemToCollectionConditionals = new List<CanAddItemDelegate>();
        private bool _isDirty = true;

        public int disableCounterRebuildBlocks { get; set; }
        public bool disableCounterRebuild
        {
            get { return disableCounterRebuildBlocks > 0; }
        }

        /// <summary>
        /// Returns true if the item can be used, and false when the item cannot be used.
        /// Allows you to add your own conditions to items.
        /// </summary>
        public List<CanAddItemDelegate> canAddItemToCollectionConditionals
        {
            get { return _canAddItemToCollectionConditionals; }
            protected set { _canAddItemToCollectionConditionals = value; }
        }


        #endregion


        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        public event AddedItem OnAddedItem;

        /// <summary>
        /// Occurs when the currency inside this collection is changed.
        /// </summary>
        public event CurrencyChanged OnCurrencyChanged;

        /// <summary>
        /// Occurs when an item is dropped from a collection.
        /// <b>Event is NOT fired when an item is used!</b>
        /// </summary>
        public event DroppedItem OnDroppedItem;

        /// <summary>
        /// Occurs when an item inside this collection is used, some items reduce in stackSize when used ( Consumable for example ).
        /// </summary>
        public event UsedItem OnUsedItem;

        /// <summary>
        /// Occurs when a reference inside this collection is used, some items reduce in stackSize when used ( Consumable for example ).
        /// </summary>
        public event UsedReference OnUsedReference;

        /// <summary>
        /// Occurs when an reference is removed from a collection.
        /// </summary>
        public event RemovedReference OnRemovedReference;

        /// <summary>
        /// Occurs when an item is removed from a collection, either dropped, stored, sold, swapped, last item in stack used, etc.
        /// <b>Item is not fired when an item of a stack is used, only the last item, as this destroys the stack.</b>
        /// Using itemID instead of object, because the object could have been destroyed in the process.
        /// Does not fire for references, use OnRemovedFerence instead
        /// </summary>
        public event RemovedItem OnRemovedItem;

        /// <summary>
        /// When an item is unstacked, the event is fired, if the inventory was full, no event will be fired.
        /// </summary>
        public event UnstackedItem OnUnstackedItem;

        public event SetItemDelegate OnSetItem;

        /// <summary>
        /// Fired when 2 slots of the same type are merged into a single stack
        /// </summary>
        public event MergedSlots OnMergedSlots;

        /// <summary>
        /// Occurs when the collection is resized (whens slots are added or removed).
        /// </summary>
        public event CollectionResized OnResized;

        /// <summary>
        /// Occurs when the collection is sorted.
        /// The collection is sorted using the collectionSorter,
        /// you can create your own sorting behavior by creating a custom class and implementing IInventoryCollectionSorter.
        /// </summary>
        public event Sorted OnSorted;

        /// <summary>
        /// When 2 items are swapped the event is fired.
        /// Note: The event is fired on both collections, if it is swapped within the same collection only that collection will be called once.
        /// toCollection is the final position where the item was stored.
        /// </summary>
        public event SwappedItems OnSwappedItems;


        public ItemCollectionBase()
        {
            moveObjectsToChildren = true;
        }

        /// <summary>
        /// Unity monobehaviour method, creates the collection.
        /// </summary>
        protected virtual void Awake()
        {
            container = container ?? transform;

            var a = new GameObject(GetType().Name);
            containerItemsParent = a.transform;
            containerItemsParent.SetParent(InventoryManager.instance.collectionObjectsParent);

            currenciesGroup = new CurrencyDecoratorCollection(true);
            RegisterCurrencyEvents();

            Awake2();
            Awake3();
            Awake4();
            Awake5();
            Awake6();
            Awake7();
            Awake8();
            Awake9();

            layoutGroup = container.GetComponent<DynamicLayoutGroup>(); // Allowed to be null
            FillUI();
        }

        public void RegisterCurrencyEvents()
        {
            foreach (var currency in currenciesGroup.lookups)
            {
                var c = currency;
                c.OnCurrencyChanged += NotifyCurrencyChanged;
            }
        }

        public void UnRegisterCurrencyEvents()
        {
            foreach (var currency in currenciesGroup.lookups)
            {
                var c = currency;
                c.OnCurrencyChanged -= NotifyCurrencyChanged;
            }
        }

        /// <summary>
        /// These Awake..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void Awake2();
        partial void Awake3();
        partial void Awake4();
        partial void Awake5();
        partial void Awake6();
        partial void Awake7();
        partial void Awake8();
        partial void Awake9();


        protected virtual void Start()
        {
            Start2();
            Start3();
            Start4();
            Start5();
            Start6();
            Start7();
            Start8();
            Start9();
        }

        /// <summary>
        /// These Start..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void Start2();
        partial void Start3();
        partial void Start4();
        partial void Start5();
        partial void Start6();
        partial void Start7();
        partial void Start8();
        partial void Start9();

        /// <summary>
        /// Unity MonoBehaviour method
        /// </summary>
        public virtual void OnDestroy()
        {
            OnDestroy2();
            OnDestroy3();
            OnDestroy4();
            OnDestroy5();
            OnDestroy6();
            OnDestroy7();
            OnDestroy8();
            OnDestroy9();
        }

        /// <summary>
        /// These OnDestroy..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void OnDestroy2();
        partial void OnDestroy3();
        partial void OnDestroy4();
        partial void OnDestroy5();
        partial void OnDestroy6();
        partial void OnDestroy7();
        partial void OnDestroy8();
        partial void OnDestroy9();


        /// <summary>
        /// Creates all visual wrapper objects and parents them to create a displayable collection.
        /// </summary>
        protected virtual void FillUI()
        {
            if (manuallyDefineCollection == false)
            {
                items = new ICollectionItem[initialCollectionSize];

                // Fill the container on startup, can add / remove later on
                for (uint i = 0; i < initialCollectionSize; i++)
                {
                    items[i] = CreateUIItem<ICollectionItem>(i, itemButtonPrefab != null ? itemButtonPrefab : InventorySettingsManager.instance.settings.itemButtonPrefab);
                    items[i].Repaint();
                }
            }
            else
            {
                IndexManuallyDefinedCollection();
            }
        }

        public virtual void IndexManuallyDefinedCollection()
        {
            items = container.GetComponentsInChildren<ICollectionItem>();
            for (uint i = 0; i < items.Length; i++)
            {
                items[i].itemCollection = this;
                items[i].index = i;
                items[i].Repaint();
            }
        }

        /// <summary>
        /// Create a single UI item (wrapper).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        protected T CreateUIItem<T>(uint i, GameObject prefab) where T : ICollectionItem
        {
            T item = Instantiate<GameObject>(prefab).GetComponent<T>();

            var c = item as UnityEngine.Component;
            if (c != null)
            {
                c.transform.SetParent(container);
                InventoryUtility.ResetTransform(c.transform);
            }

            item.itemCollection = this;
            item.index = i;
        
            return item;
        }


        #region Notifications from other objects

        /// <summary>
        /// Is notified when an item is dropped, this fires of the events.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemID">ID of the item dropped</param>
        /// <param name="amount">Amount of items dropped</param>
        public void NotifyItemDropped(InventoryItemBase item, uint itemID, uint amount, GameObject droppedObj)
        {
            _isDirty = true;

            // References just disappear, can't drop them
            if (useReferences)
            {
                if (OnRemovedReference != null)
                    OnRemovedReference(items[item.index].item, item.index);

                return;
            }
                
            if (OnDroppedItem != null)
                OnDroppedItem(item, item.index, droppedObj);

            SetItem(item.index, null, true);
            //items[item.index].Repaint();

            NotifyItemRemoved(item, itemID, item.index, amount);
            
            // Item is no longer in a collection, remove it.
            item.itemCollection = null;
            item.index = 0;
        }

        /// <summary>
        /// Is notified when a reference is used from this collection.
        /// </summary>
        /// <param name="actualItem"></param>
        /// <param name="itemID"></param>
        /// <param name="referenceSlot"></param>
        /// <param name="amountUsed"></param>
        public void NotifyReferenceUsed(InventoryItemBase actualItem, uint itemID, uint referenceSlot, uint amountUsed)
        {
            if (OnUsedReference != null)
                OnUsedReference(actualItem, itemID, referenceSlot, amountUsed);
        }

        public void NotifyResized(uint oldSize, uint newSize)
        {
            _isDirty = true;

            NotifyRequireLayoutRedraw();

            if (OnResized != null)
                OnResized(oldSize, newSize);
        }

        /// <summary>
        /// Handles events whenever an item is used.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="slot"></param>
        public void NotifyItemUsed(InventoryItemBase item, uint itemID, uint slot, uint amountUsed)
        {
            _isDirty = true;

            // Item removed itself or someone else did.
            if (items[slot].item == null)
            {
                //SetItem(slot, null);
                //NotifyItemRemoved(itemID, slot, 0); // Let the NotifyItemUsed() handle the update.
            }
            else
            {
                if (items[slot].item.currentStackSize == 0)
                {
                    SetItem(slot, null);
                    NotifyItemRemoved(item, itemID, slot, 0); // Let the NotifyItemUsed() handle the update.
                }
            }

            if (OnUsedItem != null)
                OnUsedItem(item, itemID, slot, amountUsed);


            if (item != null)
            {
                if (item.currentStackSize <= 0)
                {
                    SetItem(item.index, null);
                }
            }

            if(amountUsed > 0)
                items[slot].Repaint();
        }

        public void NotifySorted()
        {
            _isDirty = true;

            if (OnSorted != null)
            {
                OnSorted();
            }

            NotifyRequireLayoutRedraw();
        }

        public virtual void NotifyItemRemoved(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            _isDirty = true;

            if (OnRemovedItem != null)
            {
                OnRemovedItem(item, itemID, slot, amount);
            }

            NotifyRequireLayoutRedraw();
        }

        protected void NotifyRequireLayoutRedraw()
        {
            if(layoutGroup != null)
                layoutGroup.ForceRebuildNow();
        }

        public void NotifyItemAdded(InventoryItemBase item, uint amount, bool cameFromCollection)
        {
            NotifyItemAdded(new []{ item }, amount, cameFromCollection);
        }

        public virtual void NotifyItemAdded(IEnumerable<InventoryItemBase> item, uint amount, bool cameFromCollection)
        {
            _isDirty = true;

            if (OnAddedItem != null)
            {
                OnAddedItem(item, amount, cameFromCollection);
            }

            NotifyRequireLayoutRedraw();
        }

        private void NotifyCurrencyChanged(float before, CurrencyDecorator after)
        {
            //Debug.Log("Currency changed", transform);
            if (OnCurrencyChanged != null)
                OnCurrencyChanged(before, after); // Call the event on this collection.

        }

        public void NotifyItemSwapped(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot)
        {
            _isDirty = true;

            if (fromCollection.OnSwappedItems != null)
            {
                fromCollection.OnSwappedItems(fromCollection, fromSlot, toCollection, toSlot);
            }

            fromCollection.NotifyRequireLayoutRedraw();
            // Already fired what we needed, so return, else fire it on the other collection as well.
            if (fromCollection == toCollection)
                return;

            if (toCollection.OnSwappedItems != null)
            {
                toCollection.OnSwappedItems(fromCollection, fromSlot, toCollection, toSlot);
            }
            
            toCollection.NotifyRequireLayoutRedraw();        
        }

        [Obsolete("Use CanAddItem() instead.")]
        public void NotifyAddedItemCollectionFull(InventoryItemBase item, bool cameFromCollection)
        {
            Debug.LogWarning("NotifyAddedItemCollectionFull is Obsolete");
        }

        public void NotifyUnstackedItem(uint slot, ItemCollectionBase toCollection, uint toSlot, uint amount)
        {
            _isDirty = true;

            if (OnUnstackedItem != null)
            {
                OnUnstackedItem(this, slot, toCollection, toSlot, amount);
            }

            NotifyRequireLayoutRedraw();
            if(toCollection != this)
                toCollection.NotifyRequireLayoutRedraw();
        }

        private void NotifyMergedSlots(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot)
        {
            _isDirty = true;

            if (OnMergedSlots != null)
            {
                OnMergedSlots(fromCollection, fromSlot, toCollection, toSlot);
            }

            fromCollection.NotifyRequireLayoutRedraw();
            if (fromCollection != toCollection)
            {
                toCollection.NotifyRequireLayoutRedraw();
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Override the Use() method on all items inside this collection.
        /// Note that this method will be called even if canUseFromCollection is false.
        /// </summary>
        /// <returns>Return true if the useMethod() is overriden, false if not and the default item based action will be performed.</returns>
        public virtual bool OverrideUseMethod(InventoryItemBase item)
        {
            return false;
        }


        /// <summary>
        /// Resize to the new size
        /// </summary>
        /// <param name="newSize"></param>
        /// <param name="force"></param>
        /// <returns>True when resized, false if failed</returns>
        public virtual bool Resize(uint newSize, bool force = false)
        {
            uint oldSize = (uint)items.Length;
            if (oldSize == newSize)
                return true; // Nothing to resize

            if (newSize > oldSize)
            {
                // Add slots
                return AddSlots(newSize - oldSize);
            }
            else
            {
                // Remove slots
                return RemoveSlots(oldSize - newSize, force);
            }
        }


        /// <summary>
        /// Add extra slots to the collection.
        /// </summary>
        /// <returns>True if slots were added, false if not.</returns>
        public virtual bool AddSlots(uint amount, bool fireEvents = true)
        {
            DevdogLogger.LogVerbose("Adding " + amount + " slots to collection " + collectionName, transform);
            uint oldSize = (uint)items.Length;
            uint newSize = oldSize + amount;

            var currentItems = new ICollectionItem[newSize];
            for (uint i = 0; i < oldSize; i++)
            {
                currentItems[i] = items[i];
            }
            for (uint i = oldSize; i < newSize; i++)
            {
                currentItems[i] = CreateUIItem<ICollectionItem>(i, itemButtonPrefab != null ? itemButtonPrefab : InventorySettingsManager.instance.settings.itemButtonPrefab);
                currentItems[i].Repaint();
            }

            items = currentItems;

            if (fireEvents)
                NotifyResized(oldSize, newSize);

            return true;
        }

        public virtual bool CanRemoveSlots(uint amount)
        {
            RebuildCounterIfDirty();
            return _counter.CanRemoveSlots(_counter.FindLookup(this), amount);
        }

        /// <summary>
        /// Remove slots from the collection, slots are removed at the end, if however the slots are not empty the function will not do anything and return false.
        /// </summary>
        /// <returns>True if slots were removed, false if the slots were not empty and could not be removed.</returns>
        public virtual bool RemoveSlots(uint amount, bool force = false, bool fireEvents = true)
        {
            if (CanRemoveSlots(amount) == false && force == false)
                return false;

            DevdogLogger.LogVerbose("Removing " + amount + " slots from collection " + collectionName, transform);
            uint oldSize = (uint)items.Length;
            uint newSize = oldSize - amount;

            var currentItems = new ICollectionItem[newSize];
            for (uint i = 0; i < newSize; i++)
            {
                currentItems[i] = items[i];
            }

            // Remove the old
            for (uint i = newSize; i < oldSize; i++)
            {
                var c = items[i] as UnityEngine.Component;
                if (c != null)
                {
                    Destroy(c.gameObject);
                }
            }

            items = currentItems;

            if(fireEvents)
                NotifyResized(oldSize, newSize);

            return true;
        }

        /// <summary>
        /// Check if a given item type is allowed inside this collection
        /// </summary>
        /// <returns></returns>
        public virtual bool VerifyFilters(InventoryItemBase item)
        {
            return filters.IsItemAbidingFilters(item);
        }

        public virtual bool VerifyCustomConditionals(InventoryItemBase item)
        {
            foreach (var canAdd in canAddItemToCollectionConditionals)
            {
                if (canAdd(item) == false)
                    return false;
            }

            return true;
        }

        public void RebuildCounterIfDirty(bool force = false)
        {
            if ((_isDirty && disableCounterRebuild == false) || force )
            {
                _counter.LoadFrom(this);
                _isDirty = false;
            }
        }


        /// <summary>
        /// How many items can be stored in collection with itemID
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <returns></returns>
        public virtual uint CanAddItemCount(InventoryItemBase itemToAdd, uint earlyBailAmount)
        {
            RebuildCounterIfDirty();
            return _counter.CanAddItemCount(_counter.FindLookup(this), itemToAdd, earlyBailAmount);
        }

        /// <summary>
        /// Checks if an item can be placed inside the collection or not, checks if it's full, and checks item types.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CanAddItem(InventoryItemBase item)
        {
            RebuildCounterIfDirty();
            return _counter.CanAddItem(_counter.FindLookup(this), item);
        }


        public bool CanAddItems(InventoryItemBase[] items)
        {
            RebuildCounterIfDirty();

            var unAddedItems = _counter.TryAdd(items);

            _isDirty = true;
            return unAddedItems.Length == 0;
        }

        public bool CanAddItems(ItemAmountRow[] items)
        {
            RebuildCounterIfDirty();

            var unAdded = _counter.TryAdd(items);

            _isDirty = true;
            return unAdded.Length == 0;
        }

        public virtual bool AddItems(InventoryItemBase[] items,
            ICollection<InventoryItemBase> storedItems = null, bool repaint = true, bool fireEvents = true)
        {
            RebuildCounterIfDirty();
            if (CanAddItems(items) == false)
            {
                return false;
            }

            if (storedItems == null)
                storedItems = new List<InventoryItemBase>();

            foreach (var item in items)
            {
                bool added = AddItem(item, storedItems, repaint, fireEvents);
                Assert.IsTrue(added, "Couldn't add items even though check passed! Please report this bug + stack trace");
            }

            return true;
        }

        


        /// <summary>
        /// Add an item to the inventory, will handle stacking, placement and repainting.
        /// </summary>
        /// <param name="item">The item to store</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <param name="repaint">Should items be repainted? In most cases true will suit best.</param>
        /// <param name="fireEvents">Should events be fired such as OnAddedItem? Leave to true unless you know what you're doing... Don't say I didn't warn you...</param>
        /// <returns>Returns true if the item was placed, false if not.</returns>
        public virtual bool AddItem(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true, bool fireEvents = true)
        {
            if (item == null)
            {
                return false;
            }
            
            RebuildCounterIfDirty();
            using (new CollectionAvoidRebuildLock(this))
            {
                var currency = item as CurrencyInventoryItem;
                if (currency != null)
                {
                    return currency.PickupItem();
                }

                if (canPutItemsInCollection == false)
                {
                    DevdogLogger.LogVerbose("Tried to put item in collection, but setting - canPutItemsInCollection is false.", gameObject);
                    return false;
                }

                // Instantiate the object if it's not an instance object (A prefab for example...)
                if (item.IsInstanceObject() == false)
                    item = Instantiate<InventoryItemBase>(item); // Create an instance object            


                // Keeps track of the items stored in this cycle.
                // Because it's a local variable it ignored items that were already in the storedItems list and avoid sending data to events that wasn't supposed to be sent.
                var storedItemsSingle = new List<InventoryItemBase>(4);
                if (storedItems == null)
                    storedItems = new List<InventoryItemBase>(4); // For events and such

                uint placedCount = 0;
                bool cameFromCollection = item.itemCollection != null;

                bool placed = false;
                bool stacked = false;


                // First loop to try and merge into existing stacks
                if (canStackItemsInCollection)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].item == null)
                        {
                            continue;
                        }

                        if (items[i].item.Equals(item, false))
                        {
                            // Stack it
                            if (items[i].item.currentStackSize + item.currentStackSize <= items[i].item.maxStackSize)
                            {
                                // We can stack it
                                items[i].item.currentStackSize += item.currentStackSize;
                                placed = true;
                                stacked = true;
                                placedCount = item.currentStackSize;
                                storedItems.Add(items[i].item); // The stack we've added our items to
                                storedItemsSingle.Add(items[i].item);
                                break;
                            }

                            if (item.currentStackSize > item.maxStackSize)
                            {
                                // Add to a collection, but still need to store more, so no break;
                                uint added = item.maxStackSize - items[i].item.currentStackSize;
                                placedCount += added;
                                items[i].item.currentStackSize = item.maxStackSize; // Max out stack
                                item.currentStackSize -= added;
                                storedItems.Add(items[i].item); // The stack we've added our items to
                                storedItemsSingle.Add(items[i].item); // The stack we've added our items to
                            }
                        }
                    }
                }

                if (placed == false)
                {
                    // 2nd loop to check for empty slots.
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].item == null)
                        {
                            // Place in new slot
                            bool set = SetItem((uint)i, item);
                            if (set)
                            {
                                placed = true;
                                stacked = false;
                                placedCount = item.currentStackSize;
                                storedItems.Add(item);
                                storedItemsSingle.Add(item);
                                break; // All done
                            }
                        }
                    }
                }


                if (placed)
                {
                    _isDirty = true;
                    if (stacked)
                    {
                        // No longer need the object, items that are stacked are also dropped as stacks.
                        Destroy(item.gameObject);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                        if (moveObjectsToChildren)
                        {
                            item.transform.SetParent(containerItemsParent); // Keep things organized in the hierarchy
                        }
                    }

                    if (fireEvents)
                        NotifyItemAdded(storedItemsSingle, placedCount, cameFromCollection);

                    if (repaint)
                    {
                        foreach (var storedItem in storedItemsSingle)
                        {
                            storedItem.itemCollection[storedItem.index].Repaint();
                        }
                    }
                }
                else
                {
                    if(item.weight * item.currentStackSize + GetWeight() > restrictMaxWeight)
                    {
                        InventoryManager.langDatabase.collectionExceedingMaxWeight.Show(item.name, item.description, collectionName);
                        return false;
                    }

                    InventoryManager.langDatabase.collectionFull.Show(item.name, item.description, collectionName);
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Convenience function to add an item and remove it from it's original collection.
        /// If the object was not previously in a collection this will throw an error...
        /// </summary>
        /// <param name="item"></param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns>True if stored, false is not stored.</returns>
        public virtual bool AddItemAndRemove(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            var oldCollection = item.itemCollection;
            uint oldIndex = item.index;

            bool added = AddItem(item, storedItems, repaint);
            if (added)
            {
                oldCollection.SetItem(oldIndex, null);
                oldCollection.NotifyItemRemoved(item, item.ID, oldIndex, item.currentStackSize);
                oldCollection[oldIndex].Repaint();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove items first, then attempt to add them. The first remove could potentially clear up a slot for the adding.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="itemsToRemoveFirst"></param>
        /// <returns></returns>
        public bool RemoveItemsThenAdd(InventoryItemBase[] itemsToAdd, params ItemAmountRow[] itemsToRemoveFirst)
        {
            RebuildCounterIfDirty();

            _counter.TryRemoveItems(itemsToRemoveFirst);
            var unAdded = _counter.TryAdd(itemsToAdd);
            if (unAdded.Length > 0)
            {
                return false;
            }

            foreach (var item in itemsToRemoveFirst)
            {
                uint removed = RemoveItem(item.item.ID, item.amount);
                Assert.IsTrue(removed == item.amount);
            }

            bool added = AddItems(itemsToAdd);
            Assert.IsTrue(added);

            return true;
        }

        /// <summary>
        /// Remove items from this collection.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="amountToRemove"></param>
        /// <returns>The amount of item that were removed, note that when there are 4 items in this collection and you try to remove 10, 4 will be removed and the value 4 (amount of items removed) is returned.</returns>
        public virtual uint RemoveItem(uint itemID, uint amountToRemove)
        {
            uint removed = 0;
//            var items = FindAll(itemID);
            foreach (var item in GetStacksSmallestToLargest(itemID))
            {
                Assert.IsNotNull(item.itemCollection, "Item collection not set on item that you tried to remove.");

                if (removed + item.currentStackSize <= amountToRemove)
                {
                    // Take some of the stack or all if it's available

                    //item.itemCollection.SetItem(item.index, null);
                    item.itemCollection.SetItem(item.index, null);
//                    item.itemCollection[item.index].item = null;
                    item.itemCollection.NotifyItemRemoved(item, item.ID, item.index, (uint)item.currentStackSize);
                    item.itemCollection[item.index].Repaint();

                    Destroy(item.gameObject); // Item is no longer needed
                    removed += item.currentStackSize;
                }
                else if (removed < amountToRemove)
                {
                    // Remove that's left
                    // Going over, take just a few of the stack
                    uint toRemove = amountToRemove - removed;
                    item.currentStackSize -= toRemove;
                    item.itemCollection[item.index].Repaint();
                    removed += toRemove;
                    break; // We're done our stack is complete
                }
            }

            return removed;
        }

        private IEnumerable<InventoryItemBase> GetStacksSmallestToLargest(uint itemID)
        {
            return items.Select(o => o.item).Where(o => o != null && o.ID == itemID).OrderBy(o => o.currentStackSize);
        }


        public virtual bool RemoveItem(InventoryItemBase item)
        {
            Assert.IsTrue(item.itemCollection == this, "Trying to remove specific item from collection, but the item isn't in this collection.");

            SetItem(item.index, null);
//            items[item.index].item = null;
            NotifyItemRemoved(item, item.ID, item.index, item.currentStackSize);
            items[item.index].Repaint();
            return true;
        }


        #region Currencies

        public CurrencyDecorator GetCurrency(CurrencyDefinition def)
        {
            return GetCurrencyByID(def.ID);
        }

        public CurrencyDecorator GetCurrencyByID(uint currencyID)
        {
            return currenciesGroup.lookups.FirstOrDefault(o => o.currency.ID == currencyID);
        }

        public float CanAddCurrencyCount(CurrencyDefinition def)
        {
            return CanAddCurrencyCount(def.ID);
        }

        public float CanAddCurrencyCount(uint currencyID)
        {
            if (canContainCurrencies == false)
                return 0f;

            float total = 0.0f;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                total += currency.CanAddCount();
            }

            return total;
        }

        public float CanRemoveCurrencyCount(CurrencyDefinition def)
        {
            return CanRemoveCurrencyCount(def.ID);
        }

        public float CanRemoveCurrencyCount(uint currencyID)
        {
            return CanRemoveCurrencyCount(currencyID, true);
        }

        public float CanRemoveCurrencyCount(CurrencyDefinition def, bool allowCurrencyConversions)
        {
            return CanRemoveCurrencyCount(def.ID, allowCurrencyConversions);
        }

        /// <summary>
        /// The total amount of currency that can be removed.
        /// </summary>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversions">When true all possible conversions will be considered. For example 1 gold is worth 100 silver. If you try to remove 101 silver, gold will be converted down to silver.</param>
        /// <returns></returns>
        public float CanRemoveCurrencyCount(uint currencyID, bool allowCurrencyConversions)
        {
            float total = 0.0f;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
                total += currency.CanRemoveCount(allowCurrencyConversions);

            return total;
        }

        public bool CanRemoveCurrency(uint currencyID, float amount, bool allowCurrencyConversions)
        {
            if (canContainCurrencies == false)
                return false;

            return CanRemoveCurrencyCount(currencyID, allowCurrencyConversions) >= amount;
        }

        public bool CanAddCurrency(uint currencyID, float amount)
        {
            return CanAddCurrencyCount(currencyID) >= amount;
        }

        public bool AddCurrency(CurrencyDecorator dec)
        {
            if (dec.amount <= 0f)
            {
                return true;
            }

            return AddCurrency(dec.currency, dec.amount);
        }

        public bool AddCurrency(CurrencyDefinition currency, float amount)
        {
            return AddCurrency(currency.ID, amount);
        }

        /// <summary>
        /// Add currency to this collection.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="currencyID">The currencyID (type) of currency to add.</param>
        /// <param name="amount">The amount to add</param>
        /// <returns></returns>
        public bool AddCurrency(uint currencyID, float amount)
        {
            Assert.IsTrue(amount >= 0f, "Can't add negative values, use RemoveCurrency instead.");
            if (CanAddCurrency(currencyID, amount) == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount += amount;
                return true;
            }

            DevdogLogger.LogVerbose("Can't add that currency to this colleciton...");
            return false;
        }

        public bool RemoveCurrency(CurrencyDecorator dec, bool allowCurrencyConversions)
        {
            if (dec.amount <= 0f)
            {
                return true;
            }

            return RemoveCurrency(dec.currency, dec.amount, allowCurrencyConversions);
        }

        public bool RemoveCurrency(CurrencyDefinition def, float amount, bool allowCurrencyConversions)
        {
            return RemoveCurrency(def.ID, amount, allowCurrencyConversions);
        }

        public bool RemoveCurrency(uint currencyID, float amount, bool allowCurrencyConversions)
        {
            Assert.IsTrue(amount >= 0f, "Can't remove negative values, use AddCurrency instead.");
            if (CanRemoveCurrency(currencyID, amount, allowCurrencyConversions) == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount -= amount;
                return true;
            }

            DevdogLogger.LogVerbose("Can't remove that currency from this colleciton...", this);
            return false;
        }

        public bool SetCurrencyAmount(CurrencyDefinition def, float amount)
        {
            return SetCurrencyAmount(def.ID, amount);
        }

        public bool SetCurrencyAmount(uint currencyID, float amount)
        {
            if (canContainCurrencies == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount = amount;
                return true;
            }

            return false;
        }


        #endregion


        /// <summary>
        /// Finds the first empty slot in this collection
        /// </summary>
        /// <returns>The index of the first empty slot, and -1 if no slot found (when collection is full)</returns>
        public virtual int FindFirstEmptySlot()
        {
            return FindFirstEmptySlot(null);
        }

        /// <summary>
        /// Finds the first empty slot in this collection for a specific item
        /// </summary>
        /// <returns>The index of the first empty slot, and -1 if no slot found (when collection is full)</returns>
        public virtual int FindFirstEmptySlot(InventoryItemBase item)
        {
            RebuildCounterIfDirty();
            return _counter.FindFirstEmptySlotForItem(_counter.FindLookup(this), item);
        }

        /// <summary>
        /// Unstack an item slot to the first empty slot
        /// Unstacks in the current collection
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        /// <param name="repaint"></param>
        /// <returns>Returns true if unstacked, false if failed</returns>
        public virtual bool UnstackSlot(uint slot, uint amount, bool repaint = true)
        {
            // References can never be unstacked / stacked.
            if (useReferences || canUnstackItemsInCollection == false)
                return false;

            int firstEmptySlot = FindFirstEmptySlot(items[slot].item);
            if (firstEmptySlot != -1)
            {
                return UnstackSlot(slot, this, (uint) firstEmptySlot, amount, repaint);
            }

            return false;
        }


        /// <summary>
        /// Unstack an item slot to a given slot
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        /// <param name="amount"></param>
        /// <param name="repaint"></param>
        /// <returns>Returns true if unstacked, false if failed</returns>
        public virtual bool UnstackSlot(uint slot, ItemCollectionBase toCollection, uint toSlot, uint amount, bool repaint = true)
        {
            // References can never be unstacked / stacked.
            if (useReferences || canUnstackItemsInCollection == false)
                return false;

            if (toCollection[toSlot].item != null)
            {
                if (toCollection[toSlot].item.ID != items[slot].item.ID)
                    return false; // Different item

                if (toCollection[toSlot].item.currentStackSize + amount > toCollection[toSlot].item.maxStackSize)
                {
                    DevdogLogger.LogVerbose("Trying to unstack into a filled slot, but stack amount would override max stack size.");
                    return false;                    
                }
            }

            if (items[slot].item == null || amount > items[slot].item.currentStackSize)
                return false; // Trying to remove to many items, or trying to remove from a stack that doesn't exist...

            // Remove from old stack
            items[slot].item.currentStackSize -= amount;
            bool itemSet = false;

            if (toCollection[toSlot].item != null)
            {
                // Unstack onto new slot
                toCollection[toSlot].item.currentStackSize += amount;
                itemSet = true;
            }
            else
            {
                // Unstack onto empty slot
                var copy = Instantiate<InventoryItemBase>(items[slot].item);
                copy.currentStackSize = (uint)amount;

                itemSet = toCollection.SetItem(toSlot, copy);
                if (itemSet)
                {
                    copy.gameObject.SetActive(false);
                    if (moveObjectsToChildren)
                    {
                        copy.transform.SetParent(containerItemsParent); // Keep things organized in the hierarchy
                    }
                }
                else
                {
                    Destroy(copy.gameObject); // Couldn't set, no longer need it
                    return false;
                }
            }

            if (itemSet)
            {
                // Notify collection
                NotifyUnstackedItem(slot, toCollection, toSlot, amount);

                // Notify item
                items[slot].item.NotifyItemUnstacked(toSlot, amount);

                if (repaint)
                {
                    items[slot].Repaint();
                    toCollection[toSlot].Repaint();
                }

                return true; // All done                
            }

            return false;
        }


        /// <summary>
        /// Swap 2 slots, useful for re-aranging elements in the UI.
        /// </summary>
        protected virtual bool SwapSlots(uint fromSlot, ItemCollectionBase toCollection, uint toSlot, bool repaint = true, bool fireEvents = true)
        {
            RebuildCounterIfDirty();
            using (new CollectionAvoidRebuildLock(this))
            {
                ItemCollectionBase fromCollection = this;

                var fromItem = fromCollection[fromSlot].item;
//                var toItem = toCollection[toSlot].item;
                Assert.IsNotNull(fromItem, "Can't be empty!");

                var fromLookup = fromCollection.counter.FindLookup(fromCollection);
                if (fromLookup.collection.Length <= fromSlot)
                    return false;

                fromCollection.counter.RemoveSlot(fromLookup, fromSlot);

                var toLookup = toCollection.counter.FindLookup(toCollection);
                if (toLookup != null)
                {
                    if (toLookup.collection.Length <= toSlot)
                        return false;

                    toCollection.counter.RemoveSlot(toLookup, toSlot);
                }

                _isDirty = true;
                if (fromCollection.CanSetItem(fromSlot, toCollection[toSlot].item) == false ||
                    toCollection.CanSetItem(toSlot, fromCollection[fromSlot].item) == false)
                {
                    return false;
                }

                if (this == toCollection)
                {
                    // Moving to same collection

                    var currentItemPosCol = (int)(toSlot % colsCount);
                    var currentItemPosRow = Mathf.FloorToInt((float)toSlot / colsCount);

                    var toAvoidCol = (int)(fromSlot % colsCount);
                    var toAvoidRow = Mathf.FloorToInt((float)fromSlot / colsCount);

                    var item = items[fromSlot].item;

                    var r = items[toSlot].item;
                    if (r != null)
                    {
                        // AABB
                        if (fromSlot == toSlot || // Allow to collide with self.
                            currentItemPosCol + item.layoutSizeCols - 1 < toAvoidCol ||
                            currentItemPosCol > toAvoidCol + r.layoutSizeCols - 1 ||
                            currentItemPosRow + item.layoutSizeRows - 1 < toAvoidRow ||
                            currentItemPosRow > toAvoidRow + r.layoutSizeRows - 1)
                        {
                            // AABB test succeeded.
                            //                        Debug.Log("No collision");
                        }
                        else
                        {
                            // Debug.Log("Collision, avoid ..");
                            return false;
                        }
                    }
                }

                var tempTo = toCollection.items[toSlot].item;
                uint fromTempAmount = fromCollection[fromSlot].item != null ? fromCollection[fromSlot].item.currentStackSize : 0; // TODO: fromCollection item shouldn't be empty!
                                                                                                                                  //uint toTempAmount = toCollection[toSlot].item.currentStackSize;

                // Move to first item to toCollection
                bool moved = fromCollection.MoveItem(fromCollection[fromSlot].item, fromSlot, toCollection, toSlot, true, false); // Moves the item into toCollection[toSlot]
                if (moved == false)
                {
                    return false;
                }

                // Moving to another collection, so we're actually removing one in this collection
                if (fromCollection != toCollection)
                {
                    if (toCollection.useReferences == false && fromCollection.useReferences == false && fireEvents)
                    {
                        // if tempTo == null then the slot we're moving to didn't have an object before, so then one won't be removed.
                        if (tempTo != null)
                            toCollection.NotifyItemRemoved(tempTo, tempTo.ID, toSlot, tempTo.currentStackSize);

                        if (toCollection[toSlot].item != null)
                            toCollection.NotifyItemAdded(toCollection[toSlot].item, fromTempAmount, true);
                    }
                }

                // And move the temp item to fromCollection, and it's swapped :)
                if ((toCollection.useReferences == false && fromCollection.useReferences == false) ||
                    (toCollection.useReferences && fromCollection.useReferences))
                {
                    bool moved2 = toCollection.MoveItem(tempTo, toSlot, fromCollection, fromSlot, false, false);
                    if (moved2 == false)
                    {
                        DevdogLogger.LogWarning("Couldn't move 2nd object.");
                    }
                }

                // Moving to another collection, so we're actually removing one in this collection
                if (fromCollection != toCollection)
                {
                    if (fromCollection.useReferences == false && toCollection.useReferences == false && fireEvents)
                    {
                        if (toCollection[toSlot].item != null)
                            fromCollection.NotifyItemRemoved(toCollection[toSlot].item, toCollection[toSlot].item.ID, fromSlot, toCollection[toSlot].item.currentStackSize);

                        // if temp == null then the slot we're moving to didn't have an object before, so then one won't be removed.
                        if (tempTo != null)
                            fromCollection.NotifyItemAdded(fromCollection[fromSlot].item, fromCollection[fromSlot].item.currentStackSize, true);
                    }
                }

                //Debug.Log("Swapped slots " + fromSlot + " in " + fromCollection.ToString() + " with " + toSlot + " in " + toCollection.ToString());

                if (repaint)
                {
                    fromCollection.items[fromSlot].Repaint();
                    toCollection.items[toSlot].Repaint();
                }

                if (fireEvents && (fromCollection.useReferences == false && toCollection.useReferences == false))
                    NotifyItemSwapped(fromCollection, fromSlot, toCollection, toSlot);

            }

            return true;
        }

        /// <summary>
        /// Determines whether this 2 slots can be merged / stacked together.
        /// </summary>
        /// <returns><c>true</c> if this instance can merge the specified slot1 slot2; otherwise, <c>false</c>.</returns>
        /// <param name="slot1">Index 1.</param>
        /// <param name="slot2">Index 2.</param>
        public virtual bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            ItemCollectionBase collection1 = this;

            if (canStackItemsInCollection == false)
                return false;

            // If slots are empty they cannot be merged.
            if (collection1.items[slot1].item == null || collection2.items[slot2].item == null)
                return false;

            if (collection2.canPutItemsInCollection == false)
                return false;

            // References cannot be merged.
            if (collection1.useReferences || collection2.useReferences)
                return false;

            // Same item ?
            if (collection1.items[slot1].item.Equals(collection2.items[slot2].item, false))
            {
                return collection1.items[slot1].item.currentStackSize + collection2.items[slot2].item.currentStackSize <= collection1.items[slot1].item.maxStackSize; // Does it fit?
            }

            return false;
        }

        /// <summary>
        /// Merge the specified slot1 and slot2.
        /// Slot 1 will be the new slot -> Slot 2 is moved to slot 1, and slot 2 is cleared.
        /// </summary>
        /// <param name="slot1">Index 1.</param>
        /// <param name="slot2">Index 2.</param>
        protected virtual bool MergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2, bool repaint = true, bool fireEvents = true)
        {
            ItemCollectionBase collection1 = this;

            if (CanMergeSlots(slot1, collection2, slot2))
            {
                uint tempStackSize = collection1.items[slot1].item.currentStackSize;
                var tempItemCollection1 = collection1.items[slot1].item;
                collection2.items[slot2].item.currentStackSize += collection1.items[slot1].item.currentStackSize;
//                collection1[slot1].item = null;
                collection1.SetItem(slot1, null);

                // Moving to a different collection
                if(collection1 != collection2)
                {
                    collection2.NotifyItemAdded(collection2[slot2].item, tempStackSize, true); // Items are moved to collection2 so it gains the stack of the other
                    collection1.NotifyItemRemoved(tempItemCollection1, tempItemCollection1.ID, slot1, tempStackSize); // Item in collection1 is moved to collection2, so it loses an object.

                    DevdogLogger.LogVerbose("Merged an item in another collection " + collection2.ToString() + " gained " + tempStackSize + " items of ID: " + collection2[slot2].item.ID + " and removed " + tempStackSize + " of " + collection1.ToString() + " with item ID: " + collection2[slot2].item.ID + 
                              "\nThere are now " + collection2.GetItemCount(collection2[slot2].item.ID) + " items in " + collection2.ToString() + " and " + collection1.GetItemCount(collection2[slot2].item.ID) + " in " + collection1.ToString());
                }

                if (fireEvents)
                    NotifyMergedSlots(collection1, slot1, collection2, slot2);

                if (repaint)
                {
                    collection2.items[slot2].Repaint();
                    collection1.items[slot1].Repaint();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Merges the slots if possible, if not it will swap them out.
        /// </summary>
        /// <param name="slot1">Slot1.</param>
        /// <param name="slot2">Slot2.</param>
        /// <returns>True if the item was swapped or merged / False if the action could not be completed.</returns>
        public virtual bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            ItemCollectionBase handler1 = this;

            // Not actually moving anything...
            if (handler1 == handler2 && slot1 == slot2)
                return false;

            if (handler2.canPutItemsInCollection == false)
                return false;

            if (CanMergeSlots(slot1, handler2, slot2))
                return MergeSlots(slot1, handler2, slot2, repaint);
            else
                return SwapSlots(slot1, handler2, slot2, repaint);
        }

        /// <summary>
        /// Can an item be placed in a slot, does type checking auto.
        /// </summary>
        /// <param name="slot">The slot we're trying to set</param>
        /// <param name="item">The item we're trying to set</param>
        /// <returns></returns>
        public virtual bool CanSetItem(uint slot, InventoryItemBase item)
        {
            RebuildCounterIfDirty();
            return _counter.CanSetItem(slot, item, _counter.FindLookup(this));
        }

        /// <summary>
        /// <b>DOES NOT FIRE ADD / REMOVE EVENTS!</b>
        /// This function can be overridden to add custom behavior whenever an object is placed in the inventory.
        /// This happens when 2 items are swapped, items are merged, anytime an object is put in a slot.
        /// <b>Does not handle repainting</b>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="item"></param>
        /// <returns>Returns true if the item was set, false if not.</returns>
        public virtual bool SetItem(uint slot, InventoryItemBase item)
        {
            if (CanSetItem(slot, item) == false)
                return false;

            items[slot].item = item;
			items[slot].itemCollection = this;

            if (OnSetItem != null)
                OnSetItem(slot, item);

            return true;
        }

        public bool SetItem(uint slot, InventoryItemBase item, bool repaint)
        {
            bool s = SetItem(slot, item);
            if(repaint)
                items[slot].Repaint();
                
            return s;
        }


        /// <summary>
        /// Set an array of items, fails if the collection is smaller than the amount of items setting.
        /// Note: This removed all items from the collection and replaces it with the given array.
        /// </summary>
        /// <param name="toSet">Items to set, the remainder will be set to null</param>
        /// <param name="setParent">Move the transforms to the new position</param>
        public virtual void SetItems(InventoryItemBase[] toSet, bool setParent, bool repaint = true)
        {
            Resize((uint)toSet.Length);

            for (uint i = 0; i < toSet.Length; i++)
            {
                if (items[i].item != null)
                    NotifyItemRemoved(items[i].item, items[i].item.ID, items[i].item.index, items[i].item.currentStackSize);

                SetItem(i, toSet[i], repaint);
                if (items[i].item != null)
                {
                    bool cameFromCollection = items[i].itemCollection != null;
                    items[i].itemCollection = this;

                    NotifyItemAdded(items[i].item, items[i].item.currentStackSize, cameFromCollection);

                    if (setParent)
                    {
                        if (moveObjectsToChildren)
                        {
                            items[i].item.transform.SetParent(containerItemsParent);
                        }
                        items[i].item.gameObject.SetActive(false);
                    }
                }
            }

            for (int i = toSet.Length; i < items.Length; i++)
            {
                if (items[i].item != null)
                    NotifyItemRemoved(items[i].item, items[i].item.ID, items[i].item.index, items[i].item.currentStackSize);

                SetItem((uint)i, null, repaint);
            }

            _isDirty = true;
        }

        /// <summary>
        /// Item usabilities added to all items in this collection, none by default.
        /// </summary>
        public virtual IList<ItemUsability> GetExtraItemUsabilities(IList<ItemUsability> basic)
        {
            return basic;
        }

        /// <summary>
        /// Sorts the inventory and handles the required repaint.
        /// To change sorting behavior implement the IInventorySorter.
        /// </summary>
        public virtual void SortCollection()
        {
            var list = new List<InventoryItemBase>(items.Length);
            for (uint i = 0; i < items.Length; i++)
            {
                if (items[i].item != null)
                {
                    list.Add(items[i].item);
                    items[i].item = null; // No SetItem() to avoid events
                }
            }

            var tempItems = InventorySettingsManager.instance.settings.collectionSorter.Sort(list);

            // Add all items (they're already cleared)
            foreach (var item in tempItems)
            {
                bool added = AddItem(item, null, false, false);
                Assert.IsTrue(added, "Coudln't re-add item!");
            }

            // Repaint all UI elements
            foreach (var item in items)
            {
                item.Repaint();
            }

            NotifySorted();
            DevdogLogger.LogVerbose("Sorting collection: " + this.ToString());
        }

        /// <summary>
        /// Places an object in a slot and handles repaint, this method can easilly be overwritten for extra functinoality when placing an item.
        /// </summary>
        /// <param name="fromSlot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        /// <param name="doRepaint"></param>
        public virtual bool MoveItem(InventoryItemBase item, uint fromSlot, ItemCollectionBase toCollection, uint toSlot, bool clearOld, bool doRepaint = true)
        {
            // Not actually moving anything.
            if (this == toCollection && fromSlot == toSlot)
                return false;

            if (toCollection.CanSetItem(toSlot, item) == false)
                return false;

            // Only remove the item if we're not dealing with references.
            // If we're not using reference move the item from 1 place to the next.
            if (useReferences && toCollection.useReferences == false)
            {
                return SetItem(fromSlot, null, doRepaint);
            }

            // Copy to reference, without clearing it.
            if(useReferences == false && toCollection.useReferences)
            {
                bool set = toCollection.SetItem(toSlot, item, doRepaint);
                if (set == false)
                    return false;

                //toCollection.NotifyAddedReference(toSlot, item);
                return true;
            }

            // Move the reference, not copying.
            if (useReferences && toCollection.useReferences)
            {
                toCollection.SetItem(toSlot, item, doRepaint);
                //NotifyRemovedReference(fromSlot, item);

                return true;
            }

            // Just simply move it, all that's left
            bool set2 = toCollection.SetItem(toSlot, item, doRepaint);
            if (set2 == false)
            {
                return false;
            }

            if (clearOld)
            {
                return SetItem(fromSlot, null, doRepaint);
            }
                
            return true;
        }

        /// <summary>
        /// Clears the entire collection.
        /// </summary>
        public void Clear(bool fireEvents = true)
        {
            for (uint i = 0; i < items.Length; i++)
            {
                var item = items[i].item;

                if (item != null)
                {
                    SetItem(i, null);
//                    items[i].item = null;

                    if (fireEvents)
                    {
                        NotifyItemRemoved(item, item.ID, i, item.currentStackSize);
                    }
                    items[i].Repaint();
                }
            }

            UnRegisterCurrencyEvents();
            currenciesGroup = new CurrencyDecoratorCollection(true);

            if (fireEvents)
            {
                foreach (var c in currenciesGroup.lookups)
                {
                    NotifyCurrencyChanged(0f, c);
                }
            }

            RegisterCurrencyEvents();
        }


        /// <summary>
        /// Find returns the first item in the collection that matches the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The first object / stack that matches the ID. Returns null if the object is not found.</returns>
        public InventoryItemBase Find(uint id)
        {
            return FindAll(id).FirstOrDefault();
        }

        /// <summary>
        /// Find returns an array of all items that match the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An array of all the items that match the ID.Returns empty array if no objects are found.</returns>
        public InventoryItemBase[] FindAll(uint id)
        {
            var list = new List<InventoryItemBase>(items.Length);
            foreach (var item in items)
            {
                if (item.item != null && item.item.ID == id)
                {
                    list.Add(item.item);
                }
            }

            return list.ToArray();
        }

        public InventoryItemBase FindLike(InventoryItemBase itemsLikeThis)
        {
            return FindAllLike(itemsLikeThis).FirstOrDefault();
        }

        public InventoryItemBase[] FindAllLike(InventoryItemBase itemsLikeThis)
        {
            var list = new List<InventoryItemBase>(4);
            foreach (var item in items)
            {
                if (item.item != null && item.item.Equals(itemsLikeThis, false))
                {
                    list.Add(item.item);
                }
            }

            return list.ToArray();
        }


        public InventoryItemBase FindByCategory(ItemCategory category)
        {
            return FindAllByCategory(category).FirstOrDefault();
        }

        public InventoryItemBase[] FindAllByCategory(ItemCategory category)
        {
            var list = new List<InventoryItemBase>(items.Length);
            foreach (var item in items)
            {
                if (item.item != null && item.item.category == category)
                    list.Add(item.item);
            }

            return list.ToArray();
        }

        public bool CanRemoveItems(IList<InventoryItemBase> items)
        {
            return CanRemoveItems(InventoryItemUtility.ItemsToRows(items));
        }

        public bool CanRemoveItems(IList<ItemAmountRow> items)
        {
            RebuildCounterIfDirty();
            return _counter.TryRemoveItems(items);
        }

        public uint GetItemCountLike(InventoryItemBase item)
        {
            return (uint)FindAllLike(item).Sum(i => i.currentStackSize);
        }

        /// <summary>
        /// Counting how many items there are with the given ID.
        /// Lookup is O(N) (Linear performance)
        /// </summary>
        /// <param name="id">item.ID</param>
        /// <returns>The amount of items in the collection or a given ID.</returns>
        public uint GetItemCount(uint id)
        {
            return (uint)FindAll(id).Sum(i => i.currentStackSize);
        }

        /// <summary>
        /// Get the amount of empty slots in the collection
        /// </summary>
        /// <returns>Amount of empty slots</returns>
        public uint GetEmptySlotsCount()
        {
            RebuildCounterIfDirty();
            return _counter.GetEmptySlotCount();
        }

        /// <summary>
        /// Get the total weight of all items inside this collection
        /// </summary>
        /// <returns></returns>
        public virtual float GetWeight()
        {
            return items.Sum(o => o.item == null ? 0.0f : o.item.weight * o.item.currentStackSize);
        }

        #endregion


        public static ItemCollectionBase FindByName(string collectionName)
        {
            var resources = Resources.FindObjectsOfTypeAll<ItemCollectionBase>();

#if UNITY_EDITOR

            // Filter out assets, as resources returns this.
            resources = resources.Where(o => UnityEditor.EditorUtility.IsPersistent(o) == false).ToArray();

#endif
            return resources.FirstOrDefault(o => o.collectionName == collectionName);
        }

        public static T FindByName<T>(string collectionName) where T : ItemCollectionBase
        {
            return (T)FindByName(collectionName);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public IEnumerator<ICollectionItem> GetEnumerator()
        {
            return ((IEnumerable<ICollectionItem>) items).GetEnumerator();
        }

        public override string ToString()
        {
            return collectionName;
        }
    }
}