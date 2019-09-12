using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventoryPro
{

    /// <summary>
    /// A "wrapper" class that holds an item once it's dropped. When the actual item is dropped it
    /// will be used when a "drop Object" or substituion object is dropped, this call will be used to "hold" the actual item.
    /// </summary>
    [Obsolete("Replaced by ItemTrigger")]
    public partial class ObjectTriggererItemHolder : MonoBehaviour
    {

        /// <summary>
        /// The item we're holding
        /// </summary>
        public InventoryItemBase item;
        

        protected virtual void Start()
        {
            
        }

    }
}
