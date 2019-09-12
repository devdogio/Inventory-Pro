using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// A physical representation of a crafting station.
    /// </summary>
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Triggers/Crafting standard trigger")]
    public class CraftingStandardTrigger : CraftingTriggerBase<CraftingWindowStandardUI>
    {
        protected override void SetWindow()
        {
            if (InventoryManager.instance.craftingStandard == null)
            {
                Debug.LogWarning("Crafting trigger in scene, but no crafting window found", transform);
                return;
            }

            craftingWindow = InventoryManager.instance.craftingStandard;
        }
    }
}