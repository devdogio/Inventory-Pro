using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public class CollectionAvoidRebuildLock : IDisposable
    {
        public ItemCollectionBase[] collections { get; set; }

        public CollectionAvoidRebuildLock(params ItemCollectionBase[] collections)
        {
            this.collections = collections;
            foreach (var col in this.collections)
            {
                col.disableCounterRebuildBlocks++;
            }
        }

        public void Dispose()
        {
            foreach (var col in this.collections)
            {
                col.disableCounterRebuildBlocks--;
            }
        }
    }
}
