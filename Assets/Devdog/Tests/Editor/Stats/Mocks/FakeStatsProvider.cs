using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.DevdogInternal.Tests
{
    public class FakeStatsProvider : IStatsProvider
    {
        public Dictionary<string, List<IStat>> Prepare()
        {
            var appendTo = new Dictionary<string, List<IStat>>();
            appendTo["Default"] = new List<IStat>()
            {
                new Stat(new FakeStatDefinition() { statName = "Health", category = "Default" })
            };

            return appendTo;
        }
    }
}
