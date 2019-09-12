using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// A single row in the infobox.
    /// </summary>
    public partial class InventoryCraftingBlueprintUI : MonoBehaviour, IPoolable
    {
        [SerializeField]
        protected UnityEngine.UI.Text blueprintName;

        [SerializeField]
        protected UnityEngine.UI.Text blueprintDescription;

        [SerializeField]
        protected UnityEngine.UI.Image blueprintIcon;

        [Required]
        public UnityEngine.UI.Button button;

    
        public virtual void Repaint(CraftingBlueprint blueprint)
        {
            if(blueprintName != null)
                blueprintName.text = blueprint.name;

            if (blueprintDescription != null)
                blueprintDescription.text = blueprint.description;

            if (blueprintIcon != null && blueprint.resultItems.Length > 0)
                blueprintIcon.sprite = blueprint.resultItems[0].item.icon;
        }

        public void ResetStateForPool()
        {
            button.onClick.RemoveAllListeners();
            // Item has no specific states, no need to reset
        }
    }
}