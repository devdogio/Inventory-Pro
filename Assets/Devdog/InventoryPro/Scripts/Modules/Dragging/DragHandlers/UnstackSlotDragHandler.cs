using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    public class UnstackSlotDragHandler : StandardSlotDragHandler
    {

        public UnstackSlotDragHandler(int priority)
            : base(priority)
        {
            dragModel = new UIDragModel();
        }

        public override bool CanUse(ItemCollectionSlotUIBase wrapper, PointerEventData eventData)
        {
            return InventorySettingsManager.instance.settings.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.All);
        }
    }
}