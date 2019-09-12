using System;
using Devdog.General;
using Devdog.InventoryPro.UI;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class StatDefinition : ScriptableObject, IStatDefinition
    {
        //[HideInInspector]
        public int ID;

        [SerializeField]
        private bool _enabled = true;
        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public new string name
        {
            get { return _statName; }
            set { _statName = value; }
        }

        [SerializeField]
        [Required]
        private string _statName = "";
        public string statName
        {
            get { return _statName; }
            set { _statName = value; }
        }

        [SerializeField]
        [Required]
        private string _category = "Default";
        public string category
        {
            get { return _category; }
            set { _category = value; }
        }

        [SerializeField]
        private bool _showInUI = true;
        public bool showInUI
        {
            get { return _showInUI; }
            set { _showInUI = value; }
        }

        [SerializeField]
        private StatRowUI _uiPrefab;
        public StatRowUI uiPrefab
        {
            get { return _uiPrefab; }
            set { _uiPrefab = value; }
        }

        [Tooltip("How the value is shown.\n{0} = Current amount\n{1} = Max amount\n{2} = Property name")]
        [SerializeField]
        private string _valueStringFormat = "{0}";
        public string valueStringFormat
        {
            get { return _valueStringFormat; }
            set { _valueStringFormat = value; }
        }

        [SerializeField]
        private Color _color = Color.white;
        public Color color
        {
            get { return _color; }
            set { _color = value; }
        }

        [SerializeField]
        private Sprite _icon; 
        public Sprite icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// The base value is the start value of this property.
        /// </summary>
        [Tooltip("The base value is the start value of this property")]
        [SerializeField]
        private float _baseValue;
        public float baseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }

        [SerializeField]
        private float _maxValue = 100.0f;
        public float maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        [SerializeField]
        private int _startLevel;
        public int startLevel
        {
            get { return _startLevel; }
            set { _startLevel = value; }
        }

        [SerializeField]
        private bool _autoProgressLevels = true;
        public bool autoProgressLevels
        {
            get { return _autoProgressLevels; }
            set { _autoProgressLevels = value; }
        }


        [SerializeField]
        private StatLevel[] _levels = new StatLevel[0];
        public StatLevel[] levels
        {
            get { return _levels; }
            set { _levels = value; }
        }
        
        public bool Equals(IStatDefinition other)
        {
            return statName == other.statName &&
                   category == other.category;
        }

        public override string ToString()
        {
            return statName;
        }

        public string ToString(IStat stat)
        {
            return ToString(stat, valueStringFormat);
        }

        /// <summary>
        /// {0} = The current amount
        /// {1} = The max amount
        /// {2} = The stat name
        /// {3} = The stat level
        /// {4} = The stat experience
        /// {5} = The stat required experience to next level (empty if last level)
        /// </summary>
        public string ToString(IStat stat, string overrideFormat)
        {
            try
            {
                if (stat == null)
                {
                    return string.Format(overrideFormat, 0f, maxValue, statName, 1, 0, 0);
                }

                return string.Format(overrideFormat, stat.currentValue, maxValue, statName, stat.currentLevelIndex + 1, stat.currentExperience, levels.Length > stat.currentLevelIndex + 1 ? levels[stat.currentLevelIndex + 1].experienceRequired : 0);
            }
            catch (Exception)
            {
                // Ignored
            }

            return "(Formatting not valid)";
        }

        public string ToString(object value)
        {
            return ToString(value, valueStringFormat);
        }

        public string ToString(object value, string overrideFormat)
        {
            try
            {
                return string.Format(overrideFormat, value, maxValue, statName, 1, 0, 0);
            }
            catch (Exception)
            {
                // Ignored
            }

            return "(Formatting not valid)";
        }
    }
}