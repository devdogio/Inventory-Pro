using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class StatRequirement
    {
        public enum FilterType
        {
            Equal,
            NotEqual,
            GreaterThanOrEqual,
            LessThanOrEqual
        }

        public enum StatValueType
        {
            CurrentValue,
            Level
        }

        [Required]
        public StatDefinition stat;

        public float value;
        public StatValueType statValueType = StatValueType.CurrentValue;
        public FilterType filterType = FilterType.GreaterThanOrEqual;

        public StatRequirement()
        {
            
        }

        public StatRequirement(StatRequirement copyFrom)
        {
            this.stat = copyFrom.stat;
            this.value = copyFrom.value;
            this.filterType = copyFrom.filterType;
        }

        public bool CanUse(IEquippableCharacter character)
        {
            Assert.IsNotNull(character, "IEquippableCharacter object given is null");

            var stat = character.stats.Get(this.stat.category, this.stat.statName);
            if (stat != null)
            {
                switch (statValueType)
                {
                    case StatValueType.CurrentValue:
                        return IsAbbidingFilter(stat.currentValue, value, filterType);
                    case StatValueType.Level:
                        return IsAbbidingFilter(stat.currentLevelIndex + 1, value, filterType);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }

        protected virtual bool IsAbbidingFilter(float statValue, float requiredValue, FilterType filterType)
        {
            switch (filterType)
            {
                case FilterType.GreaterThanOrEqual:
                    return statValue >= requiredValue;

                case FilterType.LessThanOrEqual:
                    return statValue <= requiredValue;

                case FilterType.Equal:
                    return Mathf.Approximately(requiredValue, statValue);

                case FilterType.NotEqual:
                    return Mathf.Approximately(requiredValue, statValue) == false;
            }

            return false;
        }

        public override string ToString()
        {
            return ToString(stat.valueStringFormat);
        }

        public string ToString(string overrideFormat)
        {
            return stat.ToString(value);
        }
    }
}
