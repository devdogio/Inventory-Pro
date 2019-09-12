using System;
using System.Collections;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class CollectionSaverLoaderBase : SaverLoaderBase
    {
        [SerializeField]
        private ItemCollectionBase _collection;
        protected ItemCollectionBase collection
        {
            get 
            {
                if (_collection == null)
                {
                    _collection = GetComponent<ItemCollectionBase>();
                }

                return _collection;
            }
        }

        public override string saveName
        {
            get
            {
                return SaveNamePrefix + "Collection_" + collection.collectionName.ToLower().Replace(" ", "_");
            }
        }

        public override void Save()
        {
            try
            {
                var serializedCollection = serializer.SerializeCollection(collection);
                SaveItems(serializedCollection, (bool saved) =>
                {
                    DevdogLogger.LogVerbose("Saved collection " + collection.collectionName);
                });
            }
            catch (SerializedObjectNotFoundException e)
            {
                Debug.LogWarning(e.Message + e.StackTrace);
            }
        }

        public override void Load()
        {
            try
            {
                LoadItems((object data) =>
                {
                    DevdogLogger.LogVerbose("Loaded collection " + collection.collectionName);

                    var model = serializer.DeserializeCollection(data);
                    model.ToCollection(collection);

                    var character = this.collection.GetComponent<ICharacterCollection>();
                    // Are we loading items to a character collection?
                    if (character != null)
                    {
                        // if so, go through the items and
                        foreach (var slot in collection.items)
                        {
                            var equippedItem = slot.item as EquippableInventoryItem;

                            // if they must be equipped visually, do so
                            if (equippedItem != null && equippedItem.equipVisually)
                            {
                                character.character.equipmentHandler.EquipItemVisually(equippedItem, character.equippableSlots[equippedItem.index]);
                            }
                        }
                    }
                });
            }
            catch (SerializedObjectNotFoundException e)
            {
                Debug.LogWarning(e.Message + e.StackTrace);
            }
        }
    }
}
