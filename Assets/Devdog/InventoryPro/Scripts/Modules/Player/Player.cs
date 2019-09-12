using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.InventoryPro;

namespace Devdog.General
{
    public partial class Player
    {

        private InventoryPlayer _inventoryPlayer;
        public InventoryPlayer inventoryPlayer
        {
            get
            {
                if (_inventoryPlayer == null)
                {
                    _inventoryPlayer = GetComponent<InventoryPlayer>();
                }

                return _inventoryPlayer;
            }
        }

    }
}
