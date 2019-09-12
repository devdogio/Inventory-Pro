using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    using Devdog.InventoryPro;

    [Obsolete("REPLACED BY TRIGGER", true)]
    public abstract class ObjectTriggererBase : MonoBehaviour
    {
        /// <summary>
        /// When the item is clicked, should it trigger?
        /// </summary>
        public abstract bool triggerMouseClick { get; set; }

        /// <summary>
        /// When the item is hovered over (center of screen) and a certain key is tapped, should it trigger?
        /// </summary>
        public abstract KeyCode triggerKeyCode { get; set; }

        /// <summary>
        /// The cursor icon used to visualize this triggerer.
        /// </summary>
        public abstract CursorIcon cursorIcon { get; set; }

        /// <summary>
        /// The icon used inside the UI
        /// </summary>
        public abstract Sprite uiIcon { get; set; }

        /// <summary>
        /// Is this triggerer currently active (used)
        /// </summary>
        public bool isActive { get; protected set; }

        /// <summary>
        /// When true the triggerer will be usable, when false it won't respond to any actions.
        /// </summary>
        public new bool enabled { get; set; }
    }
}
