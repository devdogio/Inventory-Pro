using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class StatDecorator : IEquatable<StatDecorator>
    {
        public enum ActionEffect
        {
            /// <summary>
            /// Add to the bonus, increases the maximum
            /// </summary>
            Add,

            /// <summary>
            /// Add to the bonus, increases the maximum
            /// </summary>
            AddExperience,

            ///// <summary>
            ///// Add to the maximum value
            ///// </summary>
            //IncreaseMax,

            /// <summary>
            /// Restore the value (for example consumables, when eating an apple, restore the health).
            /// </summary>
            Restore,

            /// <summary>
            /// Decrease the value by a set amount, if the user doesn't have enough of the property the action will be canceled.
            /// </summary>
            Decrease
        }

        [SerializeField]
        [Required]
        private StatDefinition _stat;
        public StatDefinition stat
        {
            get { return _stat; }
            set { _stat = value; }
        }

        public string value;
        /// <summary>
        /// (1 = value * 1.0f, 0.1f = value * 0.1f so 10%).
        /// </summary>
        public bool isFactor = false;

        //public bool increaseMax = false; // Increase max or add to?
        public ActionEffect actionEffect = ActionEffect.Restore;


        public int intValue
        {
            get
            {
                int v = 0;
                Int32.TryParse(value, out v);

                return v;
            }
            set { this.value = value.ToString(); }
        }

        public float floatValue
        {
            get
            {
                float v = 0.0f;
                Single.TryParse(value, out v);

                return v;
            }
            set { this.value = value.ToString(); }
        }

        public bool isSingleValue
        {
            get
            {
                float v;
                return Single.TryParse(value, out v);
            }
        }

        public string stringValue
        {
            get
            {
                return value;
            }
            set { this.value = value; }
        }

        public bool boolValue
        {
            get
            {
                return Boolean.Parse(value);
            }
            set { this.value = value ? "true" : "false"; }
        }


        public StatDecorator()
        {
            
        }

        public StatDecorator(StatDecorator copyFrom)
        {
            this.stat = copyFrom.stat;
            this.value = copyFrom.value;
            this.isFactor = copyFrom.isFactor;
            this.actionEffect = copyFrom.actionEffect;
        }

        public bool CanDoDecrease(InventoryPlayer player)
        {
            var prop = player.stats.Get(stat.category, stat.statName);
            if (prop != null)
            {
                if (prop.currentValue >= floatValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Equals(StatDecorator other)
        {
            if (other == null)
            {
                return false;
            }

            if (stat == null && other.stat == null)
            {
                return true;
            }

            if (stat == null || other.stat == null)
            {
                return false;
            }

            if (stat.Equals(other.stat) == false)
            {
                return false;
            }

            return value == other.value &&
                   isFactor == other.isFactor &&
                   actionEffect == other.actionEffect;
        }

        public override string ToString()
        {
            return stat.ToString(floatValue);
        }
    }
}
