using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    public class StatsCollection : IEnumerable<KeyValuePair<string, List<IStat>>>
    {
        public event Action<IStat> OnStatValueChanged;
         public event Action<IStat> OnStatLevelChanged;
        // public event Action<IStat> OnStatExperienceChanged; // Subscribe to IStat directly ...


        private readonly Dictionary<string, List<IStat>> _stats;
        public List<IStatsProvider> dataProviders { get; protected set; }


        public StatsCollection()
            : this(new List<IStatsProvider>() {  })
        { }

        public StatsCollection(List<IStatsProvider> dataProviders)
        {
            Assert.IsNotNull(dataProviders, "Dataproviders object given is null!");

            _stats = new Dictionary<string, List<IStat>>();
            this.dataProviders = dataProviders;
        }

        public KeyValuePair<string, List<IStat>>? GetCategory(string category)
        {
            if (ContainsCategory(category) == false)
            {
                return null;
            }

            return _stats.FirstOrDefault(o => o.Key == category);
        }


        public bool ContainsCategory(string category)
        {
            return _stats.ContainsKey(category);
        }

        public bool ContainsStat(string category, string name)
        {
            return Get(category, name) != null;
        }

        public IStat Get(IStatDefinition s)
        {
            return Get(s.category, s.statName);
        }

        /// <summary>
        /// Convenience method to grab a stat from this character.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IStat Get(string category, string name)
        {
            if (ContainsCategory(category) == false)
            {
                return null;
            }

            return _stats[category].FirstOrDefault(o => o.definition.statName == name);
        }


        /// <summary>
        /// Add a stat to this character.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="category"></param>
        public void Add(string category, IStat stat)
        {
            if (ContainsCategory(category) == false)
            {
                _stats.Add(category, new List<IStat>());
            }

            if (_stats[category].Any(o => o == stat))
            {
                Debug.LogWarning("Stat with same name already exists in this characterUI - " + stat.definition.statName + " will still be added. Use GetStat() == null to verify if it exists first.");
            }

            stat.OnValueChanged += NotifyStatValueChanged;
            stat.OnLevelChanged += NotifyStatLevelChanged;
//            stat.OnExperienceChanged += NotifyStatExperienceChanged;
            _stats[category].Add(stat);
        }

        /// <summary>
        /// Remove all stats with the given category and name.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns>True if succeded, false if the stat couldn't be removed or found.</returns>
        public bool Remove(string category, string name)
        {
            if (_stats.ContainsKey(category) == false)
            {
                return false;
            }

            var stat = _stats[category].FirstOrDefault(o => o.definition.statName == name);
            if (stat != null)
            {
                stat.OnValueChanged -= NotifyStatValueChanged;
                stat.OnLevelChanged -= NotifyStatLevelChanged;
//                stat.OnExperienceChanged -= NotifyStatExperienceChanged;
                _stats[category].Remove(stat);
                return true;
            }

            return false;
        }


        public void Clear()
        {
            foreach (var cat in _stats)
            {
                foreach (var stat in cat.Value)
                {
                    stat.OnValueChanged -= NotifyStatValueChanged;
                    stat.OnLevelChanged -= NotifyStatLevelChanged;
//                    stat.OnExperienceChanged -= NotifyStatExperienceChanged;
                }
            }

            _stats.Clear();
        }


        /// <summary>
        /// Clears all values and grabs new values from the data providers.
        /// </summary>
        public void Prepare()
        {
            Clear();

            foreach (var provider in dataProviders)
            {
                var stats = provider.Prepare();
                foreach (var kvp in stats)
                {
                    foreach (var stat in kvp.Value)
                    {
                        Add(kvp.Key, stat);
                    }
                }
            }
        }

        public void ChangeAll(StatDecorator[] stats, float multiplier = 1f, bool fireEvents = true)
        {
            foreach (var stat in stats)
            {
                var s = Get(stat.stat.category, stat.stat.statName);
                if (s != null)
                {
                    s.ChangeCurrentValueRaw(stat.floatValue * multiplier, fireEvents);
                }
            }
        }

        public void SetAll(StatDecorator[] stats, float multiplier = 1f, bool fireEvents = true)
        {
            foreach (var stat in stats)
            {
                Set(stat, multiplier, fireEvents);
            }
        }

        public void Set(StatDecorator stat, float multiplier = 1f, bool fireEvents = true)
        {
            Assert.IsNotNull(stat, "Given stat is null");
            Assert.IsNotNull(stat.stat, "Given stat's definition is null");
            var s = Get(stat.stat.category, stat.stat.statName);
            if (s == null)
            {
                return;
            }

            switch (stat.actionEffect)
            {
                case StatDecorator.ActionEffect.Add:

                    if (stat.isFactor)
                        s.ChangeFactorMax((stat.floatValue - 1.0f) * multiplier, true, fireEvents);
                    else
                        s.ChangeMaxValueRaw(stat.floatValue * multiplier, true, fireEvents);

                    break;
                case StatDecorator.ActionEffect.AddExperience:

                    if (stat.isFactor)
                        s.SetExperience(s.currentExperience * (stat.floatValue * multiplier), fireEvents);
                    else
                        s.ChangeExperience(stat.floatValue * multiplier, fireEvents);

                    break;
                case StatDecorator.ActionEffect.Restore:

                    if (stat.isFactor)
                        s.ChangeCurrentValueRaw((s.currentValue * (stat.floatValue - 1.0f)) * multiplier, fireEvents);
                    else
                        s.ChangeCurrentValueRaw(stat.floatValue * multiplier, fireEvents);

                    break;

                case StatDecorator.ActionEffect.Decrease:

                    if (stat.isFactor)
                        s.ChangeCurrentValueRaw(-((s.currentValue * (stat.floatValue - 1.0f)) * multiplier), fireEvents);
                    else
                        s.ChangeCurrentValueRaw(-(stat.floatValue * multiplier), fireEvents);

                    break;
                default:
                    DevdogLogger.LogWarning("Action effect" + stat.actionEffect + " not found.");
                    break;
            }
        }

        protected void NotifyStatValueChanged(IStat stat)
        {
            if (OnStatValueChanged != null)
            {
                OnStatValueChanged(stat);
            }
        }

        protected void NotifyStatLevelChanged(IStat stat)
        {
            if (OnStatLevelChanged != null)
            {
                OnStatLevelChanged(stat);
            }
        }
//
//        protected void NotifyStatExperienceChanged(IStat stat)
//        {
//            if (OnStatExperienceChanged != null)
//            {
//                OnStatExperienceChanged(stat);
//            }
//        }

        public IEnumerator<KeyValuePair<string, List<IStat>>> GetEnumerator()
        {
            return _stats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
