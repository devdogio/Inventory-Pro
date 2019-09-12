using System;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    [System.Serializable]
    public partial class UIShowValue
    {
        [Header("Text")]
        public UnityEngine.UI.Text textField;
        public string textFormat = "{0}/{1}";
        public int roundToDecimals = 1;
        public bool clearTextWhenZero = true;

        [Header("Slider")]
        public UnityEngine.UI.Slider slider;
        public bool hideSliderWhenZero = false;

        [Header("Image fill")]
        public UnityEngine.UI.Image imageFill; // Used for fillAmount

        [Header("Audio")]
        public General.AudioClipInfo activationClip;


        public void Repaint(float current, float max)
        {
            if (textField != null)
            {
                if (current < 0.0001f && clearTextWhenZero)
                {
                    textField.text = "";
                    SetActive(textField, false);
                }
                else
                {
                    textField.text = string.Format(textFormat, System.Math.Round(current, roundToDecimals), System.Math.Round(max, roundToDecimals), System.Math.Round(current / max, roundToDecimals));
                    SetActive(textField, true);
                }
            }

            if (slider != null)
            {
                slider.minValue = 0.0f;
                slider.maxValue = max;

                if(current <= 0.0001f)
                {
                    if(hideSliderWhenZero)
                    {
                        SetActive(slider, false);
                    }
                    else
                    {
                        SetActive(slider, true);
                    }
                }
                else
                {
                    SetActive(slider, true);
                }

                // To avoid GC
                if (current != slider.value)
                {
                    slider.value = current;
                }
            }

            if (imageFill != null)
            {
                float n = current / max;

                SetActive(imageFill, n > 0.001f);

                // To avoid GC
                if (n != imageFill.fillAmount)
                {
                    imageFill.fillAmount = n;
                }
            }
        }

        private void SetActive(MonoBehaviour b, bool set)
        {
            // Check to avoid GC
//            if (b.gameObject.activeSelf != set)
//            {
//                b.gameObject.SetActive(set);
//            }

            if (b.enabled != set)
            {
                b.enabled = set;
            }
        }

        public void HideAll()
        {
            if (textField != null)
                SetActive(textField, false);

            if (slider != null)
                SetActive(slider, false);

            if (imageFill != null)
                SetActive(imageFill, false);
        }

        /// <summary>
        /// An action is activated, show it.
        /// </summary>
        public void Activate()
        {
            AudioManager.AudioPlayOneShot(activationClip);
        }
    }
}
