using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;
using Devdog.InventoryPro;
using UnityEngine;

namespace Devdog.InventoryPro
{
    public class AdvancedTrigger : Trigger
    {
        [Header("Requirements")]
        public ItemAmountRow[] requiredItems = new ItemAmountRow[0];
        public StatRequirement[] statRequirements = new StatRequirement[0];


        private ISelectableObjectInfo _objectInfo;
        public string triggerName
        {
            get
            {
                if (_objectInfo != null)
                {
                    return _objectInfo.name;
                }

                return string.Empty;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _objectInfo = GetComponent<ISelectableObjectInfo>();
        }


        public override bool CanUse(Player player)
        {
            var canUse = base.CanUse(player);
            if (canUse == false)
            {
                return false;
            }

            // Has requireid items?
            bool canRemoveItems = InventoryManager.CanRemoveItems(requiredItems);
            if (canRemoveItems == false)
            {
                InventoryManager.langDatabase.triggerCantBeUsedMissingItems.Show(triggerName, requiredItems.Select(o => o.item.name).Aggregate((a, b) => a + ", " + b));
                return false;
            }

            // Has required property requirements (stats)
            foreach (var req in statRequirements)
            {
                if (req.CanUse(player.inventoryPlayer) == false)
                {
                    InventoryManager.langDatabase.triggerCantBeUsedFailedStatRequirements.Show(triggerName, req.stat.statName);
                    return false;
                }
            }

            return true;
        }

        public override bool CanUnUse(Player player)
        {
            return base.CanUnUse(player);
        }
    }
}
