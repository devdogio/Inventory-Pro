using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    public class MobileSlotDragHandler : StandardSlotDragHandler
    {
        public MobileSlotDragHandler(int priority)
            : base(priority)
        {
            
        }

        public override bool CanUse(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            if (Application.isMobilePlatform)
            {
                if (eventData.pointerId != -1 && Input.touchCount > 0)
                {
                    return true; // Mobile drag
                }
            }

            return false;
        }
    }
}