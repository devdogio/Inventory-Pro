using System;
using UnityEngine;
using System.Collections;
using Devdog.General.UI;

namespace Devdog.InventoryPro.UI
{
    public partial class InventoryUIUtility
    {
        #region Variables 

        public static ItemCollectionSlotUIBase currentlyHoveringSlot
        {
            get { return ItemCollectionSlotUI.currentlyHoveringSlot; }
        }

        /// <summary>
        /// Get the currently selected slot. Is null if none selected.
        /// Note that this is not the same as the hovering item.
        /// </summary>
        public static ItemCollectionSlotUIBase currentlySelectedSlot
        {
            get
            {
                var o = UIUtility.currentlySelectedGameObject;
                if (o != null)
                    return o.GetComponent<ItemCollectionSlotUIBase>();

                return null;
            }
        }

        /// <summary>
        /// Get the current mouse position, or the current touch position if this is a mobile device.
        /// </summary>
        public static Vector3 mouseOrTouchPosition
        {
            get
            {
                if (Application.isMobilePlatform)
                {
                    if (Input.touchCount > 0)
                        return Input.GetTouch(0).position;

                }

                return Input.mousePosition;
            }
        }

        #endregion


        public static void PositionRectTransformAtPosition(RectTransform rectTransform, Vector3 screenPos)
        {
            var canvas = InventoryManager.instance.uiRoot;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, canvas.worldCamera, out pos);
                rectTransform.position = canvas.transform.TransformPoint(pos);
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = screenPos;
            }
        }
    }
}