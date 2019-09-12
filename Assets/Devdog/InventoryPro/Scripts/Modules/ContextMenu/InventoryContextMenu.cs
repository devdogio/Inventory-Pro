using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine.UI;
using Devdog.General.UI;
using Devdog.InventoryPro.UI;

namespace Devdog.InventoryPro
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows Other/Context menu")]
    public partial class InventoryContextMenu : MonoBehaviour
    {
        /// <summary>
        /// An option that can be chosen (code only).
        /// </summary>
        public partial class ContextMenuOption : ItemUsability
        {
            public InventoryContextMenuRowUI uiElement;

            public ContextMenuOption(string actionName, Action<InventoryItemBase> useItemCallback, InventoryContextMenuRowUI uiElement)
                : base(actionName, useItemCallback)
            {
                this.uiElement = uiElement;
            }
        }


        private List<ContextMenuOption> _menuOptions = new List<ContextMenuOption>(8);


        [Header("UI")]
        public RectTransform container;
        
        /// <summary>
        /// Single line / menu item inside the context menu.
        /// </summary>
        public InventoryContextMenuRowUI contextMenuItemPrefab;

        /// <summary>
        /// If there is only 1 action in the context menu, trigger it auto.
        /// </summary>
        [Header("Behavior")]
        public bool autoTriggerIfSingleAction = true;

        public bool closeWindowWhenClickedOutside = true;
        public bool positionAtMouse = true;

        private UIWindow _window;
        public virtual UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        private ComponentPool<InventoryContextMenuRowUI> _pool;

        private RectTransform _rectTransform;
        public virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _pool = new ComponentPool<InventoryContextMenuRowUI>(contextMenuItemPrefab, 8);

            window.OnShow += window_OnWindowShow;
        }

        public virtual void Update()
        {
            if (window.isVisible == false)
                return;

            if (Input.GetKeyUp(KeyCode.Mouse0) && closeWindowWhenClickedOutside && Vector2.Distance(Input.mousePosition, transform.position) > 50f)
            {
                if (InventoryUIUtility.currentlyHoveringSlot == null || InventoryUIUtility.currentlyHoveringSlot.item == null)
                {
                    window.Hide();
                }
            }
        }

        private void window_OnWindowShow()
        {
            // The context menu is being shown, update it
            if (positionAtMouse)
                _rectTransform.anchoredPosition = Input.mousePosition;

            if (_menuOptions.Count == 1 && autoTriggerIfSingleAction)
            {
                // Do the default?
                _menuOptions[0].useItemCallback(_menuOptions[0].uiElement.item);
                window.Hide(); // No need for it anymore
            }

            if(_menuOptions.Count > 0)
            {
                _menuOptions[0].uiElement.button.Select();
            }
        }

        public virtual void ClearMenuOptions()
        {
            if (positionAtMouse)
                _rectTransform.anchoredPosition = Input.mousePosition;

            // Remove the old
            foreach (var item in _menuOptions)
            {
                _pool.Destroy(item.uiElement);
                //Destroy(item.uiElement.gameObject);
            }

            _menuOptions.Clear();
        }

        public virtual void AddMenuOption(string name, InventoryItemBase item, Action<InventoryItemBase> callback)
        {
            var obj = _pool.Get();
            obj.transform.SetParent(container);
            InventoryUtility.ResetTransform(obj.transform);

            obj.item = item;
            obj.text.text = name;

            obj.button.onClick.AddListener(() =>
            {
                callback(obj.item);
                window.Hide();
            });

            _menuOptions.Add(new ContextMenuOption(name, callback, obj));
        }
    }
}