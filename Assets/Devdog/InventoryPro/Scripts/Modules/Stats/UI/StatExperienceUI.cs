using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Security;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    public class StatExperienceUI : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ForceCustomObjectPicker]
        private StatDefinition _stat;
        public StatDefinition stat
        {
            get { return _stat; }
            protected set { _stat = value; }
        }

        [Header("Player")]
        public bool useCurrentPlayer = true;
        public Player player;

        [Header("Interpolation")]
        public bool useValueInterpolation = false;
        public AnimationCurve interpolationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float interpolationSpeed = 1f;

        [Header("Visuals")]
        public Text statName;
        public Text normalizedValue;
        public string normalizedValueStringFormat = "{0:p00}%";
        public UIShowValue visualizer = new UIShowValue();

        /// <summary>
        /// The aim value used for interpolation.
        /// </summary>
        private float _deltaStatValue;

        protected virtual void Start()
        {
            if (useCurrentPlayer)
            {
                PlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            }

            // Force a repaint.
            OnPlayerChanged(null, PlayerManager.instance.currentPlayer);
        }

        private void OnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            // Remove the old
            if (oldPlayer != null && oldPlayer.inventoryPlayer != null)
            {
                oldPlayer.inventoryPlayer.stats.Get(stat.category, stat.statName).OnExperienceChanged -= Repaint;
            }

            player = newPlayer;

            // Add the new
            if (player != null && player.inventoryPlayer != null)
            {
                var s = player.inventoryPlayer.stats.Get(stat.category, stat.statName);
                s.OnExperienceChanged += Repaint;
                Repaint(s);
            }
        }

        protected virtual void Repaint(IStat stat)
        {
            if (stat == null)
            {
                return;
            }

            if (statName != null)
            {
                statName.text = stat.definition.statName;
            }

            var maxAim = GetNextLevelExperienceRequired(stat);
            if (normalizedValue != null && stat.currentLevel != null)
            {
                var currentDiffFactor = CurrentStatExperienceDiffFactor(stat);
                normalizedValue.text = string.Format(normalizedValueStringFormat, currentDiffFactor);
            }

            if (useValueInterpolation)
            {
                StartCoroutine(_RepaintInterpolated(stat));
            }
            else
            {
                visualizer.Repaint(stat.currentExperience, maxAim);
            }
        }

        private float CurrentStatExperienceDiffFactor(IStat stat)
        {
            var currentDiffFactor = Mathf.Abs(stat.currentLevel.experienceRequired - stat.currentExperience);
            var diffFromNextLevel = (GetNextLevelExperienceRequired(stat) - stat.currentLevel.experienceRequired);
            currentDiffFactor /= diffFromNextLevel;
            return currentDiffFactor;
        }

        private float GetNextLevelExperienceRequired(IStat stat)
        {
            if (stat.currentLevelIndex + 1 < stat.definition.levels.Length)
            {
                return stat.definition.levels[stat.currentLevelIndex + 1].experienceRequired;
            }

            if (stat.definition.levels.Length > 0)
            {
                return stat.definition.levels[stat.definition.levels.Length - 1].experienceRequired;
            }

            return 0f;
        }

        private IEnumerator _RepaintInterpolated(IStat stat)
        {
            var currentDiff = Mathf.Abs(stat.currentLevel.experienceRequired - stat.currentExperience);
            var nextLevelDiff = (GetNextLevelExperienceRequired(stat) - stat.currentLevel.experienceRequired);

            float timer = 0f;
            while (timer < 1f)
            {
                float val = Mathf.Lerp(_deltaStatValue, currentDiff, interpolationCurve.Evaluate(timer));
                visualizer.Repaint(val, nextLevelDiff);

                timer += Time.deltaTime * interpolationSpeed;
                yield return null;
            }

            _deltaStatValue = currentDiff;
        }
    }
}
