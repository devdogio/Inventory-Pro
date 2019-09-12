using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.General;
using Devdog.InventoryPro;

namespace Devdog.InventoryPro
{
    using Devdog.General.ThirdParty.UniLinq;

    /// <summary>
    /// This is used to represent gold as an item, once the item is picked up gold will be added to the inventory collection.
    /// </summary>
    public partial class CurrencyInventoryItem : InventoryItemBase
    {
        [SerializeField]
        private float _amount;
        public float amount
        {
            get { return _amount; }
            protected set { _amount = value; }
        }

        public override uint currentStackSize
        {
            get { return (uint)_amount; }
            set
            {
                base.currentStackSize = value;
                _amount = value;
            }
        }

        [SerializeField]
        [Required]
        private CurrencyDefinition _currency;
        public CurrencyDefinition currency
        {
            get { return _currency; }
            protected set { _currency = value; }
        }

        public override bool PickupItem ()
        {
            InventoryManager.AddCurrency(currency, _amount);

            if(this.IsInstanceObject())
                Destroy (gameObject); // Don't need to store gold objects
            
            return true;
        }

        public override LinkedList<ItemInfoRow[]> GetInfo()
        {
            var info = base.GetInfo();
            info.Clear();

            info.AddLast(new ItemInfoRow[]{
                new ItemInfoRow("Amount", currency.ToString(_amount, 0f, float.MaxValue))
            });

            return info;
        }

        public override int Use()
        {
            return -2; // Can't use currencies
        }
    }
}