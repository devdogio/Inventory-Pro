using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text.RegularExpressions;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;
using UnityEngine;

namespace Devdog.InventoryPro.UI
{
    using Devdog.InventoryPro.Dialogs;

    /// <summary>
    /// Convenience methods to use with Unity UI
    /// </summary>
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "UI Helpers/Inventory Action helper")]
    public partial class InventoryActionHelper : MonoBehaviour
    {
        public Color highlightColor = Color.yellow;

        private Sprite _slotMarkedStartSprite;
        private Color _slotMarkedStartColor;

        private static ItemCollectionSlotUIBase _markedSlot;
        private ItemCollectionSlotUIBase markedSlot
        {
            get { return _markedSlot; }
            set
            {
                _markedSlot = value;
                if (_markedSlot != null && _markedSlot.item != null)
                {
                    var button = _markedSlot.gameObject.GetComponent<UnityEngine.UI.Button>();
                    if (button != null && button.targetGraphic != null)
                    {
                        var image = button.targetGraphic as UnityEngine.UI.Image;
                        if (image != null)
                        {
                            image.sprite = button.spriteState.highlightedSprite;
                            image.color = highlightColor;
                        }
                    }
                }
            }
        }

        private static bool canUseSlot
        {
            get
            {
                if (InputManager.CanReceiveUIInput(UIUtility.currentlySelectedGameObject) == false)
                    return false;

                return InventoryUIUtility.currentlySelectedSlot != null;
            }
        }

        public void KillCurrentPlayer(bool dropAll)
        {
            PlayerManager.instance.currentPlayer.inventoryPlayer.NotifyPlayerDied(dropAll);
        }

        public void LimitInputToThis()
        {
            InputManager.LimitUIInputTo(gameObject);
        }

        public void RemoveLimitInputToThis()
        {
            InputManager.RemoveUILimitInput(gameObject);
        }

        public void TriggerUseCurrentlySelectedSlot()
        {
            if (canUseSlot)
                InventoryUIUtility.currentlySelectedSlot.TriggerUse();

            ClearMarkedSlot(); // Slot could be cleared, clear marker as well
        }

        public void TriggerDropCurrentlySelectedSlot()
        {
            if (canUseSlot)
                InventoryUIUtility.currentlySelectedSlot.TriggerDrop();

            ClearMarkedSlot(); // Slot is cleared, clear marker as well
        }

        public void TriggerUnstackCurrentlySelectedSlot()
        {
            if (canUseSlot)
                InventoryUIUtility.currentlySelectedSlot.TriggerUnstack(InventoryUIUtility.currentlySelectedSlot.itemCollection);

            ClearMarkedSlot(); // Slot could be cleared, clear marker as well
        }

        public void TriggerContextMenuCurrentlySelectedSlot()
        {
            if (canUseSlot)
                InventoryUIUtility.currentlySelectedSlot.TriggerContextMenu();

            ClearMarkedSlot(); // Slot could be cleared, clear marker as well
        }

        /// <summary>
        /// Marking can be used to temp "select" a wrapper. You can then later read the temp selected / marked wrapper.
        /// </summary>
        public void MarkCurrentlySelectedSlot()
        {
            if (canUseSlot)
            {
                if (InventoryUIUtility.currentlySelectedSlot.itemCollection.canDragInCollection == false)
                    return;

                var button = InventoryUIUtility.currentlySelectedSlot.gameObject.GetComponent<UnityEngine.UI.Button>();
                if (button != null && button.targetGraphic != null)
                {
                    var img = button.targetGraphic as UnityEngine.UI.Image;
                    if (img != null)
                        _slotMarkedStartSprite = img.sprite;

                    _slotMarkedStartColor = button.targetGraphic.color;
                }
            }

            markedSlot = InventoryUIUtility.currentlySelectedSlot;
        }

        /// <summary>
        /// Move the previously marked wrapper to the currently / newly selected wrapper.
        /// This can cause a move, merge, or swap. (depending on the new location)
        /// 
        /// Note: If markedWrapper is null it will be set using this method.
        /// </summary>
        public void MoveCurrentlySelectedSlotToMarkedSlot()
        {
            if (canUseSlot == false)
            {
                ClearMarkedSlot();
                return;
            }

            if (markedSlot == null || markedSlot.item == null)
            {
                MarkCurrentlySelectedSlot();
                return;
            }

            var newWrapper = InventoryUIUtility.currentlySelectedSlot;
            if (newWrapper == null)
            {
                ClearMarkedSlot();
                return; // No new location selected.
            }
            
            // Move it (move, merge or swap)
            markedSlot.itemCollection.SwapOrMerge(markedSlot.index, newWrapper.itemCollection, newWrapper.index);

            ClearMarkedSlot();
        }

        private void ClearMarkedSlot()
        {
            if (canUseSlot == false)
                return;

            if (markedSlot == null)
                return;

            // Reset the marked wrapper's original sprite.
            var button = markedSlot.gameObject.GetComponent<UnityEngine.UI.Button>();
            if (button != null && button.targetGraphic != null)
            {
                var image = button.targetGraphic as UnityEngine.UI.Image;
                if (image != null)
                    image.sprite = _slotMarkedStartSprite;

                button.targetGraphic.color = _slotMarkedStartColor;
            }

            markedSlot = null;
        }


        public void SelectFirstSlotOfCollection(ItemCollectionBase collection)
        {
            if (collection.items.Length == 0)
            {
                Debug.LogWarning("Collection has no items, can't select first item.");
                return;
            }

            collection[0].Select();
        }
    }
}
