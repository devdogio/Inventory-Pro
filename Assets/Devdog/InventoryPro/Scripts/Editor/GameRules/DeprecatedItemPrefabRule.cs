using Devdog.General;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class DeprecatedItemPrefabRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
#pragma warning disable 618

            var itemTriggers = Resources.FindObjectsOfTypeAll<ObjectTriggererItem>();
            foreach (var itemTrigger in itemTriggers)
            {
                if (PrefabUtility.GetPrefabType(itemTrigger.gameObject) != PrefabType.Prefab)
                {
                    continue;
                }

                var tempTrigger = itemTrigger;
                issues.Add(new GameRuleIssue("Using deprecated ObjectTriggererItem; It's been replaced by ItemTrigger", MessageType.Error, new GameRuleAction("Fix (replace)",
                    () =>
                    {
                        tempTrigger.gameObject.GetOrAddComponent<ItemTrigger>();
                        tempTrigger.gameObject.GetOrAddComponent<ItemTriggerInputHandler>();

                        UnityEngine.Object.DestroyImmediate(tempTrigger, true);

                    }), new GameRuleAction("Select object", () =>
                    {
                        SelectObject(tempTrigger);
                    })));
            }

#pragma warning restore 618
        }
    }
}