using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class InventoryActionInput
    {
        public enum MobileUIActions
        {
            LongTap,
            DoubleTap,
            SingleTap,
            None
        }

        public enum EventType
        {
            OnPointerUp,
            OnPointerDown,
            None,
            All // Use all possible event types when checking.
        }

        // OTHER
        public EventType eventType;

        // DESKTOP
        public PointerEventData.InputButton button;
        public KeyCode keyCode;
        
        // MOBILE
        public bool alwaysTriggerMobileActions = false;
        public MobileUIActions mobileAction;


        public InventoryActionInput(PointerEventData.InputButton button, EventType eventType, KeyCode keyCode, MobileUIActions mobileAction = MobileUIActions.None)
        {
            this.button = button;
            this.keyCode = keyCode;
            this.mobileAction = mobileAction;
            this.eventType = eventType;
        }

        public bool AllPressed(PointerEventData data, EventType eventUsed)
        {
            return AllPressed(MobileUIActions.None, eventUsed, data);
        }

        public bool AllPressed(MobileUIActions actionPerformed, EventType eventUsed, PointerEventData data)
        {
            // Not the right event
            if ((eventUsed != eventType || eventUsed == EventType.None) && eventUsed != EventType.All)
                return false;

            if (alwaysTriggerMobileActions || Application.isMobilePlatform)
            {
                if (actionPerformed == mobileAction)
                    return true;

            }

            if (data == null || data.button != button)
                return false;

            if (keyCode != KeyCode.None && Input.GetKey(keyCode) == false)
                return false;

            return true;
        }
    }
}
