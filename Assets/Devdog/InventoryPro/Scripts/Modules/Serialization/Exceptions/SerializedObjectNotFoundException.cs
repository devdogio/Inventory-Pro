using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    public class SerializedObjectNotFoundException : Exception
    {

        public SerializedObjectNotFoundException(string message)
            : base(message)
        {
            
        }
    }
}
