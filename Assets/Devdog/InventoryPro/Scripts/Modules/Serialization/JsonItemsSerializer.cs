using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public class JsonItemsSerializer : IItemsSerializer
    {
        public virtual object SerializeCollection(ItemCollectionBase collection)
        {
            var serializationModel = new ItemCollectionSerializationModel(collection);
            return JsonSerializer.Serialize(serializationModel, null);
        }

        public virtual object SerializeContainer(IInventoryItemContainer container)
        {
            var serializationModel = new ItemContainerSerializationModel(container);
            return JsonSerializer.Serialize(serializationModel, null);
        }

        public virtual ItemCollectionSerializationModel DeserializeCollection(object serializedData)
        {
            Assert.IsTrue(serializedData is string, "Serialized data is not string, json collection serializer can only use a JSON string.");

            var model = new ItemCollectionSerializationModel();
            JsonSerializer.DeserializeTo(ref model, serializedData as string, null);

            return model;
        }

        public virtual ItemContainerSerializationModel DeserializeContainer(object serializedData)
        {
            Assert.IsTrue(serializedData is string, "Serialized data is not string, json collection serializer can only use a JSON string.");

            var model = new ItemContainerSerializationModel();
            JsonSerializer.DeserializeTo(ref model, serializedData as string, null);

            return model;
        }
    }
}
