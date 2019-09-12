using Devdog.General;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class InventoryProManagersRule : ManagersRuleBase
    {
        public override void UpdateIssue()
        {
            FindManagerOfTypeOrCreateIssue<InventoryManager>();
            FindManagerOfTypeOrCreateIssue<InventorySettingsManager>();
            FindManagerOfTypeOrCreateIssue<ItemManager>();
//            FindManagerOfTypeOrCreateIssue<InventoryPlayerManager>();
        }
    }
}