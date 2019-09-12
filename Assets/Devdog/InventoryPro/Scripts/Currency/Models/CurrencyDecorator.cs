using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [Serializable]
    public partial class CurrencyDecorator
    {
        public delegate void CurrencyChanged(float before, CurrencyDecorator after);
        public event CurrencyChanged OnCurrencyChanged;

        public CurrencyDecoratorCollection parentGroup { get; set; }

        [SerializeField]
        private CurrencyDefinition _currency;
        public CurrencyDefinition currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        [SerializeField]
        private float _amount;
        public virtual float amount
        {
            get { return _amount; }
            set
            {
                float before = _amount;
                //if (currency.allowFractions == false)
                //{
                //    value = Mathf.Floor(value);
                //}

                _amount = value;

                TryAutoRefillCurrency();
                TryAutoConvertCurrency();
                TryAutoConvertFractions();

                if (before != _amount)
                    NotifyOnCurrencyChanged(before, this);
            }
        }
        
        public CurrencyDecorator()
        {
            _amount = 0;
        }

        public CurrencyDecorator(CurrencyDefinition currency)
            : this(currency, 0)
        { }

        public CurrencyDecorator(CurrencyDefinition currency, float startAmount)
        {
            this.currency = currency;
            _amount = startAmount;
        }

        private void TryAutoRefillCurrency()
        {
            if (parentGroup == null)
                return;

            // Value wen't below 0, grab currency from the next one up or down the chain.
            if (_amount < 0f)
            {
                var conversionDict = new Dictionary<CurrencyDefinition, float>();
                _GetConversionFactor(conversionDict, currency, 1f);
                foreach (var lookup in parentGroup.lookups)
                {
                    if (conversionDict.ContainsKey(lookup.currency))
                    {
                        // We can convert to this
                        // How much is it worth in another currency?
                        float amountInOtherCurrency = Mathf.Abs(_amount * conversionDict[lookup.currency]);
                        if (lookup.amount >= amountInOtherCurrency)
                        {
                            _amount = 0f;
                            lookup.amount -= amountInOtherCurrency;
                            return;
                        }
                    }
                }

//                Debug.Log("Trying other method... ");
                foreach (var lookup in parentGroup.lookups)
                {
                    var convertable = lookup.currency.currencyConversions.FirstOrDefault(o => o.currency == currency && o.useInAutoConversion);
                    if (convertable != null)
                    {
                        float amountToGrab = Mathf.Abs(_amount) / convertable.factor;
                        _amount = 0f; // Reset this, grabbing from next currency.
                        lookup.amount -= amountToGrab; // Example: abs(-10) = 10 * 0.01f = 0.1f silver. 
                        return;
                    }
                }
            }
        }

        protected virtual void TryAutoConvertFractions()
        {
            if (parentGroup == null)
                return;

            if (currency.autoConvertFractions)
            {
                double fraction = _amount % 1;
                fraction = System.Math.Round(fraction, 4); // Because of so much conversion float point accuracy is a problem, so round it.
                if (fraction > 0.00001f)
                {
                    var convertTo = parentGroup.GetCurrency(currency.autoConvertFractionsToCurrency);
                    if (convertTo == null)
                    {
                        Debug.LogWarning("Can't convert fractions, container doesn't accept currency type.");
                        return;
                    }

                    var convertToConversionList = currency.currencyConversions.FirstOrDefault(o => o.currency == currency.autoConvertFractionsToCurrency);
                    if (convertToConversionList == null)
                    {
                        Debug.LogError("Can't convert to this type, conversion is not set in conversion list.");
                        return;
                    }

                    _amount = Mathf.Floor(_amount); // Remove our fraction.
                    convertTo.amount += (float)(fraction * convertToConversionList.factor); // Works recursive, calls the next currency that can convert it down.
                }
            }
            else
            {
                if (currency.allowFractions == false)
                {
                    //_amount = Mathf.Round(_amount);
                }
            }
        }

        protected virtual void TryAutoConvertCurrency()
        {
            if (parentGroup == null)
                return;

            if (currency.autoConvertOnMax)
            {
                if (_amount >= currency.autoConvertOnMaxAmount)
                {
                    var convertTo = parentGroup.GetCurrency(currency.autoConvertOnMaxCurrency);
                    if (convertTo == null)
                    {
                        Debug.LogWarning("Can't convert currency, container doesn't accept currency type.");
                        return;
                    }

                    var convertToConversionList = currency.currencyConversions.FirstOrDefault(o => o.currency == currency.autoConvertOnMaxCurrency);
                    if (convertToConversionList == null)
                    {
                        Debug.LogError("Can't convert to this type, conversion is not set in conversion list.");
                        return;
                    }

                    // How many times can we extract it without going below 0? ( 26 / 5 = 5.2 ( floored = 5x ))
                    int canExtractTimes = Mathf.FloorToInt(_amount / currency.autoConvertOnMaxAmount);
                    _amount -= currency.autoConvertOnMaxAmount * canExtractTimes;

                    convertTo.amount += ((canExtractTimes * currency.autoConvertOnMaxAmount) * convertToConversionList.factor); // Works recursive
                }
            }
        }

        private void NotifyOnCurrencyChanged(float before, CurrencyDecorator after)
        {
            if (OnCurrencyChanged != null)
                OnCurrencyChanged(before, after);
        }



        public virtual float CanAddCount()
        {
            return float.MaxValue - amount;
        }

        public virtual float CanRemoveCount(bool allowCurrencyConversions)
        {
            if (allowCurrencyConversions == false)
                return amount;

            if (parentGroup == null)
            {
                Debug.LogWarning("Can't convert currency because this lookup is not in a container. (can't grab other currency amounts)");    
                return amount;
            }

            float totalAmount = amount;
            var dict = new Dictionary<CurrencyDefinition, float>();
            _GetConversionFactor(dict, currency, 1.0f);

            foreach (var tuple in dict)
            {
                if (tuple.Key == currency)
                {
                    // Can't convert to self.
                    continue;
                }

                var lookup = parentGroup.lookups.First(o => o.currency == tuple.Key);
                totalAmount += lookup.amount / tuple.Value; // How much the higher currency is worth in this currency ( gold worth in copper )
            }
            
            return totalAmount;
        }

        /// <summary>
        /// Grab the total factor of converting from a currency to another.
        /// For example copper to silver = 0.01 > silver to gold = 0.01 * 0.01 = 0.0001
        /// </summary>
        /// <returns></returns>
        private float _GetConversionFactor(Dictionary<CurrencyDefinition, float> dict, CurrencyDefinition fromCurrency, float factor)
        {
            foreach (var lookup in parentGroup.lookups)
            {
                // Find currency that converts to our current currency.
                var convertToCurrent = lookup.currency.currencyConversions.FirstOrDefault(o => o.currency == fromCurrency && o.useInAutoConversion);
                if (convertToCurrent != null)
                {
                    if (dict.ContainsKey(lookup.currency))
                    {
                        //Debug.Log("need update?  factor: " + factor + " convertToCurrent factor: " + convertToCurrent.factor + " at " + lookup.currency.pluralName);
                    }
                    else
                    {
                        // One conversion higher
                        factor /= convertToCurrent.factor; // Division because we're going the other way...

                        dict.Add(lookup.currency, factor);
                        return _GetConversionFactor(dict, lookup.currency, factor);
                    }
                }
            }


            return factor;
        }

        public virtual bool CanAdd(float addAmount)
        {
            return CanAddCount() >= addAmount;
        }

        public virtual bool CanRemove(float removeAmount, bool allowCurrencyConversions)
        {
            return CanRemoveCount(allowCurrencyConversions) >= removeAmount;
        }

        /// <summary>
        /// Get the formatted string of this currency lookup.
        /// Adds the current amount and the maxAmount to string.Format.
        /// </summary>
        /// <param name="amountMultiplier">A multiplier to increase the amount. For example a boot costs 5 gold, I sold it 10 times -> a multiplier of 10</param>
        /// <returns></returns>
        public virtual string ToString(float amountMultiplier, string overrideFormat = "")
        {
            if (currency == null)
                return "";

            return currency.ToString(amount * amountMultiplier, float.MinValue, float.MaxValue, overrideFormat);
        }

        public override string ToString()
        {
            return ToString(1.0f);
        }
    }
}