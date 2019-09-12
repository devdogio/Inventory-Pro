using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Devdog.InventoryPro
{
    public interface ICollectionItem
    {
        InventoryItemBase item { get; set; }
        ItemCollectionBase itemCollection { get; set; }
        uint index { get; set; }


        void TriggerContextMenu();
        void TriggerUnstack(ItemCollectionBase toCollection, int toIndex = -1);
        void TriggerDrop(bool useRaycast = true);
        void TriggerUse();


        void Repaint();
        void Select();
    }
}