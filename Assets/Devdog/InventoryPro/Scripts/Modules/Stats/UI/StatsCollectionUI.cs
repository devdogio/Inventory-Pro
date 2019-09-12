using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.General.UI;
using Devdog.InventoryPro;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// Used to define a row of stats.
    /// </summary>
    public partial class StatsCollectionUI : MonoBehaviour
    {
        [Required]
        public RectTransform container;

        [Header("UI Prefabs")]
        [Required]
        public StatRowUI defaultStatusRowPrefab;
        public StatCategoryUI statusCategoryPrefab;

        private StatsCollection _statsCollection;
        private Dictionary<IStat, StatRowUI> _uiLookup = new Dictionary<IStat, StatRowUI>();
        private UIWindow _window;

        public StatsCollection statsCollection
        {
            get { return _statsCollection; }
            set
            {
                if (_statsCollection != null)
                {
                    _statsCollection.OnStatValueChanged -= RepaintStat;
                    _statsCollection.OnStatLevelChanged -= RepaintStat;
                }

                _statsCollection = value;

                if (_statsCollection != null)
                {
                    _statsCollection.OnStatValueChanged += RepaintStat;
                    _statsCollection.OnStatLevelChanged += RepaintStat;
                }
            }
        }

        protected virtual void Awake()
        {
            _window = GetComponentInParent<UIWindow>();
        }

        protected void OnEnable()
        {
            if (_window != null)
            {
                _window.OnShow += OnWindowShown;
            }
        }

        protected void OnDisable()
        {
            if (_window != null)
            {
                _window.OnShow -= OnWindowShown;
            }
        }

        protected void OnDestroy()
        {
            if (_statsCollection != null)
            {
                _statsCollection.OnStatValueChanged -= RepaintStat;
                _statsCollection.OnStatLevelChanged -= RepaintStat;
            }
        }

        protected virtual void OnWindowShown()
        {
            RepaintAll();
        }

        public virtual void RepaintAll()
        {
            if (isActiveAndEnabled == false || defaultStatusRowPrefab == null)
                return;

            if (statsCollection == null)
                return;

            // Get rid of the old
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            // Maybe make a pool for the items? See some spikes...
            foreach (var stat in statsCollection)
            {
                // Maybe make a pool for the items? See some spikes...
                if (stat.Value.Count(o => o.definition.showInUI) == 0)
                    continue; // No items to display in this category.

                // stat.Key is category
                // stat.Value is all items in category
                StatCategoryUI cat = null;
                if (statusCategoryPrefab != null)
                {
                    cat = GetNewCategoryUIInstance(stat.Key);
                    cat.Repaint(stat.Key);
                    cat.transform.SetParent(container);
                    InventoryUtility.ResetTransform(cat.transform);
                }

                foreach (var s in stat.Value)
                {
                    if (s.definition.showInUI == false)
                        continue;

                    var obj = GetNewStatUIInstance(s);
                    obj.Repaint(s);
                    obj.transform.SetParent(cat != null ? cat.container : container);

                    UIUtility.ResetTransform(obj.transform);
                }
            }
        }

        protected virtual StatRowUI GetNewStatUIInstance(IStat stat)
        {
            var inst = Instantiate<StatRowUI>(stat.definition.uiPrefab ?? defaultStatusRowPrefab);
            _uiLookup[stat] = inst;
            return inst;
        }

        protected virtual StatCategoryUI GetNewCategoryUIInstance(string category)
        {
            return Instantiate<StatCategoryUI>(statusCategoryPrefab);
        }

        /// <summary>
        /// Repaint a single stat.
        /// </summary>
        /// <param name="stat"></param>
        public virtual void RepaintStat(IStat stat)
        {
            if (isActiveAndEnabled == false || defaultStatusRowPrefab == null)
            {
                return;
            }

            if (_uiLookup.ContainsKey(stat))
            {
                if (_uiLookup[stat] != null)
                {
                    _uiLookup[stat].Repaint(stat);
                }
            }
        }
    }
}