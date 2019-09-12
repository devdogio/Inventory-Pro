using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Serialization/Player Prefs Container Saver Loader")]
    public class PlayerPrefsContainerSaverLoader : ContainerSaverLoaderBase
    {


        public override void SaveItems(object serializedData, Action<bool> callback)
        {
            Assert.IsNotNull(callback, "Callback has to be set ( null given )");
            Assert.IsTrue(serializedData is string, "Serialized data is not string, json collection serializer can only use a JSON string.");
            
            PlayerPrefs.SetString(saveName, (string)serializedData);
            callback(true);
        }

        public override void LoadItems(Action<object> callback)
        {
            Assert.IsNotNull(callback, "Callback has to be set ( null given )");

            if (PlayerPrefs.HasKey(saveName) == false)
            {
                throw new SerializedObjectNotFoundException("No serialized data found with key " + saveName);
            }

            callback(PlayerPrefs.GetString(saveName));
        }

    }
}
