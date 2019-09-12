using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    public class StandardSlotDragHandler : DragHandlerBase
    {
        private RectTransform _currentlyDraggingRectTransform;

        public StandardSlotDragHandler(int priority)
            : base(priority)
        {
            dragModel = new UIDragModel();
        }


        public override bool CanUse(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Left;
        }

        public override UIDragModel OnBeginDrag(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            currentlyDragging = wrapper;
            _currentlyDraggingRectTransform = wrapper.gameObject.GetComponent<RectTransform>();
            dragModel.Reset();

            dragModel.startIndex = (int) wrapper.index;
            dragModel.startItemCollection = wrapper.itemCollection;

            return dragModel;
        }

        public override UIDragModel OnDrag(PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragModel;

            InventoryUIUtility.PositionRectTransformAtPosition(_currentlyDraggingRectTransform, eventData.position);
            return dragModel;
        }

        public override UIDragModel OnEndDrag(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragModel;

            if (wrapper != null)
            {
                dragModel.endIndex = (int)wrapper.index;
                dragModel.endItemCollection = wrapper.itemCollection;
            }

            UnityEngine.Object.Destroy(currentlyDragging.gameObject); // No longer need it, destroy 
            currentlyDragging = null;

            return dragModel;
        }
    }
}