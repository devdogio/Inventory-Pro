using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine;

namespace Devdog.InventoryPro.UI
{
    using UnityEngine.Serialization;

    [Obsolete("Use Devdog.General.UI.UIWindow instead", true)]
    [ReplacedBy(typeof(UIWindow))]
    public partial class UIWindowDeprecated : MonoBehaviour
    {
        [System.Serializable]
        public class UIWindowActionEvent : UnityEngine.Events.UnityEvent
        {

        }


        [Header("Behavior")]
        public string windowName = "MyWindow";
        
        /// <summary>
        /// Should the window be hidden when the game starts?
        /// </summary>
        public bool hideOnStart = true;

        /// <summary>
        /// Set the position to 0,0 when the game starts
        /// </summary>
        public bool resetPositionOnStart = true;



        /// <summary>
        /// The animation played when showing the window, if null the item will be shown without animation.
        /// </summary>
        [Header("Audio & Visuals")]
        [SerializeField]
        [FormerlySerializedAs("showAnimation")]
        private AnimationClip _showAnimation;
        public int showAnimationHash { get; protected set; }

        /// <summary>
        /// The animation played when hiding the window, if null the item will be hidden without animation. 
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("hideAnimation")]
        private AnimationClip _hideAnimation;
        public int hideAnimationHash { get; protected set; }

        public AudioClipInfo showAudioClip;
        public AudioClipInfo hideAudioClip;



        [Header("Actions")]
        public UIWindowActionEvent onShowActions = new UIWindowActionEvent();
        public UIWindowActionEvent onHideActions = new UIWindowActionEvent();


        /// <summary>
        /// Is the window visible or not? Used for toggling.
        /// </summary>
        public bool isVisible { get; protected set; }

        private IEnumerator _animationCoroutine;


        private List<UIWindowPage> _pages;
        public List<UIWindowPage> pages
        {
            get
            {
                if (_pages == null)
                    _pages = new List<UIWindowPage>();

                return _pages;
            }
            protected set
            {
                _pages = value;
            }
        }



        public UIWindowPage currentPage
        {
            get;
            set;
        }

        private Animator _animator;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        private RectTransform _rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }
    }
}
