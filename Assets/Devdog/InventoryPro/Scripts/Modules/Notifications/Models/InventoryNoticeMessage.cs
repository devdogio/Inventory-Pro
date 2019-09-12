using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class InventoryNoticeMessage : InventoryMessage
    {
        public DateTime time;

        public Color color = Color.white;
        public NoticeDuration duration = NoticeDuration.Medium;


        /// <summary>
        /// Required for PlayMaker...
        /// </summary>
        public InventoryNoticeMessage()
        { }

        public InventoryNoticeMessage(string title, string message, NoticeDuration duration, params System.Object[] parameters)
            : this(title, message, duration, Color.white, DateTime.Now, parameters)
        { }

        public InventoryNoticeMessage(string title, string message, NoticeDuration duration, Color color, params System.Object[] parameters)
            : this(title, message, duration, color, DateTime.Now, parameters)
        { }

        public InventoryNoticeMessage(string title, string message, NoticeDuration duration, Color color, DateTime time, params System.Object[] parameters)
        {
            this.title = title;
            this.message = message;
            this.color = color;
            this.time = time;
            this.parameters = parameters;
        }

        public override void Show(params System.Object[] param)
        {
            base.Show(param);

            this.time = DateTime.Now;

            if (InventoryManager.instance.notice != null)
                InventoryManager.instance.notice.AddMessage(this);
        }
    }
}