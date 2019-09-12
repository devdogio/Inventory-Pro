using System;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// An item in the context menu (visual item)
    /// </summary>
    public partial class InventoryContextMenuRowUI : MonoBehaviour, IPointerClickHandler, IPoolable
    {
        public UnityEngine.UI.Button button;
        public Text text;

        [HideInInspector]
        public InventoryItemBase item;

        public General.AudioClipInfo onUse;

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (onUse == null)
                return;

            button.onClick.AddListener(() =>
            {
                AudioManager.AudioPlayOneShot(onUse);
            });
        }

        public void ResetStateForPool()
        {
            //item = null; // No need to reset
            button.onClick.RemoveAllListeners();
        }
    }
}