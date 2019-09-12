using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    public class DatabaseNotFoundException : Exception
    {

        public DatabaseNotFoundException(string msg)
            : base(msg)
        {
            
        }

    }
}
