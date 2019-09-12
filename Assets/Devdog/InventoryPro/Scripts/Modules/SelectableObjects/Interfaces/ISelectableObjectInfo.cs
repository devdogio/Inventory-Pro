using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General;

namespace Devdog.InventoryPro
{
    public interface ISelectableObjectInfo
    {
        string name { get; }
        float health { get; }
        float maxHealth { get; }
        
        float healthFactor { get; }
        bool useHealth { get; set; }

        void ChangeHealth(float changeBy, bool fireEvents = true);
    }
}
