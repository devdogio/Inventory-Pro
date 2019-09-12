using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro.UI
{
    /// <summary>
    /// A single row in the infobox.
    /// </summary>
    public partial class InfoBoxRowUI : MonoBehaviour, IPoolable
    {
        public UnityEngine.UI.Text title;
        public UnityEngine.UI.Text message;


        public void ResetStateForPool()
        {
            // Item has no specific states, no need to reset
        }
    }
}