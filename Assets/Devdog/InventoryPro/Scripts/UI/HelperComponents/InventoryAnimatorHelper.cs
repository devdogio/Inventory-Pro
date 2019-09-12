using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Devdog.InventoryPro.UI
{
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "UI Helpers/Inventory Animation Helper")]
    public partial class InventoryAnimatorHelper : MonoBehaviour
    {

        private Animator _animator;
        private Regex _animationNameRegex;

//        private Vector3 _startPosition;
//        private Vector3 _startScale;
//        private Quaternion _startRotation;

        public void Awake()
        {
            _animator = GetComponent<Animator>();

//            _startPosition = transform.localPosition;
//            _startScale = transform.localScale;
//            _startRotation = transform.localRotation;

            _animationNameRegex = new Regex(@"(\w*)(?:[(]*([0-9,]+\.?[0-9,.]*)*[)]*)", RegexOptions.Singleline); // RegexOptions.Compiled??
        }


        /// <summary>
        /// Allows you to send an animation name with delay and speed.
        /// SlideInLeft    -- Plays animation SlideInLeft
        /// SlideInLeft(0.2)   -- Plays animation SlideInLeft with a 0.2s delay.
        /// SlideInLeft(0.2, 0.5) -- Plays animation SlideInLeft with a 0.2s delay, and at 0.5x speed.
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            var result = _animationNameRegex.Match(name);
            if (result.Groups.Count == 3)
            {
                _animator.enabled = true;

                if (result.Groups[2].Value == "")
                {
                    // No params passed
                    //StartCoroutine(PlayAnimationAfter(result.Groups[1].Value, 0.0f));
                    _animator.Play(result.Groups[1].Value);
                    //animator.enabled = false; // TODO: Disable in co-routine.
                    return;
                }

                // Params passed in
                if (result.Groups[2].Value.Contains(","))
                {
                    // Multiple params
                    string[] paramStrings = result.Groups[2].Value.Split(',');
                    var l = new List<float>(paramStrings.Length);
                    foreach (var param in paramStrings)
                    {
                        string p = param.Trim();
                        if (p == "")
                            continue;

                        float paramFloat;
                        bool parsed = float.TryParse(p, out paramFloat);
                        if (parsed == false)
                        {
                            Debug.LogWarning("Parameter (" + p + ") passed is not a number, use (0.3), not (0.3f) or (0,3).", transform);
                            _animator.enabled = false;
                            return;
                        }

                        l.Add(paramFloat);
                    }

                    // Make sure it's active, can't start coroutines on in-active objects.
                    if (gameObject.activeInHierarchy)
                        StartCoroutine(PlayAnimationAfter(result.Groups[1].Value, l.ToArray()));
                }
                else
                {
                    float waitTime;
                    bool parsed = float.TryParse(result.Groups[2].Value, out waitTime);
                    if (parsed == false)
                    {
                        Debug.LogWarning("Parameter passed is not a number, use (0.3), not (0.3f) or (0,3).", transform);
                        _animator.enabled = false;
                        return;
                    }

                    // Make sure it's active, can't start coroutines on in-active objects.
                    if (gameObject.activeInHierarchy)
                        StartCoroutine(PlayAnimationAfter(result.Groups[1].Value, waitTime, 1.0f));
                }
            }
            else
            {
                Debug.LogWarning("InventoryAnimatorHelper: Regex failed, most the string passed is likely faulty.", transform);
            }
        }

        private IEnumerator PlayAnimationAfter(string animationName, params float[] paramsFloat)
        {
            // Set the animation to the first frame.
            _animator.Play(animationName);
            _animator.speed = 0.0f; // Reset from previous actions?

            if(paramsFloat[0] > 0.0f)
                yield return new WaitForSeconds(paramsFloat[0]);
            
            if (paramsFloat.Length > 1)
                _animator.speed = paramsFloat[1];
            else
                _animator.speed = 1.0f;

            //var clipInfo = animator.GetCurrentAnimatorStateInfo(0);
            //yield return new WaitForSeconds(clipInfo.length);
            //animator.enabled = false; // Disable to avoid continous repaint
        }
    }
}
