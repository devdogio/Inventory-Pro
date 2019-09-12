using Devdog.General;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class InventoryPlayerRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
            var players = Resources.FindObjectsOfTypeAll<InventoryPlayer>();
            foreach (var player in players)
            {
                CreateIssueIfMissingComponent<Player>(player.gameObject, "InventoryPlayer component is missing the general Player component.", MessageType.Warning);
            }
        }
    }
}