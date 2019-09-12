using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    public partial class CurrencyUI : MonoBehaviour
    {
        public ItemCollectionBase collection;

        [SerializeField]
        [Required]
        private CurrencyDefinition _currency;
        public CurrencyDefinition currency
        {
            get { return _currency; }
            protected set { _currency = value; }
        }

        public CurrencyUIElement currencyUIElement;

        protected virtual void Awake()
        {
            currencyUIElement.Reset();
        }

        protected virtual void Start()
        {
            if (collection != null)
            {
                collection.OnCurrencyChanged += OnCurrencyChanged;
                currencyUIElement.Repaint(collection.currenciesGroup.GetCurrency(currency));
            }
        }

        protected virtual void OnDestroy()
        {
            if (collection != null)
            {
                collection.OnCurrencyChanged -= OnCurrencyChanged;
            }
        }

        protected virtual void OnCurrencyChanged(float amountBefore, CurrencyDecorator decorator)
        {
            if (decorator.currency == currency)
            {
                currencyUIElement.Repaint(decorator);
            }
        }
    }
}
