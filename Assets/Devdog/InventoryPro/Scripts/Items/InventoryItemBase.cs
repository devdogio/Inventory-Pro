using UnityEngine;
using System.Collections.Generic;
using System;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine.Assertions;
using Devdog.General.ThirdParty.UniLinq;

namespace Devdog.InventoryPro
{

    /// <summary>
    /// The base item of all the inventory items, contains some default behaviour for items, which can (almost) all be overriden.
    /// </summary>
//    [DisallowMultipleComponent]
    public partial class InventoryItemBase : MonoBehaviour, IEquatable<InventoryItemBase>
    {
        
        #region Item data

        public uint index { get; set; }
        public ItemCollectionBase itemCollection { get; set; }


        [SerializeField]
        [HideInInspector]
        private uint _id;
        /// <summary>
        /// Unique ID of the object
        /// </summary>
        public uint ID {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        [SerializeField]
        private string _name = "";
        /// <summary>
        /// Name of the object (does not have to be unique)
        /// <example>
        /// Access as item.name
        /// </example>
        /// </summary>
        public new virtual string name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        [SerializeField]
        private string _description = "";
        /// <summary>
        /// Description of the object.
        /// </summary>
        public virtual string description
        {
            get {
                return _description;
            }
            set {
                _description = value;
            }
        }

        [SerializeField]
        [Required]
        private ItemCategory _category;
        public ItemCategory category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string categoryName
        {
            get
            {
                if (category != null)
                {
                    return category.name;
                }

                return string.Empty;
            }
        }


        /// <summary>
        /// Use the cooldown from the category? If true the global cooldown will be used, if false a unique cooldown can be set.
        /// </summary>
        [SerializeField]
        private bool _useCategoryCooldown = true;
        public bool useCategoryCooldown
        {
            get
            {
                return _useCategoryCooldown;
            }
            set
            {
                _useCategoryCooldown = value;
            }
        }

        [SerializeField]
        private GameObject _overrideDropObjectPrefab;
        public GameObject overrideDropObjectPrefab
        {
            get { return _overrideDropObjectPrefab; }
            set { _overrideDropObjectPrefab = value; }
        }


        [SerializeField]
        private Sprite _icon;
        /// <summary>
        /// The icon shown in the UI.
        /// </summary>
        public Sprite icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
            }
        }


        [SerializeField]
        [Range(1, 4)]
        private uint _layoutSizeCols = 1;
        public uint layoutSizeCols
        {
            get { return (uint)Mathf.Max(1, _layoutSizeCols); }
            set { _layoutSizeCols = value; }
        }

        [SerializeField]
        [Range(1, 4)]
        private uint _layoutSizeRows = 1;
        public uint layoutSizeRows
        {
            get { return (uint)Mathf.Max(1, _layoutSizeRows); }
            set { _layoutSizeRows = value; }
        }

        public uint layoutSize
        {
            get { return layoutSizeRows * layoutSizeCols; }
        }


        [SerializeField]
        [Range(0.0f, 999.0f)]
        private float _weight;
        /// <summary>
        /// The weight of the object, KG / LBS / Stone whatever you want, as long as every object uses the same units.
        /// </summary>
        public float weight {
            get {
                return _weight;
            }
            set {
                _weight = value;
            }
        }

        [SerializeField]
        [Range(0, 100)]
        private uint _requiredLevel;
        /// <summary>
        /// The minimal required level to use this item. This is not used directly by Inventory Pro but can be used by 3rd party assets / custom code.
        /// </summary>
        public uint requiredLevel
        {
            get
            {
                return _requiredLevel;
            }
            set
            {
                _requiredLevel = value;
            }
        }

        [SerializeField]
        [Required]
        private ItemRarity _rarity;
        public ItemRarity rarity
        {
            get { return _rarity; }
            set { _rarity = value;  }
        }


        [SerializeField]
        private StatDecorator[] _stats = new StatDecorator[0];
        /// <summary>
        /// Item properties, to define your own custom data on items.
        /// If you have a property that repeats itself all the time consider making an itemType (check documentation)
        /// </summary>
        public StatDecorator[] stats
        {   
            get
            {
                return _stats;
            }
            set
            {
                _stats = value;
            }
        }


        [SerializeField]
        private StatRequirement[] _usageRequirement;

        /// <summary>
        /// How much of a specific stat (property) does the user need to have in order to use this item?
        /// Example: requirement of 10 strength > The item can only be used if the player has 10 or more strength.
        /// </summary>
        public StatRequirement[] usageRequirement
        {
            get { return _usageRequirement; }
            set { _usageRequirement = value; }
        }
        

        [SerializeField]
        private CurrencyDecorator _buyPrice;
        public CurrencyDecorator buyPrice {
            get {
                return _buyPrice;
            }
            set {
                _buyPrice = value;
            }
        }

        [SerializeField]
        private CurrencyDecorator _sellPrice;
        public CurrencyDecorator sellPrice {
            get {
                return _sellPrice;
            }
            set {
                _sellPrice = value;
            }
        }
	
        [SerializeField]
        private bool _isDroppable = true;
        /// <summary>
        /// Can the item be dropped?
        /// </summary>
        public bool isDroppable {
            get {
                return _isDroppable;
            }
            set {
                _isDroppable = value;
            }
        }

        [SerializeField]
        private bool _isSellable = true;
        /// <summary>
        /// Can the item be sold?
        /// </summary>
        public bool isSellable {
            get {
                return _isSellable;
            }
            set {
                _isSellable = value;
            }
        }

        [SerializeField]
        private bool _isStorable = true;
        /// <summary>
        /// Can the item be stored in a bank / or crate / etc.
        /// </summary>
        public bool isStorable {
            get {
                return _isStorable;
            }
            set {
                _isStorable = value;
            }
        }

        [SerializeField]
        [Range(1,999)]
        private uint _maxStackSize = 1;
        /// <summary>
        /// How many items fit in 1 pile / stack
        /// </summary>
        public uint maxStackSize {
            get {
                return _maxStackSize;
            }
            set {
                _maxStackSize = value;
            }
        }

        [NonSerialized]
        private uint _currentStackSize = 1;
        /// <summary>
        /// The current amount of items in this stack
        /// </summary>
        public virtual uint currentStackSize
        {
            get
            {
                return _currentStackSize;
            }
            set
            {
                _currentStackSize = value;
            }
        }


        [SerializeField]
        private float _cooldownTime = 0.0f;
        /// <summary>
        /// The time an item is unusable for once it's used.
        /// </summary>
        public virtual float cooldownTime {
            get
            {
                if (useCategoryCooldown)
                {
                    return category.cooldownTime;
                }

                return _cooldownTime;
            } 
            protected set
            {
                _cooldownTime = value;
            }
        }

        private static Dictionary<uint, float> _lastUsageTimeLookup = new Dictionary<uint, float>();
        /// <summary>
        /// Used to calculate if the cooldown is over. ((lastUsageTime + cooldown) > Time.TimeSinceStarted).
        /// Only used when useCategoryCooldown is false.
        /// </summary>
        public virtual float lastUsageTime
        {
            get
            {
                if (useCategoryCooldown)
                {
                    return category.lastUsageTime;
                }

                float v;
                _lastUsageTimeLookup.TryGetValue(ID, out v);
                return v;
            }
            set
            {
                if (useCategoryCooldown)
                {
                    category.lastUsageTime = value;
                    return;
                }

                _lastUsageTimeLookup[ID] = value;
            }
        }

        public bool isInCooldown
        {
            get
            {
                return Time.timeSinceLevelLoad - lastUsageTime < cooldownTime && lastUsageTime > 0f;
            }
        }

        /// <summary>
        /// Value from 0 to ... that defines how far the cooldown is. 0 is just started 1 or higher means the cooldown is over.
        /// Use isInCooldown first to verify if item is in cooldown.
        /// </summary>
        public float cooldownFactor
        {
            get
            {
                return (Time.timeSinceLevelLoad - lastUsageTime) / cooldownTime;
            }
        }

        protected virtual ItemDropHandlerBase dropHandler
        {
            get { return InventorySettingsManager.instance.settings.itemDropHandler; }
        }


        #endregion


        /// <summary>
        /// Returns true if the item can be used, and false when the item cannot be used.
        /// Allows you to add your own conditions to items.
        /// </summary>
        public static List<Predicate<InventoryItemBase>> canUseItemConditionals { get; protected set; }


        static InventoryItemBase()
        {
            canUseItemConditionals = new List<Predicate<InventoryItemBase>>();
        }



        /// <summary>
        /// Get the info of this box, useful when displaying this item.
        /// 
        /// Some elements are displayed by default, these are:
        /// Item icon
        /// Item name
        /// Item description
        /// Item rarity
        /// 
        /// </summary>
        /// <returns>
        /// Returns a LinkedList , which works as follows.
        /// Each InfoBoxUI.Row is used to define a row / property of an item.
        /// Each row has a title and description, the color, font type, etc, can all be changed.
        /// </returns>
        public virtual LinkedList<ItemInfoRow[]> GetInfo()
        {
            var list = new LinkedList<ItemInfoRow[]>();
        
            list.AddLast(new ItemInfoRow[]{
                new ItemInfoRow("Weight", weight.ToString()),
                new ItemInfoRow("Required level", requiredLevel.ToString()),
                new ItemInfoRow("Category", category.name),
            });

            var extra = new List<ItemInfoRow>(3);
            if (sellPrice != null)
            {
                extra.Add(new ItemInfoRow("Sell price", sellPrice.ToString()));
            }
            if (buyPrice != null)
            {
                extra.Add(new ItemInfoRow("Buy price", buyPrice.ToString()));
            }

            if (isDroppable == false || isSellable == false || isStorable == false)
            {
                extra.Add(new ItemInfoRow((!isDroppable ? "Not droppable" : "") + (!isSellable ? ", Not sellable" : "") + (!isStorable ? ", Not storable" : ""), Color.yellow));
            }

            if (extra.Count > 0)
            {
                list.AddLast(extra.ToArray());
            }

            var extraProperties = new List<ItemInfoRow>();
            foreach (var property in stats)
            {
                var prop = property.stat;
                if (prop == null)
                {
                    continue;
                }

                if(prop.showInUI)
                {
                    if(property.isFactor && property.isSingleValue)
                        extraProperties.Add(new ItemInfoRow(prop.statName, (property.floatValue - 1.0f) * 100 + "%", prop.color, prop.color));
                    else
                        extraProperties.Add(new ItemInfoRow(prop.statName, property.value, prop.color, prop.color));
                }
            }

            if(extraProperties.Count > 0)
                list.AddLast(extraProperties.ToArray());
        
            return list;
        }


        /// <summary>
        /// Returns a list of usabilities for this item, what can it do?
        /// </summary>
        public virtual IList<ItemUsability> GetUsabilities()
        {
            var l = new List<ItemUsability>(8);

            if(itemCollection.canUseFromCollection)
            {
                l.Add(new ItemUsability("Use", (item) =>
                {
                    itemCollection[index].TriggerUse();
                }));
            }

            if(currentStackSize > 1 && itemCollection.canPutItemsInCollection)
            {
                l.Add(new ItemUsability("Unstack", (item) =>
                {
                    itemCollection[index].TriggerUnstack(itemCollection);
                }));
            }

            if(isDroppable && itemCollection.canDropFromCollection)
            {
                l.Add(new ItemUsability("Drop", (item) =>
                {
                    itemCollection[index].TriggerDrop(false);
                }));
            }

            return l;
        }


        public virtual bool CanPickupItem()
        {
            return InventoryManager.CanAddItem(this);
        }

        /// <summary>
        /// Pickups the item and stores it in the Inventory.
        /// </summary>
        /// <returns>Returns 0 if item was stored, -1 if not, -2 for some other unknown reason.</returns>
        public virtual bool PickupItem()
        {
            //itemCollection = null; // No item collection if we're "picking" up stuff.
            bool pickedUp = InventoryManager.AddItem(this);
            if (pickedUp)
                NotifyItemPickedUp();

            return pickedUp;
        }

        public virtual void NotifyItemPickedUp()
        {

        }

        /// <summary>
        /// When an item is used, notify the object so that events can be fired.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="alsoNotifyCollection">If the collection of the item didn't change in the process it's safe to notify the collection.</param>
        public virtual void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            // Set the last used time, used to figure out if item is in cooldown
            lastUsageTime = Time.timeSinceLevelLoad;

            if (itemCollection != null && alsoNotifyCollection)
            {
                itemCollection.NotifyItemUsed(this, ID, index, amount); // Dont forget the collection
            }
        }

        public void NotifyItemUnstacked(uint newSlot, uint amount)
        {

        }

        public void NotifyItemDropped(GameObject dropObj, bool notifyCollection = true)
        {
            if (itemCollection != null)
            {
                // Clear old collection (CLEARS COLLECTION REFERENCE IN THIS OBJECT ALSO!)
                itemCollection.NotifyItemDropped(this, ID, currentStackSize, dropObj);
            }
        }

        public virtual bool VerifyCustomUseConditionals()
        {
            foreach (var canUse in canUseItemConditionals)
            {
                if (canUse(this) == false)
                    return false;
            }

            return true;
        }

        public virtual bool CanUse()
        {
            if (itemCollection != null)
            {
                // Collection denies action
                if (itemCollection.canUseFromCollection == false)
                    return false;
            }

            if (VerifyCustomUseConditionals() == false)
            {
                return false;
            }

            foreach (var prop in stats)
            {
                if (prop.actionEffect == StatDecorator.ActionEffect.Decrease)
                {
                    if (prop.actionEffect == StatDecorator.ActionEffect.Decrease && prop.CanDoDecrease(PlayerManager.instance.currentPlayer.inventoryPlayer) == false)
                    {
                        InventoryManager.instance.sceneLangDatabase.itemCannotBeUsedToLowStat.Show(name, description, prop.stat.statName);
                        return false;
                    }
                }
            }

            foreach (var prop in usageRequirement)
            {
                if (prop.CanUse(PlayerManager.instance.currentPlayer.inventoryPlayer) == false)
                {
                    InventoryManager.instance.sceneLangDatabase.itemCannotBeUsedStatNotValid.Show(name, description, prop.stat.statName);
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Use the item, returns the amount of items that have been used.
        /// (n) the amount of items that have been used.
        ///  0 when 0 items were used but there is still an effect (like a re-usable item that doesn't decrease in stack size)
        /// -1 when the item is in cooldown
        /// -2 when the item cannot be used.
        /// When overriding this method, do not forget to call base.Use();
        /// <b>Note that the caller has to handle the UI repaint.</b>
        /// </summary>
        /// <returns>Returns -1 if in timeout / cooldown, returns -2 if item use failed, 0 is 0 items were used, 1 if 1 item was used, 2 if 2...</returns>
        public virtual int Use()
        {
            if (itemCollection != null)
            {
                // Collection has overridden behavior.
                bool overrideBehaviour = itemCollection.OverrideUseMethod(this);
                if (overrideBehaviour)
                {
                    return -2;
                }
            }
            
            if (CanUse() == false)
                return -2;

            if (isInCooldown)
            {
                InventoryManager.langDatabase.itemIsInCooldown.Show(name, description, lastUsageTime + cooldownTime - Time.timeSinceLevelLoad, cooldownTime);
                return -1;
            }
            
            return 0;
        }

        public virtual GameObject Drop()
        {
            return dropHandler.DropItem(this);
        }

        public virtual GameObject Drop(Vector3 position, Quaternion rotation)
        {
            return dropHandler.DropItem(this, position, rotation);
        }

        /// <summary>
        /// Unstack this item to the first empty slot
        /// </summary>
        /// <param name="amount"></param>
        public virtual bool UnstackItem(uint amount)
        {
            if (itemCollection == null)
            {
                Debug.LogWarning("Can't unstack an item that is not in a collection", transform);
                return false;
            }

            return itemCollection.UnstackSlot(index, amount);
        }

        /// <summary>
        /// Unstack this item
        /// </summary>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool UnstackItem(ItemCollectionBase toCollection, uint toSlot, uint amount)
        {
            if (itemCollection == null)
            {
                Debug.LogWarning("Can't unstack an item that is not in a collection", transform);
                return false;
            }

            return itemCollection.UnstackSlot(index, toCollection, toSlot, amount);
        }

        /// <summary>
        /// A very un-efficient way to check if an object is an instance object or not.
        /// Note this method is O(n), so it's rather slow...
        /// </summary>
        public bool IsInstanceObject()
        {
            return !ItemManager.database.items.Contains(this);
        }

        public override string ToString()
        {
            return name;
        }

        public bool Equals(InventoryItemBase other)
        {
            return this.Equals(other, true);
        }

        public virtual bool Equals(InventoryItemBase other, bool checkLocation)
        {
            if (ID != other.ID)
            {
                return false;
            }

            if (checkLocation)
            {
                if (itemCollection != other.itemCollection || index != other.index)
                {
                    return false;
                }
            }

            if (stats.Length != other.stats.Length)
            {
                return false;
            }

            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].Equals(other.stats[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}