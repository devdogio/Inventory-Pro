using System;
using System.Collections.Generic;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [Serializable]
    public class CurrencyConversion
    {

        public float factor = 1.0f;

        [Required]
        public CurrencyDefinition currency;

        public bool useInAutoConversion = false;


    }
}
