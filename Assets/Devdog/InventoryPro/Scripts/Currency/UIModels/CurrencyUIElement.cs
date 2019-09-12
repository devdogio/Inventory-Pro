using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventoryPro
{
    [Serializable]
    public struct CurrencyUIElement
    {
        [Header("Options")]
        [SerializeField]
        private bool _overrideStringFormat;

        [SerializeField]
        private string _overrideStringFormatString;

        [SerializeField]
        private bool _hideWhenEmpty;

        [Header("Audio & Visuals")]
        [SerializeField]
        private Text _amount;

        [SerializeField]
        private Image _icon;

        

        public void Repaint(CurrencyDecorator decorator)
        {
            SetActive(true);

            if (_amount != null)
            {
                if(decorator.amount <= 0f && _hideWhenEmpty)
                {
                    _amount.gameObject.SetActive(false);
                }

                _amount.text = decorator.ToString(1.0f, _overrideStringFormat ? _overrideStringFormatString : "");
            }

            if (_icon != null)
            {
                if(decorator.amount <= 0f && _hideWhenEmpty)
                {
                    _icon.gameObject.SetActive(false);
                }

                _icon.sprite = decorator.currency.icon;
            }
        }

        public void Reset()
        {
            if (_amount != null)
                _amount.text = "0";

            if(_hideWhenEmpty)
            {
                SetActive(false);
            }
        }

        private void SetActive(bool active)
        {
            if (_amount != null)
                _amount.gameObject.SetActive(active);

            if (_icon != null)
                _icon.gameObject.SetActive(active);
        }
    }
}
