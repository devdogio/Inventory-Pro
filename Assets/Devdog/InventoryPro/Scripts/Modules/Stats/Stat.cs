using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// Holds the final stat as well as all the affectors changing the item's behaviour.
    /// </summary>
    public partial class Stat : IStat
    {
        public IStatDefinition definition { get; protected set; }

        /// <summary>
        /// The factor by which the value is multiplied.
        /// </summary>
        public float currentFactor { get; protected set; }

        /// <summary>
        /// The factor by which the max value is multiplied.
        /// </summary>
        public float currentFactorMax { get; protected set; }

        /// <summary>
        /// The current value of this stat. (baseValue + currentValueRaw) * currentFactor * currentFactorMaxValue
        /// </summary>
        public float currentValue
        {
            get
            {
                return Mathf.Clamp(currentValueNotClamped, float.MinValue, currentMaxValue);
            }
        }

        protected float currentValueNotClamped
        {
            get
            {
                return currentValueRaw * currentFactor;
            }
        }


        /// <summary>
        /// The current value without the factor and base value applied.
        /// </summary>
        public float currentValueRaw { get; protected set; }


        public float currentValueNormalized
        {
            get { return currentValue / currentMaxValue; }
        }


        public float currentExperience
        {
            get; protected set;
        }

        public float currentMaxValueRaw { get; protected set; }
        public float currentMaxValue
        {
            get
            {
                return currentMaxValueRaw * currentFactorMax;
            }
        }


        private int _currentLevelIndex;
        public int currentLevelIndex
        {
            get { return _currentLevelIndex; }
            protected set
            {
                var before = _currentLevelIndex;
                _currentLevelIndex = Mathf.Clamp(value, 0, definition.levels.Length - 1);
                if (before != _currentLevelIndex && currentLevel != null)
                {
                    currentLevel.Unlock(this);
                }
            }
        }

        public StatLevel currentLevel
        {
            get
            {
                if (_currentLevelIndex > definition.levels.Length - 1 || definition.levels.Length == 0)
                {
                    return null;
                }

                return definition.levels[_currentLevelIndex];
            }
        }


        public event Action<IStat> OnValueChanged;
        public event Action<IStat> OnLevelChanged;
        public event Action<IStat> OnExperienceChanged;


        // For Generation only
        public Stat() { }

        public Stat(IStatDefinition statDefinition)
        {
            Assert.IsNotNull(statDefinition);

            this.definition = statDefinition;

            currentFactor = 1.0f;
            currentFactorMax = 1.0f;

            SetMaxValueRaw(statDefinition.maxValue, false);
            SetLevel(definition.startLevel, true, false);
            SetCurrentValueRaw(definition.baseValue, false);
        }

        public virtual void Reset()
        {
            SetCurrentValueRaw(definition.baseValue, false);
            SetMaxValueRaw(currentLevel.maxValue, false, true);
        }

        public void SetLevel(int index, bool setMaxValueToLevelMaxValue, bool fireEvents = true)
        {
            var before = currentLevelIndex;
            currentLevelIndex = index;

            if (setMaxValueToLevelMaxValue && currentLevel != null)
            {
                SetMaxValueRaw(currentLevel.maxValue, false, false);
            }

            if (fireEvents && before != _currentLevelIndex)
            {
                NotifyLevelChanged();
            }
        }

        public void IncreaseLevel(bool setMaxValueToLevelMaxValue, bool fireEvents = true)
        {
            SetLevel(currentLevelIndex + 1, setMaxValueToLevelMaxValue, fireEvents);
        }

        public void DecreaseLevel(bool setMaxValueToLevelMaxValue, bool fireEvents = true)
        {
            SetLevel(currentLevelIndex - 1, setMaxValueToLevelMaxValue, fireEvents);
        }

        public void ChangeExperience(float xp, bool fireEvents = true)
        {
            SetExperience(this.currentExperience + xp, fireEvents);
        }

        public void SetExperience(float experience, bool fireEvents = true)
        {
            float diff = experience - this.currentExperience;
            this.currentExperience = experience;
            if (diff > 0f)
            {
                if (currentLevel == null || this.currentExperience >= currentLevel.experienceRequired)
                {
                    SetLevel(GetLevelIndexForExperience(this.currentExperience), true, fireEvents);
                }
            }
            else if (diff < 0f)
            {
                int levelShouldBe = GetLevelIndexForExperience(this.currentExperience);
                if (levelShouldBe != currentLevelIndex)
                {
                    SetLevel(levelShouldBe, true, fireEvents);
                }
            }

            if (fireEvents && Mathf.Approximately(experience, 0f) == false)
            {
                NotifyExperienceChanged();
            }
        }

        private int GetLevelIndexForExperience(float xp)
        {
            int best = -1;
            for (int i = 0; i < definition.levels.Length; i++)
            {
                if (best == -1)
                {
                    best = i;
                    continue;
                }

                if (xp > definition.levels[i].experienceRequired)
                {
                    best = i;
                    continue;
                }

                break;
            }

            return best;
        }

        private void NotifyLevelChanged()
        {
            if (OnLevelChanged != null)
                OnLevelChanged(this);
        }

        private void NotifyExperienceChanged()
        {
            if (OnExperienceChanged != null)
                OnExperienceChanged(this);
        }



        protected void ClampCurrentValueRaw()
        {
            float over = currentValueNotClamped - currentMaxValue;
            if (over > 0.0f)
            {
                currentValueRaw -= over; // "clamp" the currentValue raw, so that currentValueRaw + all other stats hit the maxValueRaw.
            }
        }

        /// <summary>
        /// The raw value is the value before any other transmutations ( base value and factors ).
        /// </summary>
        public void SetCurrentValueRaw(float value, bool fireEvents = true)
        {
            currentValueRaw = value;
            ClampCurrentValueRaw();

            if (fireEvents)
                NotifyValueChanged();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        public void ChangeCurrentValueRaw(float value, bool fireEvents = true)
        {
            SetCurrentValueRaw(currentValueRaw + value, fireEvents);
        }

        /// <summary>
        ///  Factor allows you to set a multiplier for the actual value. Default is 1.0
        /// </summary>
        public void SetFactor(float value, bool fireEvents = true)
        {
            currentFactor = value;
            ClampCurrentValueRaw();

            if (fireEvents)
                NotifyValueChanged();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        public void ChangeFactor(float value, bool fireEvents = true)
        {
            SetFactor(currentFactor + value, fireEvents);
        }


        /// <summary>
        /// Factor max allows you to set a multiplier for the maximum value. Default is 1.0
        /// </summary>
        public void SetFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = currentMaxValue;
            currentFactorMax = value;
            if (andIncreaseCurrentValue)
            {
                float increase = currentMaxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClampCurrentValueRaw();

            if (fireEvents)
                NotifyValueChanged();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        public void ChangeFactorMax(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetFactorMax(currentFactorMax + value, andIncreaseCurrentValue, fireEvents);
        }

        /// <summary>
        /// The max value raw is the value before any transmutations ( base value and factors )
        /// </summary>
        public void SetMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            float prevMax = currentMaxValue;
            currentMaxValueRaw = value;
            if (andIncreaseCurrentValue)
            {
                float increase = currentMaxValue - prevMax;
                ChangeCurrentValueRaw(increase, false); // Updating below..
            }

            ClampCurrentValueRaw();


            if (fireEvents)
                NotifyValueChanged();
        }

        /// <summary>
        /// Change can either be positive or negative.
        /// </summary>
        public void ChangeMaxValueRaw(float value, bool andIncreaseCurrentValue, bool fireEvents = true)
        {
            SetMaxValueRaw(currentMaxValueRaw + value, andIncreaseCurrentValue, fireEvents);
        }

        private void NotifyValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged(this);
        }

        public override string ToString()
        {
            return definition.ToString(this);
        }
    }
}