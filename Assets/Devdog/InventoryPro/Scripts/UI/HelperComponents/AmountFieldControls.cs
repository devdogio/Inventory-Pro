using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    public class AmountFieldControls : MonoBehaviour
    {
        public int minAmount = 1;
        public int maxAmount = 999;

        [SerializeField]
        private Button _minButton;

        [SerializeField]
        private Button _plusButton;

        [SerializeField]
        private InputField _inputField;

        public int amount
        {
            get { return int.Parse(_inputField.text); }
        }

        protected virtual void Awake()
        {
            if (_minButton != null)
                _minButton.onClick.AddListener(MinusClicked);

            if (_plusButton != null)
                _plusButton.onClick.AddListener(PlusClicked);
        }

        public void Set(int min, int max)
        {
            this.minAmount = min;
            this.maxAmount = max;

            ValidateAmount();
        }

        protected virtual void MinusClicked()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _inputField.text = (amount - 10).ToString();
            else
                _inputField.text = (amount - 1).ToString();

            ValidateAmount();
        }

        protected virtual void PlusClicked()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _inputField.text = (amount + 10).ToString();
            else
                _inputField.text = (amount + 1).ToString();

            ValidateAmount();
        }

        protected virtual void ValidateAmount()
        {
            _inputField.text = Mathf.RoundToInt(Mathf.Clamp(amount, minAmount, maxAmount)).ToString();
        }
    }
}
