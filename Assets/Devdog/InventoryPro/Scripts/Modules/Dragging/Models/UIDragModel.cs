using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.UI
{
    public partial class UIDragModel
    {
        public int startIndex = -1;
        public ItemCollectionBase startItemCollection;

        public int endIndex = -1;
        public ItemCollectionBase endItemCollection;

        public bool endOnSlot
        {
            get
            {
                return endItemCollection != null;
            }
        }


        public void Reset()
        {
            startIndex = -1;
            startItemCollection = null;

            endIndex = -1;
            endItemCollection = null;
        }
    }
}
