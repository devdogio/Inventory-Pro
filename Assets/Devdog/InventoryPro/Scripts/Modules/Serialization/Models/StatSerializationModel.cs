using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;


namespace Devdog.InventoryPro
{
    public partial class StatSerializationModel
    {
        public int statID;

        public float currentMaxValue;
        public float currentFactorMax;
        public float currentFactor;
        public float currentValueRaw;
        public float currentExperience;

        public StatSerializationModel()
        {
            
        }

        public StatSerializationModel(Stat from)
        {
            FromStat(from);
        }

        public void FromStat(Stat from)
        {
            statID = ((StatDefinition)from.definition).ID;
            currentMaxValue = from.currentMaxValueRaw;
            currentFactorMax = from.currentFactorMax;
            currentFactor = from.currentFactor;
            currentValueRaw = from.currentValueRaw;
            currentExperience = from.currentExperience;
        }

        public Stat ToStat()
        {
            Stat dec;
            ToStat(out dec);

            return dec;
        }

        public void ToStat(out Stat to)
        {
            to = new Stat(ItemManager.database.statDefinitions.FirstOrDefault(o => o.ID == statID));
            to.SetMaxValueRaw(currentMaxValue, false, false);
            to.SetFactorMax(currentFactorMax, false, false);
            to.SetFactor(currentFactor, false);
            to.SetCurrentValueRaw(currentValueRaw, false);
            to.SetExperience(currentExperience, true);
        }
    }
}
