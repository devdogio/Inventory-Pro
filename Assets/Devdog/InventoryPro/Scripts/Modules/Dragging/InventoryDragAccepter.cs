using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.EventSystems;
using Devdog.General.UI;

namespace Devdog.InventoryPro.UI
{
    [RequireComponent(typeof(ItemCollectionBase))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "UI Helpers/Drag accepter UI")]
    public partial class InventoryDragAccepter : MonoBehaviour
    {
        /// <summary>
        /// The visual representation that shows the drag accept.
        /// </summary>
        public UIWindow acceptShow;

        /// <summary>
        /// The visual representation that shows the drag deny
        /// </summary>
        public UIWindow denyShow;

        /// <summary>
        /// When the user drags the item onto a wrapper object, should the event be fired?
        /// </summary>
        public bool triggerWhenDroppedOnSlot = false;

        [SerializeField]
        protected ItemCollectionBase collection;

        protected IInventoryDragAccepter iCollection { get; set; }

        [SerializeField]
        [Required]
        protected UIWindow window;

        public void Awake()
        {
            if (collection == null)
            {
                Debug.LogError("No ItemCollection found on Drag accepter!", transform);
                return;
            }

            // Got a col
            iCollection = (IInventoryDragAccepter)GetComponent(typeof(IInventoryDragAccepter));
            if (iCollection == null)
            {
                Debug.LogWarning("InventoryDragAccepter has a collection, but collection does not implement IInventoryDragAccepter!", transform);
                return;
            }
            
            window.OnShow += WindowOnShow;
        }

        public void OnEnable()
        {
            InventoryUIDragUtility.OnStartDragging += OnStartDragging;
            InventoryUIDragUtility.OnEndDragging += OnEndDragging;
        }

        public void OnDisable()
        {
            InventoryUIDragUtility.OnStartDragging -= OnStartDragging;
            InventoryUIDragUtility.OnEndDragging -= OnEndDragging;
        }


        private void WindowOnShow()
        {
            TryShow();
        }

        public virtual void ShowAccept()
        {
            if (window.isVisible == false)
                return;

            if (acceptShow != null)
                acceptShow.Show();

            if (denyShow != null)
                denyShow.Hide();
        }

        public virtual void ShowDeny()
        {
            if (window.isVisible == false)
                return;

            if (denyShow != null)
                denyShow.Show();

            if (acceptShow != null)
                acceptShow.Hide();
        }

        public virtual void Hide()
        {
            if (window.isVisible == false)
                return;

            if (denyShow != null)
                denyShow.Hide();

            if (acceptShow != null)
                acceptShow.Hide();
        }

        public virtual void TryShow()
        {
            if (InventoryUIDragUtility.isDraggingItem && InventoryUIDragUtility.currentDragHandler != null)
            {
                if (iCollection.AcceptsDragItem(InventoryUIDragUtility.currentDragHandler.currentlyDragging.item))
                    ShowAccept();
                else
                    ShowDeny();
            }
        }


        private void OnStartDragging(UIDragModel dragModel, ItemCollectionSlotUIBase dragging, PointerEventData eventData)
        {
            TryShow();
        }

        private void OnEndDragging(UIDragModel dragModel, ItemCollectionSlotUIBase dragging, PointerEventData eventData)
        {
            Hide();

            if (eventData.hovered.Contains(gameObject))
            {
                if (dragModel.startItemCollection != collection)
                {
                    // As long as the player didn't directly drop it on a wrapper object, we can auto. equip it to the right slot.
                    bool droppedOnEquipSlot = false;
                    if (triggerWhenDroppedOnSlot == false)
                    {
                        foreach (var o in eventData.hovered)
                        {
                            var wrapper = o.GetComponent<ItemCollectionSlotUIBase>();
                            if (wrapper != null)
                            {
                                droppedOnEquipSlot = true;
                                break;
                            }
                        }
                    }

                    if (droppedOnEquipSlot == false)
                    {
                        var item = dragModel.startItemCollection[dragModel.startIndex].item;
                        iCollection.AcceptDragItem(item);
                       
                        //Debug.Log("Dragged from other collection " + item.name);
                    }
                }
            }
        }
    }
}