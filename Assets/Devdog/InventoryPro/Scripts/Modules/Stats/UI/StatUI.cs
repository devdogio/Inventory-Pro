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
    public class StatUI : MonoBehaviour
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
        public UIShowValue visualizer = new UIShowValue();

        /// <summary>
        /// The aim value used for interpolation.
        /// </summary>
        private float _aimStatValue;
        private float _deltaStatValue;

        protected virtual void Start()
        {
            if (useCurrentPlayer)
            {
                this.player = PlayerManager.instance.currentPlayer;

                PlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            }

            // Force a repaint.
            OnPlayerChanged(null, this.player);
        }

        protected void OnDestroy()
        {
//            if (useCurrentPlayer)
//            {
//                PlayerManager.instance.OnPlayerChanged -= OnPlayerChanged;
//            }

            if (player != null && player.inventoryPlayer != null)
            {
                player.inventoryPlayer.stats.Get(stat).OnValueChanged -= Repaint;
            }
        }

        private void OnPlayerChanged(Player oldPlayer, Player newPlayer)
        {
            // Remove the old
            if (oldPlayer != null && oldPlayer.inventoryPlayer != null)
            {
                oldPlayer.inventoryPlayer.stats.Get(stat).OnValueChanged -= Repaint;
            }

            player = newPlayer;

            // Add the new
            if (player != null && player.inventoryPlayer != null)
            {
                var s = player.inventoryPlayer.stats.Get(stat);
                Assert.IsNotNull(s, "Given stat " + stat + " could not be found on player.");
                s.OnValueChanged += Repaint;
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

            if (useValueInterpolation && gameObject.activeInHierarchy)
            {
                StartCoroutine(_RepaintInterpolated(stat));
            }
            else
            {
                visualizer.Repaint(stat.currentValue, stat.currentMaxValue);
            }
        }

        private IEnumerator _RepaintInterpolated(IStat stat)
        {
            _aimStatValue = stat.currentValue;
            float timer = 0f;
            while (timer < 1f)
            {
                float val = Mathf.Lerp(_deltaStatValue, _aimStatValue, interpolationCurve.Evaluate(timer));
                visualizer.Repaint(val, stat.currentMaxValue);

                timer += Time.deltaTime * interpolationSpeed;
                yield return null;
            }

            _deltaStatValue = _aimStatValue;
        }
    }
}
