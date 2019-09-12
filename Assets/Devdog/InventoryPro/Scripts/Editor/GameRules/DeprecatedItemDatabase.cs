using System;
using Devdog.General;
using Devdog.General.Editors;
using Devdog.General.Editors.GameRules;
using UnityEngine;
using UnityEditor;

#pragma warning disable 618

namespace Devdog.InventoryPro.Editors.GameRules
{
    public class DeprecatedItemDatabase : GameRuleBase
    {
        public override void UpdateIssue()
        {
            if (ItemManager.database == null)
            {
                issues.Add(new GameRuleIssue("No database set, can't determine issues.", MessageType.Error));
                return;
            }

            if (ItemManager.database.itemRarities.Length > 0 ||
                ItemManager.database.itemCategories.Length > 0 ||
                ItemManager.database.itemProperties.Length > 0 ||
                ItemManager.database.equipTypes.Length > 0 ||
                ItemManager.database.inventoryCurrencies.Length > 0 ||
                ItemManager.database.inventoryCraftingCategories.Length > 0)
            {
                issues.Add(new GameRuleIssue("Move old values to scriptable objects", MessageType.Warning, new GameRuleAction("Fix (Copy)",
                    () =>
                    {

                        ReplaceOldTypes<InventoryItemRarityDeprecated, ItemRarity>(ItemManager.database.itemRarities, (arr) => ItemManager.database.rarities = arr);
                        ItemManager.database.itemRarities = new InventoryItemRarityDeprecated[0];

                        ReplaceOldTypes<InventoryItemCategoryDeprecated, ItemCategory>(ItemManager.database.itemCategories, (arr) => ItemManager.database.categories = arr);
                        ItemManager.database.itemCategories = new InventoryItemCategoryDeprecated[0];

                        ReplaceOldTypes<InventoryItemPropertyDeprecated, StatDefinition>(ItemManager.database.itemProperties, (arr) => ItemManager.database.statDefinitions = arr);
                        ItemManager.database.itemProperties = new InventoryItemPropertyDeprecated[0];

                        ReplaceOldTypes<InventoryEquipTypeDeprecated, EquipmentType>(ItemManager.database.equipTypes, (arr) => ItemManager.database.equipmentTypes = arr);
                        ItemManager.database.equipTypes = new InventoryEquipTypeDeprecated[0];

                        ReplaceOldTypes<InventoryCurrencyDeprecated, CurrencyDefinition>(ItemManager.database.inventoryCurrencies, (arr) => ItemManager.database.currencies = arr);
                        ItemManager.database.inventoryCurrencies = new InventoryCurrencyDeprecated[0];

                        ConvertCraftingCategories();

                        UnityEditor.EditorUtility.SetDirty(ItemManager.database);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    })));
            }
        }

        private void ConvertCraftingCategories()
        {
            ReplaceOldTypes<InventoryCraftingCategoryDeprecated, CraftingCategory>(ItemManager.database.inventoryCraftingCategories, val => ItemManager.database.craftingCategories = val);

            for (int i = 0; i < ItemManager.database.inventoryCraftingCategories.Length; i++)
            {
                var c = ItemManager.database.craftingCategories[i];
                ReplaceOldTypes<InventoryCraftingBlueprintDeprecated, CraftingBlueprint>(ItemManager.database.inventoryCraftingCategories[i].blueprints,
                    val =>
                    {
                        c.blueprints = val;
                    });
            }

            ItemManager.database.inventoryCraftingCategories = new InventoryCraftingCategoryDeprecated[0];
        }

        private static void ReplaceOldTypes<T1, T2>(T1[] old, Action<T2[]> callbackNew)
            where T1 : class, new()
            where T2 : ScriptableObject
        {
            var newArr = new T2[old.Length];
            for (int i = 0; i < newArr.Length; i++)
            {
                var scriptableObject = ScriptableObjectUtility.CreateAsset<T2>(InventoryScriptableObjectUtility.GetSaveFolderForType(typeof (T2)), DateTime.Now.ToFileTimeUtc() + ".asset", false);
                ReflectionUtility.CopySerializableValues(old[i], scriptableObject);

                UnityEditor.EditorUtility.SetDirty(scriptableObject);
                newArr[i] = scriptableObject;
            }

            callbackNew(newArr);
        }
    }
}

#pragma warning restore 618