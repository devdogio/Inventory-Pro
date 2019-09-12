using System;
using System.Collections.Generic;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Managers/Inventory Player manager")]
    public class InventoryPlayerManager : ManagerBase<InventoryPlayerManager>
    {
        protected static InventoryPlayer previousPlayer;
        protected virtual void RemovePlayerCollections(InventoryPlayer playerBefore)
        {
            foreach (var inv in playerBefore.inventoryCollections)
                InventoryManager.RemoveInventoryCollection(inv);

            if (playerBefore.characterUI != null)
                InventoryManager.RemoveEquipCollection(playerBefore.characterUI);
        }

        public virtual void SetPlayerAsCurrentPlayer(InventoryPlayer player)
        {
            if (previousPlayer != null)
            {
                RemovePlayerCollections(previousPlayer);
            }

            foreach (var inv in player.inventoryCollections)
            {
                var i = inv as ICollectionPriority;
                InventoryManager.AddInventoryCollection(inv, i != null ? i.collectionPriority : 50);
            }

            var eq = player.characterUI as ICollectionPriority;
            InventoryManager.AddEquipCollection(player.characterUI, eq != null ? eq.collectionPriority : 50);
            previousPlayer = player;
        }
    }
}
