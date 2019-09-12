using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public class ItemContainerSerializationModel
    {
        public InventoryItemSerializationModel[] items = new InventoryItemSerializationModel[0];
        

        public ItemContainerSerializationModel()
        { }

        public ItemContainerSerializationModel(IInventoryItemContainer container)
        {
            FromContainer(container);
        }

        /// <summary>
        /// Fill this data model with a collection reference.
        /// Gets the collection data from the collection and stores it in this serializable model.
        /// </summary>
        public void FromContainer(IInventoryItemContainer container)
        {
            // Serialize based on inventory item serialization model.
            items = new InventoryItemSerializationModel[container.items.Length];
            for (int i = 0; i < container.items.Length; i++)
            {
                var item = container.items[i];
                InventoryItemSerializationModel inst = null;
                if (item != null)
                {
                    var classes = ReflectionUtility.GetAllClassesWithAttribute(typeof(SerializationModelAttribute), true);
                    Type serializationModel = typeof(InventoryItemSerializationModel);

                    foreach (var c in classes)
                    {
                        var attrib = (SerializationModelAttribute)c.GetCustomAttributes(typeof(SerializationModelAttribute), true).First();
                        if (c == item.GetType())
                        {
                            DevdogLogger.LogVerbose("Using custom serialization model for " + item.GetType().Name + " - " + attrib.type.Name);
                            serializationModel = attrib.type;
                        }
                    }

                    inst = (InventoryItemSerializationModel)Activator.CreateInstance(serializationModel);
                    inst.FromItem(item);
                }
                else
                {
                    inst = new InventoryItemSerializationModel(null);
                }

                items[i] = inst;
            }
        }

        /// <summary>
        /// Fill a collection using this data object.
        /// </summary>
        public void ToContainer(IInventoryItemContainer container)
        {
            container.items = items.Select(o => o.ToItem()).ToArray();
        }
    }
}
