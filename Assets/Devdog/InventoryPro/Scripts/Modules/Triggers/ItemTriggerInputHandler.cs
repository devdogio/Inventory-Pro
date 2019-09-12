using System;
using System.Collections.Generic;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro
{
    [RequireComponent(typeof(ItemTrigger))]
    [DisallowMultipleComponent]
    public class ItemTriggerInputHandler : TriggerInputHandlerBase
    {
        public override TriggerActionInfo actionInfo
        {
            get
            {
                return new TriggerActionInfo()
                {
                    actionName = triggerKeyCode.ToString(),
                    icon = uiIcon
                };
            }
        }
        
        public CursorIcon cursorIcon
        {
            get { return InventorySettingsManager.instance.settings.pickupCursor; }
        }

        public bool triggerMouseClick
        {
            get { return InventorySettingsManager.instance.settings.itemTriggerMouseClick; }
        }

        public KeyCode triggerKeyCode
        {
            get { return InventorySettingsManager.instance.settings.itemTriggerUseKeyCode; }
        }

        public Sprite uiIcon
        {
            get { return InventorySettingsManager.instance.settings.itemTriggerPickupSprite; }
        }

        private ItemTrigger _itemTrigger;
        public ItemTrigger itemTrigger
        {
            get
            {
                if (_itemTrigger == null)
                {
                    _itemTrigger = GetComponent<ItemTrigger>();
                }

                return _itemTrigger;
            }
        }

//        protected override void Update()
//        {
//            base.Update();
//
//            if (TriggerUtility.mouseOnTrigger && UIUtility.isHoveringUIElement == false)
//            {
//                cursorIcon.Enable();
//            }
//        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            cursorIcon.Enable();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (triggerMouseClick)
            {
                Use();
            }
        }

        public override bool AreKeysDown()
        {
            if (triggerKeyCode == KeyCode.None)
            {
                return false;
            }

            return Input.GetKeyDown(triggerKeyCode);
        }

        public override void Use()
        {
            var used = trigger.Use();
            if (used)
            {
                OnPointerExit(new PointerEventData(EventSystem.current));
            }
        }

        public override string ToString()
        {
            return triggerKeyCode.ToString();
        }
    }
}
