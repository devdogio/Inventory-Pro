using System;
using UnityEngine;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine.EventSystems;

namespace Devdog.InventoryPro.UI
{
    [Obsolete("Use Devdog.General.UI.DraggableWindow instead")]
    [ReplacedBy(typeof(DraggableWindow))]
    public partial class DraggableWindowDeprecated : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler
    {
        [Header("Dragging")]
        public float dragSpeed = 1.0f;


        [Header("Bounds")]
        public bool preventFromGoingOutOfBounds = true;
        public Vector2 spacing = new Vector2(100.0f, 100.0f);

        /// <summary>
        /// Once clicked should this draggable window be moved to the foreground?
        /// </summary>
        [Header("Bring to foreground")]
        public bool onClickBringToForeground = true;

        /// <summary>
        /// When the window is shown, should we bring this element to the foreground?
        /// </summary>
        public bool onWindowShowBringToForeground = true;

        /// <summary>
        /// The max sibling index this window can get when bringing it to the foreground.
        /// </summary>
        public int maxForegroundIndex = 10;

        private Vector2 _dragOffset;
        private Vector2 _maxPos;
        private Vector2 _minPos;
        private RectTransform _rectTransform;

        /// <summary>
        /// The window that is currently "focused" / on top.
        /// </summary>
        public static DraggableWindowDeprecated currentWindow { get; protected set; }


        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            set { _window = value; }
        }



        public void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            var w = _rectTransform.rect.width * _rectTransform.pivot.x;
            var h = _rectTransform.rect.height * _rectTransform.pivot.y;
            _minPos = new Vector2(w - spacing.x, h - spacing.y);
            _maxPos = new Vector2(Screen.width + w - spacing.x, Screen.height + h - spacing.y);

            if (onWindowShowBringToForeground)
            {
                window.OnShow += MoveToForeground;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragOffset = new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y) - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition = new Vector3(eventData.position.x + _dragOffset.x * dragSpeed, eventData.position.y + _dragOffset.y * dragSpeed, 0.0f);

            if (preventFromGoingOutOfBounds)
            {
                _rectTransform.position = Clamp(_rectTransform.position, _minPos, _maxPos);
            }
        }

        private Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
        {
            a.x = Mathf.Clamp(a.x, min.x, max.x);
            a.y = Mathf.Clamp(a.y, min.y, max.y);
            
            return a;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onClickBringToForeground)
            {
                MoveToForeground();
            }
        }

        /// <summary>
        /// Move this draggable window back by 1.
        /// </summary>
        public virtual void MoveBack()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        }

        /// <summary>
        /// Move this draggable window all the way back.
        /// </summary>
        public virtual void MoveToBackground()
        {
            transform.SetAsFirstSibling();
        }

        /// <summary>
        /// Move this draggable window up by 1.
        /// </summary>
        public virtual void MoveUp()
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }

        /// <summary>
        /// Bring this draggable window all the way to the front (maxSiblingIndex)
        /// </summary>
        public virtual void MoveToForeground()
        {
            if (currentWindow == this)
                return; // Already top window.

            transform.SetSiblingIndex(maxForegroundIndex);
            currentWindow = this;
        }
    }
}