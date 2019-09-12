using System;
using UnityEngine;
using Devdog.General;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// A physical representation of a crafting station.
    /// </summary>
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Triggers/Crafting layout trigger")]
    public class CraftingLayoutTrigger : CraftingTriggerBase<CraftingWindowLayoutUI>
    {
        protected override void SetWindow()
        {
            if (InventoryManager.instance.craftingLayout == null)
            {
                Debug.LogWarning("Crafting trigger in scene, but no crafting window found", transform);
                return;
            }

            craftingWindow = InventoryManager.instance.craftingLayout;
        }
    }
}