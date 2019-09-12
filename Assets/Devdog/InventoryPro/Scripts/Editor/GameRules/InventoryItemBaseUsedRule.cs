using Devdog.General;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class InventoryItemBaseUsedRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            var items = Resources.FindObjectsOfTypeAll<InventoryItemBase>();
            foreach (var item in items)
            {
                if (item.GetType() == typeof (InventoryItemBase))
                {
                    var itemTemp = item;
                    issues.Add(new GameRuleIssue("InventoryItemBase shouldn't be used.", MessageType.Error, new GameRuleAction("Fix (remove)",
                    () =>
                    {
                        UnityEngine.Object.DestroyImmediate(itemTemp, true);
                    }
                    ), new GameRuleAction("Select", () =>
                    {
                        SelectObject(itemTemp);
                    })));
                }
            }
        }
    }
}