using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.UI
{
    public interface IInventoryDragAccepter
    {

        /// <summary>
        /// Check if the drag accepter can accept this specific item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AcceptsDragItem(InventoryItemBase item);

        /// <summary>
        /// Accept the item
        /// </summary>
        /// <returns></returns>
        bool AcceptDragItem(InventoryItemBase item);
    }
}
