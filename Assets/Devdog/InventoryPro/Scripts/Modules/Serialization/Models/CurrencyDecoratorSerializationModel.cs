using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.InventoryPro
{
    public partial class CurrencyDecoratorSerializationModel
    {
        public uint currencyID;
        public float amount;

        public CurrencyDecoratorSerializationModel()
        {
            
        }

        public CurrencyDecoratorSerializationModel(CurrencyDefinition currency, float amount)
            : this(currency.ID, amount)
        { }

        public CurrencyDecoratorSerializationModel(CurrencyDecorator currency)
            : this(currency.currency.ID, currency.amount)
        { }

        public CurrencyDecoratorSerializationModel(uint currencyID, float amount)
        {
            this.currencyID = currencyID;
            this.amount = amount;
        }

        public CurrencyDecorator ToCurrencyDecorator()
        {
            var dec = new CurrencyDecorator();
            ToCurrencyDecorator(dec);
            return dec;
        }

        public void ToCurrencyDecorator(CurrencyDecorator dec)
        {
            dec.currency = ItemManager.database.currencies.FirstOrDefault(o => o.ID == currencyID);
            dec.amount = amount;
        }
    }
}
