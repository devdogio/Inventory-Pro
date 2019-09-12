using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public abstract class ContainerSaverLoaderBase : SaverLoaderBase
    {
        private IInventoryItemContainer _container;
        protected IInventoryItemContainer container
        {
            get
            {
                if (_container == null)
                {
                    _container = (IInventoryItemContainer)GetComponent(typeof(IInventoryItemContainer));
                }

                return _container;
            }
        }

        public override string saveName
        {
            get
            {
                return SaveNamePrefix + "Container_" + container.uniqueName.ToLower().Replace(" ", "_");
            }
        }

        public override void Save()
        {
            try
            {
                SaveItems(serializer.SerializeContainer(container), (bool saved) =>
                {
                    DevdogLogger.LogVerbose("Saved container " + saveName);
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
                LoadItems((data) =>
                {
                    DevdogLogger.LogVerbose("Loaded container " + saveName);

                    var model = serializer.DeserializeContainer(data);
                    model.ToContainer(container);
                    foreach (var item in container.items)
                    {
                        if (item != null)
                        {
                            item.gameObject.SetActive(false);
                            item.transform.SetParent(transform);
                            item.transform.localPosition = Vector3.zero;
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
