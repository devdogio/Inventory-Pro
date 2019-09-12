using System;
using UnityEngine;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine.Serialization;

namespace Devdog.InventoryPro
{
    [Obsolete("Replaced by Trigger and TriggerBase", true)]
    [ReplacedBy(typeof(Trigger))]
    public partial class ObjectTriggerer : ObjectTriggererBase
    {
        [Serializable]
        public struct ItemBoolPair
        {
            public ItemAmountRow item;
            public bool remove;
        }

        [SerializeField]
        private bool _triggerMouseClick = true;
        public override bool triggerMouseClick
        {
            get { return _triggerMouseClick; }
            set { _triggerMouseClick = value; }
        }

        [SerializeField]
        private KeyCode _triggerKeyCode = KeyCode.None;
        public override KeyCode triggerKeyCode
        {
            get { return _triggerKeyCode; }
            set { _triggerKeyCode = value; }
        }

        /// <summary>
        /// When true the window will be triggered directly, if false, a 2nd party will have to handle it through events.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public bool handleWindowDirectly = true;

        /// <summary>
        /// Toggle when triggered
        /// </summary>
        public bool toggleWhenTriggered = true;

        /// <summary>
        /// Only required if handling the window directly
        /// </summary>
        [Header("The window")]
        [FormerlySerializedAs("window")]
        [SerializeField]
        private UIWindowField _window;
        public UIWindowField window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }

        public override CursorIcon cursorIcon
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override Sprite uiIcon
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        [Header("Requirements")]
        public ItemBoolPair[] requiredItems = new ItemBoolPair[0];
        public StatRequirement[] statRequirements = new StatRequirement[0];

        [Header("Animations & Audio")]
        public AnimationClip useAnimation;
        public AnimationClip unUseAnimation;

        public AudioClipInfo useAudioClip;
        public AudioClipInfo unUseAudioClip;
    }
}