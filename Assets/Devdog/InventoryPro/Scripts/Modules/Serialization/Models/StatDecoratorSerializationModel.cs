using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;


namespace Devdog.InventoryPro
{
    public partial class StatDecoratorSerializationModel
    {
        public int statID;
        public string value;
        public StatDecorator.ActionEffect actionEffect;
        public bool isFactor;

        public StatDecoratorSerializationModel()
        {
            
        }

        public StatDecoratorSerializationModel(StatDecorator decorator)
        {
            FromStat(decorator);
        }

        public void FromStat(StatDecorator dec)
        {
            this.statID = dec.stat.ID;
            this.value = dec.value;
            this.actionEffect = dec.actionEffect;
            this.isFactor = dec.isFactor;
        }

        public StatDecorator ToStat()
        {
            var dec = new StatDecorator();
            ToStat(dec);

            return dec;
        }

        public void ToStat(StatDecorator dec)
        {
            dec.stat = ItemManager.database.statDefinitions.FirstOrDefault(o => o.ID == statID);
            dec.value = value;
            dec.actionEffect = actionEffect;
            dec.isFactor = isFactor;
        }
    }
}
