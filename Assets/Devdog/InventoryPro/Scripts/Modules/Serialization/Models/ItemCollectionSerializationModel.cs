using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public class ItemCollectionSerializationModel
    {
        public InventoryItemSerializationModel[] items = new InventoryItemSerializationModel[0];
        public CurrencyDecoratorSerializationModel[] currencies = new CurrencyDecoratorSerializationModel[0];
        
        public ItemCollectionSerializationModel()
        { }

        public ItemCollectionSerializationModel(ItemCollectionBase itemCollection)
        {
            FromCollection(itemCollection);
        }

        /// <summary>
        /// Fill this data model with a collection reference.
        /// Gets the collection data from the collection and stores it in this serializable model.
        /// </summary>
        /// <param name="collection"></param>
        public virtual void FromCollection(ItemCollectionBase collection)
        {
            currencies = collection.currenciesGroup.lookups.Select(o => new CurrencyDecoratorSerializationModel(o)).ToArray();
            //            items = collection.items.Select(o => new InventoryItemSerializationModel(o.item)).ToArray();


            // Serialize based on inventory item serialization model.
            items = new InventoryItemSerializationModel[collection.items.Length];
            for (int i = 0; i < collection.items.Length; i++)
            {
                var item = collection.items[i];
                InventoryItemSerializationModel inst = null;
                if (item.item != null)
                {
                    var classes = ReflectionUtility.GetAllClassesWithAttribute(typeof(SerializationModelAttribute), true);
                    Type serializationModel = typeof(InventoryItemSerializationModel);

                    foreach (var c in classes)
                    {
                        var attrib = (SerializationModelAttribute)c.GetCustomAttributes(typeof(SerializationModelAttribute), true).First();
                        if (c == item.item.GetType())
                        {
                            DevdogLogger.LogVerbose("Using custom serialization model for " + item.item.GetType().Name + " - " + attrib.type.Name);
                            serializationModel = attrib.type;
                        }
                    }

                    inst = (InventoryItemSerializationModel) Activator.CreateInstance(serializationModel);
                    inst.FromItem(item.item);
                }
                else
                {
                    inst = new InventoryItemSerializationModel(item.item);
                }

                items[i] = inst;
            }
        }

        public virtual void ToCollection(ItemCollectionBase collection)
        {
            collection.Resize((uint)items.Length);
            if (collection.useReferences)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    var c = ItemCollectionBase.FindByName(item.collectionName);
                    if (item.amount > 0 && item.itemID >= 0)
                    {
                        collection[i].item = c.Find((uint)item.itemID);
                        collection[i].Repaint();
                    }
                }
            }
            else
            {
                var deserializedItems = items.Select(o => o.ToItem()).ToArray();
                for (int i = 0; i < deserializedItems.Length; i++)
                {
                    collection[i].item = deserializedItems[i];
                    collection[i].Repaint();

                    if (deserializedItems[i] != null)
                    {
                        deserializedItems[i].gameObject.SetActive(false);
                        deserializedItems[i].transform.SetParent(collection.container);
                    }
                }
            }

            // Handle equippable items; Make sure the reference to the equippable collection is set.
            foreach(var item in collection)
            {
                var eq = item.item as EquippableInventoryItem;
                var charCollection = collection as ICharacterCollection;
                if (eq != null && charCollection != null)
                {
                    eq.equippedToCollection = charCollection;
                    eq.NotifyItemEquipped(charCollection.equippableSlots[eq.index], eq.currentStackSize);
                }
            }

            collection.UnRegisterCurrencyEvents();
            collection.currenciesGroup = new CurrencyDecoratorCollection(false);

            var deserializedCurrencies = currencies.Select(o => o.ToCurrencyDecorator());
            foreach (var c in deserializedCurrencies)
            {
                collection.currenciesGroup.AddCurrency(c);
            }
            
            collection.RegisterCurrencyEvents();
        }
    }
}
