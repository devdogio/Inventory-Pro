using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;

namespace Devdog.InventoryPro.UI
{
    public static class InventoryUIDragUtility
    {
        public delegate void Drag(UIDragModel dragModel, ItemCollectionSlotUIBase dragging, PointerEventData eventData);
        //public delegate void Drag(InventoryUIItemWrapperBase dragging, PointerEventData eventData);
        //public delegate void EndDrag(InventoryUIItemWrapperBase dragging, PointerEventData eventData);

        public static event Drag OnStartDragging;
        public static event Drag OnDragging;
        public static event Drag OnEndDragging;


        #region Variables

        /// <summary>
        /// The item we're currently dragging, can be null.
        /// </summary>
        private static ItemCollectionSlotUIBase draggingItem
        {
            get
            {
                if (currentDragHandler == null)
                    return null;

                return currentDragHandler.currentlyDragging;
            }
        }

        /// <summary>
        /// Check if we're currently dragging an item.
        /// </summary>
        public static bool isDraggingItem
        {
            get
            {
                return draggingItem != null;
            }
        }


        public static List<DragHandlerBase> dragHandlers { get; private set; }
        public static DragHandlerBase currentDragHandler { get; private set; }

        #endregion


        // Static constructor
        static InventoryUIDragUtility()
        {
            // TODO: Move to manager, and serialize.
            dragHandlers = new List<DragHandlerBase>
            {
                new StandardSlotDragHandler(0),
                new UnstackSlotDragHandler(10)
            };

            if(Application.isMobilePlatform)
                dragHandlers.Add(new MobileSlotDragHandler(20));
        }

        /// <summary>
        /// Creates a draggable object.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static ItemCollectionSlotUIBase CreateDragObject(ItemCollectionSlotUIBase from)
        {
            var copy = UnityEngine.Object.Instantiate<ItemCollectionSlotUIBase>(from);
            copy.index = from.index;
            copy.itemCollection = from.itemCollection;
            copy.item = from.item;

            var copyComp = copy.GetComponent<RectTransform>();
            copyComp.SetParent(InventoryManager.instance.uiRoot.transform);
            copyComp.transform.localPosition = new Vector3(copyComp.transform.localPosition.x, copyComp.transform.localPosition.y, 0.0f);
            copyComp.localScale = Vector3.one;
            copyComp.sizeDelta = from.GetComponent<RectTransform>().sizeDelta;

            float singleSize = copyComp.sizeDelta.x / copy.item.layoutSizeCols;
            float halfSize = singleSize / 2f;
            copyComp.pivot = new Vector2(halfSize / copyComp.sizeDelta.x, (copyComp.sizeDelta.y - halfSize) / copyComp.sizeDelta.y);

            // Canvas group allows object to ignore raycasts.
            var group = copyComp.gameObject.GetComponent<CanvasGroup>();
            if (group == null)
                group = copyComp.gameObject.AddComponent<CanvasGroup>();

            group.blocksRaycasts = false; // Allows rays to go through so we can hover over the empty slots.
            group.interactable = false;

            return copy;
        }

        /// <summary>
        /// Grab the best suited handler for dragging.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private static DragHandlerBase FindBestDragHandler(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            DragHandlerBase best = null;
            foreach (var handler in dragHandlers)
            {
                if (handler.CanUse(wrapper, eventData) && (best == null || handler.priority > best.priority))
                    best = handler;
            }

            return best;
        }

        public static UIDragModel OnBeginDrag(ItemCollectionSlotUIBase originalWrapper, PointerEventData eventData)
        {
            if (draggingItem != null)
            {
                Debug.LogWarning("Item still attached to cursor, can only drag one item at a time", draggingItem.gameObject);
                return null; // Can only drag one item at a time
            }

            currentDragHandler = FindBestDragHandler(originalWrapper, eventData);
            if (currentDragHandler == null)
                return null;

            var toDrag = CreateDragObject(originalWrapper);
            var lookup = currentDragHandler.OnBeginDrag(toDrag, eventData);
            if (OnStartDragging != null)
                OnStartDragging(lookup, toDrag, eventData);

            return lookup;
        }

        public static void OnDrag(PointerEventData eventData)
        {
            if (currentDragHandler != null)
            {
                currentDragHandler.OnDrag(eventData);

                if (OnDragging != null)
                    OnDragging(currentDragHandler.dragModel, currentDragHandler.currentlyDragging, eventData);
            }
        }

        public static UIDragModel OnEndDrag(PointerEventData eventData)
        {
            if (currentDragHandler == null)
                return null;

            var lookup = currentDragHandler.OnEndDrag(InventoryUIUtility.currentlyHoveringSlot, eventData);
            if (OnEndDragging != null)
                OnEndDragging(lookup, currentDragHandler.currentlyDragging, eventData);

            return lookup;
        }
    }
}