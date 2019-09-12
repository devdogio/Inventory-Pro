using System;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public partial class ItemInfoRow
    {
        /// <summary>
        /// Title of the label.
        /// </summary>
        public string title;
        public Color titleColor;

        /// <summary>
        /// Text of the label.
        /// </summary>
        public string text;
        public Color textColor;


        public ItemInfoRow()
        {

        }

        public ItemInfoRow(string title, Color color)
            : this(title, String.Empty, color, Color.white)
        { }

        public ItemInfoRow(string title, string text)
            : this(title, text, Color.white, Color.white)
        { }

        public ItemInfoRow(string title, string text, Color titleColor, Color textColor)
        {
            this.title = title;
            this.text = text;
            this.titleColor = titleColor;
            this.textColor = textColor;
        }
    }
}