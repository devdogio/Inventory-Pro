using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro;
using Devdog.InventoryPro.UI;
using UnityEngine;

namespace Devdog.DevdogInternal.Tests
{
    public class FakeStatDefinition : IStatDefinition
    {
        public string statName { get; set; }
        public string category { get; set; }
        public bool showInUI { get; set; }
        public Sprite icon { get; set; }
        public Color color { get; set; }
        public StatRowUI uiPrefab { get; set; }
        public string valueStringFormat { get; set; }
        public float baseValue { get; set; }
        public float maxValue { get; set; }
        public int startLevel { get; set; }
        public StatLevel[] levels { get; set; }
        public bool autoProgressLevels { get; set; }

        public FakeStatDefinition()
        {
            levels = new StatLevel[0];
        }

        public string ToString(IStat stat)
        {
            return statName;
        }

        public string ToString(IStat stat, string overrideFormat)
        {
            return statName;
        }

        public string ToString(object value)
        {
            return statName;
        }

        public string ToString(object value, string overrideFormat)
        {
            return statName;
        }

        public bool Equals(IStatDefinition other)
        {
            return this.statName == other.statName &&
                   this.category == other.category;
        }
    }
}
