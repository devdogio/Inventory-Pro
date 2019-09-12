using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Collection UI/Slot UI")]
    public partial class ItemCollectionSlotUI : ItemCollectionSlotUIBase, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region UI Elements

        public Text amountText;
        public Text itemName;
        public Image icon;

        public UIShowValue cooldownVisualizer = new UIShowValue();
        public bool disableIconWhenEmpty = false;


        [NonSerialized]
        private Sprite _startIcon;

        public virtual bool isEmpty
        {
            get
            {
                return item == null;
            }
        }

        /// <summary>
        /// Converts the rect transform's rect to screen space (adds the position to it)
        /// </summary>
        public virtual Rect screenSpaceRect
        {
            get
            {
                var rectTransform = GetComponent<RectTransform>();
                var pos = rectTransform.position;

                var r = new Rect(rectTransform.rect); // TODO: Doesn't work with camera-space UI
                
                r.x += pos.x;
                r.y += pos.y;

                r.width += InventorySettingsManager.instance.settings.onPointerUpInsidePadding.x;
                r.height += InventorySettingsManager.instance.settings.onPointerUpInsidePadding.y;

                r.x -= (InventorySettingsManager.instance.settings.onPointerUpInsidePadding.x / 2);
                r.y -= (InventorySettingsManager.instance.settings.onPointerUpInsidePadding.y / 2);

                return r;
            }
        }

        public static ItemCollectionSlotUI currentlyHoveringSlot { get; protected set; }

        [NonSerialized]
        protected static ItemCollectionSlotUIBase pointerDownOnUIElement;

        private bool isPressingButton
        {
            get { return pointerDownOnUIElement != null; }
        }

        private bool isLongPress
        {
            get
            {
                return Mathf.Approximately(lastDownTime, 0f) == false && Time.timeSinceLevelLoad - InventorySettingsManager.instance.settings.mobileLongPressTime > lastDownTime;                
            }
        }

        private bool isDoubleTap
        {
            get { return Mathf.Approximately(lastDownTime, 0f) == false && Time.timeSinceLevelLoad < lastDownTime + InventorySettingsManager.instance.settings.mobileDoubleTapTime && Mathf.Approximately(lastDownTime, Time.timeSinceLevelLoad) == false; }
        }


        /// <summary>
        /// Last time the button was pressed, used to determine long presses.
        /// </summary>
        protected static float lastDownTime { get; set; }

        protected bool useCustomUpdate = true;

        #endregion

        protected virtual void Awake()
        {
            if (icon != null)
            {
                _startIcon = icon.sprite;
            }
        }
        
        protected virtual void OnEnable()
        {
            if (useCustomUpdate)
            {
                StartCustomUpdate();
            }
        }

        protected virtual void OnDisable()
        {
            if (useCustomUpdate)
            {
                StopCustomUpdate();
            }

            if (currentlyHoveringSlot == this)
            {
                currentlyHoveringSlot = null;
            }

            if (pointerDownOnUIElement == this)
            {
                pointerDownOnUIElement = null;
            }

            if (InventoryUIDragUtility.isDraggingItem)
            {
                InventoryUIDragUtility.OnEndDrag(new PointerEventData(EventSystem.current));
            }
        }

        protected void StartCustomUpdate()
        {
            InvokeRepeating("CustomUpdate", 0f, 0.05f);
        }

        protected void StopCustomUpdate()
        {
            CancelInvoke("CustomUpdate");
        }

        protected virtual void CustomUpdate()
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            RepaintCooldown();

            if (isPressingButton && pointerDownOnUIElement == this && InventoryUIDragUtility.isDraggingItem == false && isLongPress)
            {
                OnLongTap(new PointerEventData(EventSystem.current), InventoryActionInput.EventType.All); // Long press for mobile
            }
        }


        #region Button handler UI events

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;

            if (item != null && itemCollection.canDragInCollection)
            {
                InventoryUIDragUtility.OnBeginDrag(this, eventData);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;


            InventoryUIDragUtility.OnDrag(eventData);

            //if (eventData.button == PointerEventData.InputButton.Left || (eventData.button == PointerEventData.InputButton.Right && InventorySettingsManager.instance.unstackItemButton == PointerEventData.InputButton.Right))
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;

            var lookup = InventoryUIDragUtility.OnEndDrag(eventData);
            if (lookup == null)
                return; // No drag handler.



            // for mobile...
            var endOnData = UIUtility.PointerOverUIObject(InventoryManager.instance.uiRoot, InventoryUIUtility.mouseOrTouchPosition);
            foreach (var d in endOnData)
            {
                var slot = d.gameObject.GetComponent<ItemCollectionSlotUIBase>();
                if (slot != null)
                {
                    lookup.endItemCollection = slot.itemCollection;
                    lookup.endIndex = (int)slot.index;

                    break;
                }
            }

            if (lookup.endOnSlot)
            {
                #region Slot movement

                // When is large and dragged the end drag event can happen on the slots that's being dragged.
                // from     x       x
                // x        to      x
                // x        x       x
                // From -> To -> 1 down -> 1 to the right

                // Offset things for the layout
                var i = lookup.endItemCollection[lookup.endIndex].item;
                if (i != null)
                {
                    if (i == item && item.layoutSize > 1)
                    {
                        // Ended on self, and we're larger than 1x1 -- Try to move item to the side
                        var rect = transform.GetComponent<RectTransform>();
                        var tempItem = lookup.endItemCollection[lookup.endIndex].item;

                        var singleSize = rect.sizeDelta;
                        singleSize.x /= tempItem.layoutSizeCols;
                        singleSize.y /= tempItem.layoutSizeRows;

                        var dif = eventData.pressPosition - eventData.position;
                        var stepsCols = Mathf.RoundToInt(-dif.x / singleSize.x);
                        var stepsRows = Mathf.RoundToInt(dif.y / singleSize.y);

                        lookup.endIndex += (int)(stepsCols + (stepsRows*lookup.endItemCollection.colsCount));
                        lookup.endIndex = Mathf.Clamp(lookup.endIndex, 0, lookup.endItemCollection.items.Length - 1);
                    }
                }

                #endregion

                // Place on a slot
                if (InventorySettingsManager.instance.settings.useUnstackDrag && lookup.endItemCollection.useReferences == false && lookup.endItemCollection.canUnstackItemsInCollection)
                {
                    if (InventorySettingsManager.instance.settings.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.All))
                    {
                        TriggerUnstack(lookup.endItemCollection, lookup.endIndex);
                        return; // Stop the rest otherwise we'll do 2 actions at once.                        
                    }
                }

                lookup.startItemCollection.SwapOrMerge((uint)lookup.startIndex, lookup.endItemCollection, (uint)lookup.endIndex);
            }
            else if (lookup.startItemCollection.useReferences)
            {
                lookup.startItemCollection.SetItem((uint)lookup.startIndex, null);
                lookup.startItemCollection[lookup.startIndex].Repaint();
            }
            else if (UIUtility.isHoveringUIElement == false)
            {
                TriggerDrop();
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (itemCollection == null)
                return;

            pointerDownOnUIElement = currentlyHoveringSlot;
            if (pointerDownOnUIElement == null)
                return;


            //////////// Ugly hack, because input modules will take over soon.
            bool tapped = OnTap(eventData, InventoryActionInput.EventType.OnPointerDown); // Mobile version of OnPointerUp
            if (tapped)
            {
                return;
            }

            if (isDoubleTap)
            {
                OnDoubleTap(eventData, InventoryActionInput.EventType.OnPointerDown);
                return;
            }

            var s = InventorySettingsManager.instance.settings;
            if (s.useContextMenu && s.triggerContextMenuKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerDown))
            {
                if (item != null)
                {
                    TriggerContextMenu();
                    return;
                }
            }

            lastDownTime = Time.timeSinceLevelLoad;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            // Started on a UI element?
            if (pointerDownOnUIElement == null)
                return;

            pointerDownOnUIElement = null;
//            lastDownTime = 0f;

            // Cursor still inside the button on Pointer up?
            var canvas = gameObject.GetComponentInParent<Canvas>();
            if(canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (screenSpaceRect.Contains(eventData.position) == false)
                    return;
            }

            if (isDoubleTap)
            {
                OnDoubleTap(eventData, InventoryActionInput.EventType.OnPointerUp);
                return;
            }

            bool tapped = OnTap(eventData, InventoryActionInput.EventType.OnPointerUp); // Mobile version of OnPointerUp
            if (tapped)
            {
                return;
            }

            var s = InventorySettingsManager.instance.settings;
            if (s.useContextMenu && s.triggerContextMenuKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerUp))
            {
                if (item != null)
                {
                    TriggerContextMenu();
                    return;
                }
            }

            if (item != null && InventoryUIDragUtility.isDraggingItem == false)
            {
                // Check if we're trying to unstack
                if (itemCollection.useReferences == false && InventorySettingsManager.instance.settings.useUnstackClick && itemCollection.canUnstackItemsInCollection)
                {
                    if (InventorySettingsManager.instance.settings.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerUp))
                    {
                        TriggerUnstack(itemCollection);
                        return; // Stop the rest otherwise we'll do 2 actions at once.
                    }
                }

                // Use the item
                if (InventorySettingsManager.instance.settings.useItemKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerUp))
                    TriggerUse();

            }
        }

        /// <summary>
        /// Check if mobile input is valid.
        /// </summary>
        /// <param name="tap"></param>
        /// <param name="eventData"></param>
        /// <returns>True if an action was taken, false if no action was taken.</returns>
        protected virtual bool CheckMobileInput(InventoryActionInput.MobileUIActions tap, InventoryActionInput.EventType eventUsed, PointerEventData eventData)
        {
            var s = InventorySettingsManager.instance.settings;
            if (s.unstackKeys.AllPressed(tap, eventUsed, eventData))
            {
                TriggerUnstack(itemCollection);
                return true;
            }

            if (s.useItemKeys.AllPressed(tap, eventUsed, eventData))
            {
                TriggerUse();
                return true;
            }

            if (s.triggerContextMenuKeys.AllPressed(tap, eventUsed, eventData))
            {
                if (s.useContextMenu)
                {
                    TriggerContextMenu();
                    return true;
                }
            }

            return false; // No action taken
        }

        public virtual bool OnTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.SingleTap, eventUsed, eventData);
        }

        public virtual bool OnDoubleTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.DoubleTap, eventUsed, eventData);
        }

        public virtual bool OnLongTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.LongTap, eventUsed, eventData);
        }

        #endregion

        #region Triggers

        public override void TriggerContextMenu()
        {
            if (item == null)
                return;

            var contextMenu = InventoryManager.instance.contextMenu;

            // Show context menu
            contextMenu.ClearMenuOptions();

            var itemList = item.GetUsabilities();
            itemList = itemCollection.GetExtraItemUsabilities(itemList);
            foreach (var i in itemList)
            {
                contextMenu.AddMenuOption(i.actionName, item, i.useItemCallback);
            }

            contextMenu.window.Show();
        }

        // <inheritdoc />
        public override void TriggerUnstack(ItemCollectionBase toCollection, int toIndex = -1)
        {
            if (item == null || itemCollection.useReferences || itemCollection.canUnstackItemsInCollection == false)
                return;

            if (item.currentStackSize > 1)
            {
                var m = InventorySettingsManager.instance;
                if(m.settings.useUnstackDialog)
                {
                    var d = InventoryManager.langDatabase.unstackDialog;
                    InventoryManager.instance.unstackDialog.ShowDialog(itemCollection.transform, d.title, d.message, 1, (int)item.currentStackSize - 1, item,
                        (int val) =>
                        {
                            if (toIndex != -1)
                                itemCollection.UnstackSlot(index, toCollection, (uint) toIndex, (uint)val, true);
                            else
                                itemCollection.UnstackSlot(index, (uint)val, true);
                        },
                        (int val) =>
                        {
                            // Canceled


                        });
                }
                else
                {
                    if(toIndex != -1)
                        itemCollection.UnstackSlot(index, toCollection, (uint)toIndex, (uint)Mathf.Floor(item.currentStackSize / 2), true);                    
                    else
                        itemCollection.UnstackSlot(index, (uint)Mathf.Floor(item.currentStackSize / 2), true);
                }
            }
        }

        public override void TriggerDrop(bool useRaycast = true)
        {
            if (item == null || itemCollection.canDropFromCollection == false)
                return;
            
            Assert.IsNotNull(PlayerManager.instance.currentPlayer, "Current player is not set, can't drop item.");

            if(item.isDroppable == false)
            {
                InventoryManager.langDatabase.itemCannotBeDropped.Show(item.name, item.description);
                return;
            }

            var s = InventorySettingsManager.instance;
            if (s.settings.showConfirmationDialogWhenDroppingItem && InventoryManager.instance.confirmationDialog == null)
            {
                Debug.LogError("Trying to drop item with confimration dialog, but dialog is not assigned, please check your settings.");
                return;
            }

            if (s.settings.showConfirmationDialogWhenDroppingItem)
            {
                // Not on a button, drop it
                var tempItem = item; // Capture list stuff
                var msg = InventoryManager.langDatabase.confirmationDialogDrop;
                InventoryManager.instance.confirmationDialog.ShowDialog(itemCollection.transform, msg.title, msg.message, item,
                    (dialog) =>
                    {
                        tempItem.Drop();
                    },
                    (dialog) =>
                    {
                        //Debug.Log("No clicked");
                    });
            }
            else
            {
                item.Drop();
            }
        }

        public override void TriggerUse()
        {
            if (item == null)
                return;

            // Avoid reference using something from other collection that doesn't allow it.
            if (itemCollection.useReferences && item.itemCollection.canUseItemsFromReference == false)
                return;

            item.Use();

            if (itemCollection.useReferences)
                itemCollection.NotifyReferenceUsed(item, item.ID, index, 1);

            Repaint();
        }
    
        #endregion


        /// <summary>
        /// Repaints the item icon and amount.
        /// </summary>
        public override void Repaint()
        {
            if (item != null)
            {
                if (amountText != null)
                {
                    // Only show when we have more then 1 item.
                    if (item.currentStackSize > 1)
                    {
                        amountText.text = item.currentStackSize.ToString();
                        SetActive(amountText, true);
                    }
                    else
                    {
                        amountText.text = string.Empty;
                        SetActive(amountText, false);
                    }
                }

                if (itemName != null)
                {
                    itemName.text = item.name;
                    SetActive(itemName, true);
                }

                if (icon != null)
                {
                    icon.sprite = item.icon;
                    SetActive(icon, true);
                }
            }
            else
            {
                if (amountText != null)
                {
                    SetActive(amountText, false);
                }

                if (itemName != null)
                {
                    SetActive(itemName, false);
                }

                if (icon != null)
                {
                    icon.gameObject.SetActive(!disableIconWhenEmpty);
                    icon.sprite = _startIcon;
                }
            }
        }

        protected void SetActive(MonoBehaviour b, bool set)
        {
            // Check to avoid GC
            if (b.gameObject.activeSelf != set)
            {
                b.gameObject.SetActive(set);
            }
        }

        public virtual void RepaintCooldown()
        {
            if (item != null)
            {
                if(item.isInCooldown)
                {
                    cooldownVisualizer.Repaint((1.0f - item.cooldownFactor) * item.cooldownTime, item.cooldownTime);
                    return;
                }
            }

            cooldownVisualizer.HideAll();
//            cooldownVisualizer.Repaint(0.0f, 1.0f);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            currentlyHoveringSlot = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerDownOnUIElement = null;
            if (currentlyHoveringSlot == this)
            {
                currentlyHoveringSlot = null;
            }
        }
    }
}