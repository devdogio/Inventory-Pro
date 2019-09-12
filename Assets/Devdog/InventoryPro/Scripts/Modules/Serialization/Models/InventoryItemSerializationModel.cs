using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;


namespace Devdog.InventoryPro
{
    public partial class InventoryItemSerializationModel
    {
        /// <summary>
        /// ID is -1 if no item is in the given slot.
        /// </summary>
        public int itemID;
        public uint amount;

        public StatDecoratorSerializationModel[] stats = new StatDecoratorSerializationModel[0];
        public string collectionName;

        public virtual bool isReference
        {
            get { return string.IsNullOrEmpty(collectionName) == false; }
        }

        public InventoryItemSerializationModel()
        {
            
        }

        public InventoryItemSerializationModel(InventoryItemBase item)
        {
            FromItem(item);
        }

        public virtual void FromItem(InventoryItemBase item)
        {
            if (item == null)
            {
                this.itemID = -1;
                this.amount = 0;
                this.collectionName = string.Empty;
                this.stats = new StatDecoratorSerializationModel[0];
                return;
            }

            this.itemID = (int) item.ID;
            this.amount = item.currentStackSize;
            this.collectionName = item.itemCollection != null ? item.itemCollection.collectionName : string.Empty;
            this.stats = item.stats.Select(o => new StatDecoratorSerializationModel(o)).ToArray();
        }

        public virtual InventoryItemBase ToItem()
        {
            if (itemID < 0 || itemID > ItemManager.database.items.Length - 1)
            {
//                DevdogLogger.LogWarning("ItemID is out of range, trying to deserialize item " + itemID);
                return null;
            }

            var item = ItemManager.database.items[itemID];
            var inst = UnityEngine.Object.Instantiate<InventoryItemBase>(item);
            var s = this.stats.Select(o => o.ToStat()).ToArray();

            inst.currentStackSize = amount;
            inst.stats = s;
            if (string.IsNullOrEmpty(collectionName) == false)
            {
                inst.itemCollection = ItemCollectionBase.FindByName(collectionName);
            }

            return inst;
        }
    }
}
