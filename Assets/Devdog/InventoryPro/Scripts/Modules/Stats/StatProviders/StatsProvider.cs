using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    public class StatsProvider : IStatsProvider
    {
        public Dictionary<string, List<IStat>> Prepare()
        {
            var appendTo = new Dictionary<string, List<IStat>>();

            // Get the properties
            foreach (var stat in ItemManager.database.statDefinitions)
            {
                if (stat == null || stat.enabled == false)
                {
                    continue;
                }

                if (appendTo.ContainsKey(stat.category) == false)
                {
                    appendTo.Add(stat.category, new List<IStat>());
                }

                // Check if it's already in the list
                if (appendTo[stat.category].FirstOrDefault(o => o.definition.Equals(stat)) != null)
                {
                    continue;
                }

                appendTo[stat.category].Add(new Stat(stat));
            }

            return appendTo;
        }
    }
}