using UnityEngine;
using UnityEngine.UI;
using System;
using Devdog.General;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// A single row in the infobox.
    /// </summary>
    public partial class InventoryCraftingCategoryUI : MonoBehaviour, IPoolable
    {
        [SerializeField]
        protected UnityEngine.UI.Text title;

        [SerializeField]
        protected UnityEngine.UI.Image icon;

        [Required]
        public RectTransform container;

        public virtual void Repaint(CraftingCategory category, ItemCategory itemCategory)
        {
            title.text = category.name;

            if (icon != null)
                icon.sprite = itemCategory.icon;
        }

        public void ResetStateForPool()
        {
            // Item has no specific states, no need to reset
        }
    }
}