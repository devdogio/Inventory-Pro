using System;
using UnityEngine;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;

namespace Devdog.InventoryPro
{

    [System.Serializable]
	public partial class CurrencyDefinition : ScriptableObject, IEquatable<CurrencyDefinition>
    {
        public uint ID;

        [Required]
		public string singleName = "";
        [Required]
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
        public CurrencyDefinition autoConvertOnMaxCurrency;
        
        public bool autoConvertFractions = true;
        public CurrencyDefinition autoConvertFractionsToCurrency;

        public static bool operator ==(CurrencyDefinition a, CurrencyDefinition b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            
            return a.Equals(b);
        }

        public static bool operator !=(CurrencyDefinition a, CurrencyDefinition b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Convert this currency to the amount given ID.
        /// </summary>
        public float ConvertTo(float amount, CurrencyDefinition currency)
        {
            foreach (var conversion in currencyConversions)
            {
                if (conversion.currency == currency)
                {
                    return amount * conversion.factor;
                }
            }

            Debug.LogWarning("Conversion not possible no conversion found with currencyID " + currency.ID);
            return 0.0f;
        }

        public string ToString(float amount, float minAmount, float maxAmount, string overrideFormat = "")
        {
            try
            {
                return string.Format(overrideFormat == "" ? stringFormat : overrideFormat, amount, minAmount, maxAmount, amount >= -1.0f - float.Epsilon && amount <= 1.0f + float.Epsilon ? singleName : pluralName);
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

        public bool Equals(CurrencyDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CurrencyDefinition);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int)ID;
            }
        }
    }
}