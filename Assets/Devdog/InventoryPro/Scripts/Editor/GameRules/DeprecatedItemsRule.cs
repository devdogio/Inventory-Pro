using Devdog.General;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class DeprecatedItemsRule : GameRuleBase
    {
        public override void UpdateIssue()
        {
#pragma warning disable 618

            var itemHolders = Resources.FindObjectsOfTypeAll<ObjectTriggererItemHolder>();
            foreach (var itemHolder in itemHolders)
            {
                var tempItemHolder = itemHolder;
                issues.Add(new GameRuleIssue("Using deprecated ObjectTriggererItemHolder; It's been replaced by ItemTrigger", MessageType.Error, new GameRuleAction("Fix (replace)",
                    () =>
                    {
                        var oldTrigger = tempItemHolder.gameObject.GetComponent<ObjectTriggererItem>();
                        var itemTrigger = tempItemHolder.gameObject.AddComponent<ItemTrigger>();
                        itemTrigger.itemPrefab = tempItemHolder.item;

                        tempItemHolder.gameObject.AddComponent<ItemTriggerInputHandler>();

                        EditorUtility.SetDirty(tempItemHolder.gameObject);
                        UnityEngine.Object.DestroyImmediate(tempItemHolder, true);
                        if (oldTrigger != null)
                        {
                            UnityEngine.Object.DestroyImmediate(oldTrigger, true);
                        }

                    }), new GameRuleAction("Select object", () =>
                    {
                        SelectObject(tempItemHolder);
                    })));
            }

#pragma warning restore 618
        }
    }
}