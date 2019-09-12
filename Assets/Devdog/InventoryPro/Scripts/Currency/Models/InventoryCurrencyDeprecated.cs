using System;
using UnityEngine;

namespace Devdog.InventoryPro
{
    using Devdog.General.ThirdParty.UniLinq;

    [System.Serializable]
    [Obsolete("Replaced by Currency")]
	public partial class InventoryCurrencyDeprecated
	{
        public uint ID;
		public string singleName = "";
        public string pluralName = "";
        public string description = "";

	    public Sprite icon;

        [Tooltip("How should the string be shown?\n{0} = The amount\n{1} = The currency name")]
        public string stringFormat = "{0} {3}";


        public CurrencyConversion[] currencyConversions = new CurrencyConversion[0];

        /// <summary>
        /// True if we can we get 0.1 gold (fraction), when false only ints are allowed.
        /// </summary>
        public bool allowFractions = true;
        

        /// <summary>
        /// Usefull when you want to "cap" a currency.
        /// For example in your game contains copper, silver and gold. When copper reaches 100 it can be converted to 1 silver.
        /// </summary>
        public bool autoConvertOnMax = false;
        public float autoConvertOnMaxAmount = 1000f;
        public uint autoConvertOnMaxCurrencyID;
        public CurrencyDefinition autoConvertOnMaxCurrency
        {
            get
            {
                return ItemManager.database.currencies.FirstOrDefault(o => o.ID == autoConvertOnMaxCurrencyID);
            }
        }

        public bool autoConvertFractions = true;
        public uint autoConvertFractionsToCurrencyID;
        public CurrencyDefinition autoConvertFractionsToCurrency
        {
            get
            {
                return ItemManager.database.currencies.FirstOrDefault(o => o.ID == autoConvertFractionsToCurrencyID);
            }
        }


        /// <summary>
        /// Convert this currency to the amount given ID.
        /// </summary>
        /// <param name="currencyID"></param>
        /// <returns></returns>
        public float ConvertTo(float amount, CurrencyDefinition currency)
        {
            foreach (var conversion in currencyConversions)
            {
                if (conversion.currency == currency)
                {
                    return amount * conversion.factor;
                }
            }

            Debug.LogWarning("Conversion not possible no conversion found with: " + currency);
            return 0.0f;
        }

        public string ToString(float amount, float minAmount, float maxAmount, string overrideFormat = "")
        {
            try
            {
                return string.Format(overrideFormat == "" ? stringFormat : overrideFormat, amount, minAmount, maxAmount,
                    amount >= -1.0f - float.Epsilon && amount <= 1.0f + float.Epsilon ? singleName : pluralName);
            }
            catch (Exception)
            {
                // Ignored
            }

            return "(Formatting not valid)";
        }

        public override string ToString()
        {
            return pluralName;
        }
	}
}