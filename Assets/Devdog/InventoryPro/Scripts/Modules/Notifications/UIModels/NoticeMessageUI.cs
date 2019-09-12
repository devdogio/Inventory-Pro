using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// A single message inside the message displayer
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public partial class NoticeMessageUI : MonoBehaviour, IPoolable
    {
        public UnityEngine.UI.Text title;
        public UnityEngine.UI.Text message;
        public UnityEngine.UI.Text time;

        public string format = "{0}";

        [Header("Animations")]
        public AnimationClip showAnimation;
        public AnimationClip hideAnimation;

        [HideInInspector]
        [NonSerialized]
        public float showTime = 4.0f;
        
        public DateTime dateTime { get; private set; }


        [NonSerialized]
        protected Animator animator;
        [NonSerialized]
        protected bool isHiding = false; // In the process of hiding

        public virtual void Awake()
        {
            animator = GetComponent<Animator>();

            if (showAnimation != null)
                animator.Play(showAnimation.name);
        }

        public virtual void Repaint(InventoryNoticeMessage message)
        {
            this.showTime = (int)message.duration;
            this.dateTime = message.time;

            if (string.IsNullOrEmpty(message.title) == false)
            {
                if (this.title != null)
                {
                    this.title.text = string.Format(string.Format(format, message.title), message.parameters);
                    this.title.color = message.color;
                }
            }
            else
            {
                if (title != null)
                {
                    title.gameObject.SetActive(false);
                }
            }


            string msg = string.Format(format, message.message);
            this.message.text = string.Format(msg, message.parameters);
            this.message.color = message.color;

            if (this.time != null)
            {
                this.time.text = dateTime.ToShortTimeString();
                this.time.color = message.color;
            }
        }

        public virtual void Hide()
        {
            // Already hiding
            if (isHiding)
                return;

            isHiding = true;

            if (hideAnimation != null)
                animator.Play(hideAnimation.name);
        }

        public void ResetStateForPool()
        {
            isHiding = false;
        }
    }
}