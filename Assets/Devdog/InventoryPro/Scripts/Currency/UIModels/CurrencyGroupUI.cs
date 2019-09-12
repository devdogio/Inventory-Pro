using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro.UI
{
    public partial class CurrencyGroupUI : MonoBehaviour
    {
        private CurrencyUI[] _currencies;
        private CurrencyUI[] currencies
        {
            get
            {
                if (_currencies == null)
                    _currencies = GetComponentsInChildren<CurrencyUI>(true);

                return _currencies;
            }
        }

        public void Repaint(CurrencyDecorator amount)
        {
            // TODO: Maybe add fractions conversion to lower values, as done in main system (1.12 silver is 1 silver and 12 copper).

            foreach (var currencyUI in currencies)
            {
                if (currencyUI.currency == null)
                {
                    Debug.LogWarning("Empty currencyUIElement in group ", currencyUI.transform);
                    continue;
                }

                if (currencyUI.currency == amount.currency)
                {
                    currencyUI.currencyUIElement.Repaint(amount);
                }
                else
                {
                    currencyUI.currencyUIElement.Reset();
                }
            }
        }
    }
}
