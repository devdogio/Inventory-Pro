using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro.UI;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    public abstract class DragHandlerBase
    {
        public int priority { get; protected set; }
        public ItemCollectionSlotUIBase currentlyDragging { get; protected set; }
        public UIDragModel dragModel { get; protected set; }


        protected DragHandlerBase(int priority)
        {
            this.priority = priority;
            this.dragModel = new UIDragModel();
        }
        
        /// <summary>
        /// Can this handler be used to drag an item? Return true if allowed to use, false if not.
        /// </summary>
        public abstract bool CanUse(ItemCollectionSlotUIBase wrapper, PointerEventData eventData);

        public abstract UIDragModel OnBeginDrag(ItemCollectionSlotUIBase wrapperToDrag, PointerEventData eventData);
        public abstract UIDragModel OnDrag(PointerEventData eventData);
        public abstract UIDragModel OnEndDrag(ItemCollectionSlotUIBase currentlyHoveringWrapper, PointerEventData eventData);
    }
}
