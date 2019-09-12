using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public interface IStatsProvider
    {

        /// <summary>
        /// Set the categories and properties, does not calculate anything.
        /// </summary>
        Dictionary<string, List<IStat>> Prepare();

    }
}
