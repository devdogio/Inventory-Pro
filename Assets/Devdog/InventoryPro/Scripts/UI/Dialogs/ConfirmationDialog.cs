using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Devdog.InventoryPro.Dialogs
{
    public partial class ConfirmationDialog : InventoryUIDialogBase
    {
        private InventoryUIDialogCallback _yesCallback, _noCallback;

        /// <summary>
        /// Show this dialog.
        /// <b>Don't forget to call dialog.Hide(); when you want to hide it, this is not done auto. just in case you want to animate it instead of hide it.</b>
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="description">The description of the dialog.</param>
        /// <param name="yes">The name of the yes button.</param>
        /// <param name="no">The name of the no button.</param>
        /// <param name="yesCallback"></param>
        /// <param name="noCallback"></param>
        public virtual void ShowDialog(Transform caller, string title, string description, InventoryUIDialogCallback yesCallback, InventoryUIDialogCallback noCallback)
        {
            SetEnabledWhileActive(false);

            window.Show(); // Have to show it first, otherwise we can't use the elements, as they're disabled.

            _yesCallback = yesCallback;
            _noCallback = noCallback;

            titleText.text = title;
            descriptionText.text = description;

            NotifyDialogShown(caller);
        }

        public override void OnYesButtonClicked()
        {
            base.OnYesButtonClicked();
            if (window.isVisible == false)
                return;

            SetEnabledWhileActive(true);
            _yesCallback(this);
            window.Hide();
        }

        public override void OnNoButtonClicked()
        {
            base.OnNoButtonClicked();
            if (window.isVisible == false)
                return;

            SetEnabledWhileActive(true);
            _noCallback(this);
            window.Hide();
        }


        /// <summary>
        /// Show the dialog.
        /// <b>Don't forget to call dialog.Hide(); when you want to hide it, this is not done auto. just in case you want to animate it instead of hide it.</b>
        /// </summary>
        /// <param name="title">The title of the dialog. Note that {0} is the item ID and {1} is the item name.</param>
        /// <param name="description">The description of the dialog. Note that {0} is the item ID and {1} is the item name.</param>
        /// <param name="yes">The name of the yes button.</param>
        /// <param name="no">The name of the no button.</param>
        /// <param name="item">
        /// You can add an item, if you're confirming something for that item. This allows you to use {0} for the title and {1} for the description inside the title and description variables of the dialog.
        /// An example:
        /// 
        /// ShowDialog("Are you sure you want to drop {0}?", "{0} sure seems valuable..", ...etc..);
        /// This will show the item name at location {0} and the description at location {1}.
        /// </param>
        /// <param name="yesCallback"></param>
        /// <param name="noCallback"></param>
        public virtual void ShowDialog(Transform caller, string title, string description, InventoryItemBase item, InventoryUIDialogCallback yesCallback, InventoryUIDialogCallback noCallback)
        {
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), yesCallback, noCallback);
        }
    }
}