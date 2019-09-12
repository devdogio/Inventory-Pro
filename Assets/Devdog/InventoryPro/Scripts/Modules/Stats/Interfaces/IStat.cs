using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public interface IStat
    {
        IStatDefinition definition { get; }

        /// <summary>
        /// The factor by which the value is multiplied.
        /// </summary>
        float currentFactor { get; }

        /// <summary>
        /// The factor by which the max value is multiplied.
        /// </summary>
        float currentFactorMax { get; }

        /// <summary>
        /// The current value of this stat. (baseValue + currentValueRaw) * currentFactor * currentFactorMaxValue
        /// </summary>
        float currentValue { get; }
        
        /// <summary>
        /// The current value without the factor and base value applied.
        /// </summary>
        float currentValueRaw { get; }
        float currentValueNormalized { get; }
        float currentMaxValue { get; }

        float currentExperience { get; }
        int currentLevelIndex { get; }
        StatLevel currentLevel { get; }

        event Action<IStat> OnValueChanged;
        event Action<IStat> OnLevelChanged;
        event Action<IStat> OnExperienceChanged;

        void Reset();



        void SetLevel(int index, bool setMaxValueToLevelMaxValue, bool fireEvents = true);
        void IncreaseLevel(bool setMaxValueToLevelMaxValue, bool fireEvents = true);
        void DecreaseLevel(bool setMaxValueToLevelMaxValue, bool fireEvents = true);
        void ChangeExperience(float experience, bool fireEvents = true);
        void SetExperience(float experience, bool fireEvents = true);


        /// <summary>
        /// The raw value is the value before any other transmutations ( base value and factors ).
        /// </summary>
        void SetCurrentValueRaw(float value, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        void ChangeCurrentValueRaw(float value, bool fireEvents = true);

        /// <summary>
        ///  Factor allows you to set a multiplier for the actual value. Default is 1.0
        /// </summary>
        void SetFactor(float value, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        void ChangeFactor(float value, bool fireEvents = true);


        /// <summary>
        /// Factor max allows you to set a multiplier for the maximum value. Default is 1.0
        /// </summary>
        void SetFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        void ChangeFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// The max value raw is the value before any transmutations ( base value and factors )
        /// </summary>
        void SetMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true);

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        void ChangeMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true);
    }
}
