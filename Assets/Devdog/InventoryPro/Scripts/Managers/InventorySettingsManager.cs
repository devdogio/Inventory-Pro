// /**
// * Written By Joris Huijbregts
// * Some legal stuff --- Copyright!
// */
using UnityEngine;
using System.Collections;
using Devdog.General;
using UnityEngine.Assertions;

namespace Devdog.InventoryPro
{
    [AddComponentMenu(InventoryPro.AddComponentMenuPath + "Managers/Inventory Settings manager")]
    public partial class InventorySettingsManager : ManagerBase<InventorySettingsManager>
    {
        [Required]
        [SerializeField]
        private InventorySettingsDatabase _settings;
        public InventorySettingsDatabase settings
        {
            get
            {
                return _settings;
            }
            set { _settings = value; }
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}