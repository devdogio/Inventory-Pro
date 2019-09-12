using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Devdog.InventoryPro.Dialogs
{
    public enum ItemBuySellDialogAction
    {
        Selling,
        Buying,
        BuyingBack
    }


    public partial class ItemBuySellDialog : ItemIntValDialog
    {
        public UnityEngine.UI.Text price;
        protected ItemBuySellDialogAction action;
        protected VendorTrigger vendor;

        [Header("Audio & Visuals")]
        public Color affordableColor = Color.white;
        public Color unAffordableColor = Color.red;
    

        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            base.ShowDialog(caller, title, description, minValue, maxValue, yesCallback, noCallback);
            OnAmountValueChanged(inputField.text);
        }

        protected override void OnAmountValueChanged(string amountString)
        {
            base.OnAmountValueChanged(amountString);

            int amount = GetInputValue(); // Let's trust Unity on this...

            string formattedText;
            float finalPrice;
            CurrencyDefinition currencyDef;
            GetTransactionInfo(amount, out formattedText, out finalPrice, out currencyDef);

            price.text = formattedText;
            if (currencyDef != null)
            {
                SetPriceColor(finalPrice, currencyDef);
            }
        }


        private void SetPriceColor(float finalPrice, CurrencyDefinition currencyDefinition)
        {
            if (action == ItemBuySellDialogAction.Buying || action == ItemBuySellDialogAction.BuyingBack)
            {
                if (InventoryManager.CanRemoveCurrency(currencyDefinition, finalPrice, true))
                    price.color = affordableColor;
                else
                    price.color = unAffordableColor;
            }
            else
                price.color = affordableColor;
        }

        private void GetTransactionInfo(int amount, out string formattedText, out float finalPrice, out CurrencyDefinition finalCurrency)
        {
            if (action == ItemBuySellDialogAction.Buying)
            {
                formattedText = inventoryItem.buyPrice.ToString(amount * vendor.buyPriceFactor);
                finalPrice = vendor.GetBuyPrice(inventoryItem, (uint)amount);
                finalCurrency = inventoryItem.buyPrice.currency;
                return;
            }

            if (action == ItemBuySellDialogAction.Selling)
            {
                formattedText = inventoryItem.sellPrice.ToString(amount * vendor.sellPriceFactor);
                finalPrice = vendor.GetSellPrice(inventoryItem, (uint)amount);
                finalCurrency = inventoryItem.sellPrice.currency;
                return;
            }

            if (action == ItemBuySellDialogAction.BuyingBack)
            {
                formattedText = inventoryItem.sellPrice.ToString(amount * vendor.buyBackPriceFactor);
                finalPrice = vendor.GetBuyBackPrice(inventoryItem, (uint)amount);
                finalCurrency = inventoryItem.sellPrice.currency;
                return;
            }

            formattedText = "";
            finalPrice = 0f;
            finalCurrency = null;
        }

        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            inventoryItem = item;
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }

        public virtual void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, ItemBuySellDialogAction action, VendorTrigger vendor, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            // Don't call base class going directly to this.ShowDialog()
            inventoryItem = item;
            this.action = action;
            this.vendor = vendor;
            ShowDialog(caller, string.Format(string.Format(title, item.name, item.description)), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }
    }
}
