using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General.UI;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [HelpURL("http://devdog.nl/documentation/settings-menu/")]
    [RequireComponent(typeof(UIWindow))]
    public partial class SettingsMenuUI : MonoBehaviour
    {
        /// <summary>
        /// When true, each time the "keyCode" is pressed a window is hidden. When there are no longer any interactive windows visible the settings menu will be shown.
        /// When false, all windows will be hidden.
        /// </summary>
        public bool hideSingleWindowAtATime = true;

        /// <summary>
        /// When the settings menu is closed should the previously hidden windows be restored?
        /// </summary>
        public bool restoreWindowsAfterClose = true;

        [NonSerialized]
        private readonly List<UIWindow> _hiddenWindows = new List<UIWindow>();

        [NonSerialized]
        private UIWindow[] _interactiveWindowsInScene = new UIWindow[0];

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


        public void Start()
        {
            _interactiveWindowsInScene = UIWindowUtility.GetAllWindowsWithInput();

            window.OnShow += OnShowSettingsWindow;
            window.OnHide += OnHideSettingsWindow;
        }

        protected virtual void OnHideSettingsWindow()
        {
            if (restoreWindowsAfterClose)
            {
                RestoreInteractiveWindows();
            }
        }

        protected virtual void OnShowSettingsWindow()
        {
            HideInteractiveWindows();
        }

        public virtual void HideInteractiveWindows()
        {
            // Show menu, hide current interactive hiddenWindows
            foreach (var w in _interactiveWindowsInScene)
            {
                if (w == window)
                    continue;

                if (w.isVisible)
                {
                    w.Hide();
                    _hiddenWindows.Add(w);

                    if (hideSingleWindowAtATime)
                        return;

                }
            }
        }

        public virtual void RestoreInteractiveWindows()
        {
            foreach (var w in _hiddenWindows)
            {
                w.Show();
            }

            _hiddenWindows.Clear();
        }
    }
}
