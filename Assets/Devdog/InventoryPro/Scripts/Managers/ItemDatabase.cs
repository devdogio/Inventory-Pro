using System;
using Devdog.InventoryPro;
using UnityEngine;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.Serialization;

namespace Devdog.InventoryPro
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemDatabase.asset", menuName = InventoryPro.CreateAssetMenuPath + "Item Database")]
    public partial class ItemDatabase : ScriptableObject
    {
        [Header("Items")]
        public InventoryItemBase[] items = new InventoryItemBase[0];
        public ItemRarity[] rarities = new ItemRarity[0];
        public ItemCategory[] categories = new ItemCategory[0];
        public StatDefinition[] statDefinitions = new StatDefinition[0];

        [Header("Equipment")]
        public EquipmentType[] equipmentTypes = new EquipmentType[0];

        [Header("Crafting")]
        public CraftingCategory[] craftingCategories = new CraftingCategory[0];

        [Header("Currencies")]
        public CurrencyDefinition[] currencies = new CurrencyDefinition[0];


#pragma warning disable 618

        [Header(" -- Deprecated")]

        [Obsolete("Replaced by rarities")]
        public InventoryItemRarityDeprecated[] itemRarities = new InventoryItemRarityDeprecated[0];
        [Obsolete("Replaced by categories")]
        public InventoryItemCategoryDeprecated[] itemCategories = new InventoryItemCategoryDeprecated[0];
        [Obsolete("Replaced by properties"), FormerlySerializedAs("properties")]
        public InventoryItemPropertyDeprecated[] itemProperties = new InventoryItemPropertyDeprecated[0];
        
        [Obsolete("Replaced by equipmentTypes")]
        public InventoryEquipTypeDeprecated[] equipTypes = new InventoryEquipTypeDeprecated[0];

        [Obsolete("Replaced by craftCategories")]
        [FormerlySerializedAs("craftingCategories")]
        public InventoryCraftingCategoryDeprecated[] inventoryCraftingCategories = new InventoryCraftingCategoryDeprecated[0];

        [Obsolete("Replaced by currencies")]
        [FormerlySerializedAs("currencies")]
        public InventoryCurrencyDeprecated[] inventoryCurrencies = new InventoryCurrencyDeprecated[0];



#pragma warning restore 618


        // Convenience methods:
        public string[] pluralCurrenciesStrings
        {
            get
            {
                return currencies.Select(o => o.pluralName).ToArray();
            }
        }
        public string[] singleCurrenciesStrings
        {
            get
            {
                return currencies.Select(o => o.singleName).ToArray();
            }
        }

        public string[] craftingCategoriesStrings
        {
            get
            {
                return craftingCategories.Select(o => o.name).ToArray();
            }
        }

        public string[] propertiesStrings
        {
            get
            {
                return statDefinitions.Select(o => o.statName).ToArray();
            }
        }
        public string[] rarityStrings
        {
            get
            {
                return rarities.Select(o => o.name).ToArray();
            }
        }

        public Color[] itemRaritiesColors
        {
            get
            {
                return rarities.Select(o => o.color).ToArray();
            }
        }
        public string[] itemCategoriesStrings
        {
            get
            {
                return categories.Select(o => o.name).ToArray();
            }
        }

        public string[] equipmentTypesStrings
        {
            get
            {
                return equipmentTypes.Select(o => o.name).ToArray();
            }
        }
    }
}
