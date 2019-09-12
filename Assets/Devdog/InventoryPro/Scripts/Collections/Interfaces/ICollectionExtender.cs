using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    public interface ICollectionExtender
    {
        /// <summary>
        /// The collection being extended.
        /// </summary>
        ItemCollectionBase extendingCollection { get; }

        /// <summary>
        /// The collection that is extending the extendingCollection
        /// </summary>
        ItemCollectionBase extenderCollection { get; }
    }
}
