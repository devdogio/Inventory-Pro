using System;
using UnityEngine;

namespace Devdog.InventoryPro.Editors
{
    public class CustomOverrideProperty
    {
        public string serializedName { get; set; }
        public System.Action action{ get; set; }

        public CustomOverrideProperty(string serializedName, System.Action action)
        {
            this.serializedName = serializedName;
            this.action = action;
        }
    }
}
