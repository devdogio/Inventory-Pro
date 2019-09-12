using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro.UI;
using Devdog.InventoryPro;
using UnityEngine.UI;

namespace Devdog.InventoryPro
{
    /// <summary>
    /// How long a message should last.
    /// Parse to int to get time in seconds.
    /// </summary>
    public enum NoticeDuration
    {
        Short = 2,
        Medium = 4,
        Long = 6,
        ExtraLong = 8
    }

    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Windows/Notice")]
    public partial class NoticeUI : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Note that it also fired when message == null or empty, even though the system won't process the message.
        /// This is because someone might want to implement their own system and just use the event as a link to connect the 2 systems.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="duration"></param>
        /// <param name="parameters"></param>
        public delegate void NewMessage(InventoryNoticeMessage message, params System.Object[] parameters);
        public event NewMessage OnNewMessage;

        #endregion

        [Header("General")]
        public NoticeMessageUI noticeRowPrefab;

        [Required]
        public RectTransform container;

        public ScrollRect scrollRect;
        public General.AudioClipInfo onNewMessageAudioClip;


        /// <summary>
        /// When more messages come in the last items will be removed.
        /// </summary>
        [Header("Messages")]
        public int maxMessages = 50;

        /// <summary>
        /// Remove the item after the show time has passed, if false, the item will continue to exist.
        /// </summary>
        public bool destroyAfterShowTime = true;
    
        /// <summary>
        /// All show times are multiplied by this value, if you want to increase all times, use this value.
        /// </summary>
        public float showTimeFactor = 1.0f;
    
        [NonSerialized]
        protected List<NoticeMessageUI> messages = new List<NoticeMessageUI>(8);
        private ComponentPool<NoticeMessageUI> pool;


        public virtual void Awake()
        {
            pool = new ComponentPool<NoticeMessageUI>(noticeRowPrefab, maxMessages);
        }

        public virtual void Update()
        {
            if (destroyAfterShowTime == false)
                return;

            foreach (var message in messages)
            {
                message.showTime -= Time.deltaTime;
                if (message.showTime < 0.0f)
                {
                    message.Hide();
                }
            }
        }

        public void AddMessage(string message, NoticeDuration duration = NoticeDuration.Medium)
        {
            AddMessage(new InventoryNoticeMessage(string.Empty, message, duration));
        }

        public void AddMessage(string message, NoticeDuration duration, params System.Object[] parameters)
        {
            AddMessage(string.Empty, message, duration, parameters);
        }

        public void AddMessage(string title, string message, NoticeDuration duration, params System.Object[] parameters)
        {
            AddMessage(new InventoryNoticeMessage(title, message, duration));
        }

        public virtual void AddMessage(InventoryNoticeMessage message)
        {
            if (OnNewMessage != null)
                OnNewMessage(message, message.parameters);

            if (string.IsNullOrEmpty(message.message))
                return;

            bool scrollbarAtBottom = false;
            if (scrollRect != null && scrollRect.verticalScrollbar != null && scrollRect.verticalScrollbar.value < 0.05f)
                scrollbarAtBottom = true;

            // Incase we don't actually want to display anything and just port the data to some other class through events.
            if (noticeRowPrefab != null)
            {
                var item = pool.Get();
                //var item = GameObject.Instantiate<NoticeMessageUI>(noticeRowPrefab);
                item.transform.SetParent(container);
                item.transform.SetSiblingIndex(0); // Move to the top of the list
                InventoryUtility.ResetTransform(item.transform);

                item.Repaint(message);

                AudioManager.AudioPlayOneShot(onNewMessageAudioClip);
                messages.Add(item);
            }
        
            if (messages.Count > maxMessages)
            {
                StartCoroutine(DestroyAfter(messages[0], messages[0].hideAnimation != null ? messages[0].hideAnimation.length : 0));
                messages[0].Hide();
                messages.RemoveAt(0);
            }

            if (scrollbarAtBottom)
                scrollRect.verticalNormalizedPosition = 0.0f;
        }

        protected virtual IEnumerator DestroyAfter(NoticeMessageUI item, float time)
        {
            yield return new WaitForSeconds(time);
            pool.Destroy(item);
        }
    }
}