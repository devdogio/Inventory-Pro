using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro.Demo
{
    [RequireComponent(typeof(Trigger))]
    public class MyCustomCollectionTrigger : MonoBehaviour, IInventoryItemContainer, ITriggerCallbacks
    {
        [SerializeField]
        private InventoryItemBase[] _items = new InventoryItemBase[0];
        public InventoryItemBase[] items
        {
            get { return _items; }
            set { _items = value; }
        }


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        private CollectionToArraySyncer _syncer;
        private ItemCollectionBase _collection;


        public void Awake()
        {
            // Create instance objects.
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    items[i] = GameObject.Instantiate<InventoryItemBase>(items[i]);
                    items[i].transform.SetParent(transform);
                    items[i].gameObject.SetActive(false);
                }
            }

            // The triggerHandler component, that is always there because of RequireComponent
            var trigger = GetComponent<Trigger>();

            // The collection we want to place the items into.
            _collection = trigger.window.window.GetComponent<ItemCollectionBase>();
        }

        public bool OnTriggerUsed(Player player)
        {
            // When the user has triggered this object, set the items in the window
            _collection.SetItems(items, true);

            _syncer = new CollectionToArraySyncer(_collection, items);
            _syncer.StartSyncing();

            // And done!
            return false;
        }

        public bool OnTriggerUnUsed(Player player)
        {
            _syncer.StopSyncing();
            return false;
        }
    }
}