using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    [RequireComponent(typeof(Outline))]
    [RequireComponent(typeof(ItemCollectionSlotUIBase))]
    public partial class DraggingOutline : MonoBehaviour
    {
        protected Outline outline { get; set; }

        protected ItemCollectionSlotUIBase slot { get; private set; }
        protected EquippableSlot equippableSlot { get; private set; }

        public virtual void Start()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;

            slot = GetComponent<ItemCollectionSlotUIBase>();
            equippableSlot = GetComponent<EquippableSlot>();

            InventoryUIDragUtility.OnStartDragging += InventoryUiDragUtilityOnOnStartDragging;
            InventoryUIDragUtility.OnEndDragging += InventoryUiDragUtilityOnOnEndDragging;
        }

        protected virtual void SetOutlineValues()
        {
            if(outline != null)
                outline.enabled = true;
        }

        protected virtual void RemoveOutlineValues()
        {
            if(outline != null)
                outline.enabled = false;
        }

        protected virtual void InventoryUiDragUtilityOnOnStartDragging(UIDragModel dragModel, ItemCollectionSlotUIBase dragging, PointerEventData eventData)
        {
            if (dragging.item != null)
            {
                if (slot != null)
                {
                    // Equippable character field
                    if (equippableSlot != null)
                    {
                        var equippable = dragging.item as EquippableInventoryItem;
                        if (equippable != null)
                        {
                            if (equippableSlot.equipmentTypes.Any(o => o == equippable.equipmentType))
                            {
                                SetOutlineValues();
                            }
                        }
                    }
                    else
                    {
                        if (slot.itemCollection != null)
                        {
                            var canSet = slot.itemCollection.filters.IsItemAbidingFilters(dragging.item);
                            if (canSet)
                            {
                                SetOutlineValues();
                            }
                        }
                    }
                }
            }
        }


        protected virtual void InventoryUiDragUtilityOnOnEndDragging(UIDragModel dragModel, ItemCollectionSlotUIBase dragging, PointerEventData eventData)
        {
            RemoveOutlineValues();
        }

    }
}
