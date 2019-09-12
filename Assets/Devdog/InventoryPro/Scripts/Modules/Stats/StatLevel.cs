using System;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.InventoryPro.UI;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    public partial class StatLevel
    {
        public float maxValue;
        public float experienceRequired;

        [Header("Unlock")]
//        public float changeValueOnUnlock = 0f;
        public GameObject effectOnUnlock;

        public StatLevel()
        {
            
        }

        public StatLevel(float maxValue, float experienceRequired)
        {
            this.maxValue = maxValue;
            this.experienceRequired = experienceRequired;
        }

        public void Unlock(IStat parent)
        {
#if UNITY_EDITOR
            DevdogLogger.LogVerbose("Unlocked stat level on stat: " + parent.definition.statName + " Level is now: " + parent.definition.levels.ToList().IndexOf(this));
#endif
            if (effectOnUnlock != null)
            {
                UnityEngine.Object.Instantiate<GameObject>(effectOnUnlock);
            }
        }
    }
}