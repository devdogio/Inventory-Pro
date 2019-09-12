using System;
using UnityEngine;
using System.Collections;
using Devdog.General;
using Devdog.General.UI;
using UnityEngine.UI;

namespace Devdog.InventoryPro.Dialogs
{

    public delegate void InventoryUIDialogCallback(InventoryUIDialogBase dialog);

    /// <summary>
    /// The abstract base class used to create all dialogs. If you want to create your own dialog, extend from this class.
    /// </summary>
    [HelpURL("http://devdog.nl/documentation/dialogs/")]
    [RequireComponent(typeof (Animator))]
    [RequireComponent(typeof (UIWindow))]
    public abstract partial class InventoryUIDialogBase : MonoBehaviour
    {
        [Header("UI")] public Text titleText;
        public Text descriptionText;

        public UnityEngine.UI.Button yesButton;
        public UnityEngine.UI.Button noButton;

        /// <summary>
        /// The item that should be selected by default when the dialog opens.
        /// </summary>
        [Header("Behavior")]
        public bool disableSelectOnOpenDialogOnMobile = true;
        public Selectable selectOnOpenDialog;

        /// <summary>
        /// When enabled the window will be positioned on top of the caller's window.
        /// </summary>
        public bool positionOnTopOfCaller;

        /// <summary>
        /// Disables the items defined in InventorySettingsManager.disabledWhileDialogActive if set to true.
        /// </summary>
        public bool disableElementsWhileActive = true;

        protected CanvasGroup canvasGroup { get; set; }
        protected Animator animator { get; set; }
        public UIWindow window { get; protected set; }

        public UIWindow dialogCallerWindow { get; protected set; }

        private Transform _dialogCaller;
        public Transform dialogCaller
        {
            get { return _dialogCaller; }
            set
            {
                _dialogCaller = value;
                if(dialogCaller != null)
                    dialogCallerWindow = _dialogCaller.GetComponent<UIWindow>();
            }
        }

        public virtual void Awake()
        {
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            animator = GetComponent<Animator>();
            window = GetComponent<UIWindow>();

            window.OnShow += WindowOnShow;
            window.OnHide += WindowOnHide;

            if (yesButton != null)
            {
                yesButton.onClick.AddListener(OnYesButtonClicked);
            }
            if (noButton != null)
            {
                noButton.onClick.AddListener(OnNoButtonClicked);
            }
        }

        public virtual void OnNoButtonClicked()
        {

        }

        public virtual void OnYesButtonClicked()
        {

        }

        protected virtual void WindowOnShow()
        {
            SetEnabledWhileActive(false); // Disable other UI elements
            yesButton.Select();

            if (Application.isMobilePlatform && disableSelectOnOpenDialogOnMobile == false)
            {
                if (selectOnOpenDialog != null)
                    selectOnOpenDialog.Select();

            }
        }

        protected virtual void WindowOnHide()
        {
            SetEnabledWhileActive(true); // Enable other UI elements
        }

        public virtual void Update()
        {
            
        }

        private void OnDialogCallerWindowHidden()
        {
            if(dialogCallerWindow != null)
                dialogCallerWindow.OnHide -= OnDialogCallerWindowHidden;

            if (window.isVisible && dialogCallerWindow != null && dialogCallerWindow.isVisible == false)
                window.Hide();
        }

        public void Toggle()
        {
            window.Toggle();
            SetEnabledWhileActive(!window.isVisible);
        }

        /// <summary>
        /// Disables elements of the UI when a dialog is active. Useful to block user actions while presented with a dialog.
        /// </summary>
        /// <param name="enabled">Should the items be disabled?</param>
        protected virtual void SetEnabledWhileActive(bool enabled)
        {
            
        }

        /// <summary>
        /// Called when a dialog is shown
        /// </summary>
        /// <param name="dialogCaller">The gameObject that is responsible for opening this dialog.</param>
        protected virtual void NotifyDialogShown(Transform dialogCaller)
        {
            this.dialogCaller = dialogCaller;
            if (dialogCallerWindow != null)
            {
                dialogCallerWindow.OnHide += OnDialogCallerWindowHidden;
            }

            if (positionOnTopOfCaller && dialogCaller != null)
            {
                transform.position = dialogCaller.position + (-dialogCaller.forward * 0.5f);
                transform.rotation = dialogCaller.rotation;
            }
        }
    }
}