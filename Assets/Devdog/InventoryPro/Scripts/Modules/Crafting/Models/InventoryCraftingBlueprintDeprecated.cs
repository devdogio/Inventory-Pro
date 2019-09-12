using UnityEngine;
using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using Devdog.General;

namespace Devdog.InventoryPro
{
    [System.Serializable]
    [Obsolete("Replaced by CraftingBlueprint")]
    public partial class InventoryCraftingBlueprintDeprecated
    {
        /// <summary>
        /// The unique ID of this object. Note that this is NOT the index inside the category.
        /// </summary>
        [HideInInspector]
        public int ID;

//        /// <summary>
//        /// The index inside the category.
//        /// </summary>
//        [HideInInspector]
//        public int indexInCategory;

        ///// <summary>
        ///// The category, convenience property.
        ///// </summary>
        //public InventoryCraftingCategory category
        //{
        //    get
        //    {
        //        return ItemManager.database.craftingCategories[categoryID];
        //    }
        //}

        /// <summary>
        /// Use the name of the item instead of a custom crafting name
        /// </summary>
        public bool useItemResultNameAndDescription = true;


        /// <summary>
        /// Name of the blueprint.
        /// </summary>
        public string name
        {
            get
            {
                if (useItemResultNameAndDescription)
                {
                    if (resultItems.Length == 0)
                    {
                        return string.Empty;
                    }
                    var items = resultItems.Where(o => o.item != null);
                    if (items.Any() == false)
                    {
                        return string.Empty;
                    }

                    return items.Select(o => o.item.name).Aggregate((a, b) => a + ", " + b);
                }

                return customName;
            }
        }

        /// <summary>
        /// Description of the blueprint.
        /// </summary>
        public string description
        {
            get
            {
                if (useItemResultNameAndDescription)
                {
                    if (resultItems.Length == 0)
                    {
                        return string.Empty;
                    }

                    return resultItems.First().item != null ? resultItems.First().item.description : string.Empty;
                }

                return customDescription;
            }
        }


        /// <summary>
        /// Crafting name, ignored if useItemResultNameAndDescription = true
        /// </summary>
        public string customName;

        /// <summary>
        /// Crafting description, ignored if useItemResultNameAndDescription = true
        /// </summary>
        public string customDescription;

        
        public bool overrideCategoryAudioClips = false;

        public AudioClipInfo successAudioClip = new AudioClipInfo();
        public AudioClipInfo craftingAudioClip = new AudioClipInfo() { loop = true };
        public AudioClipInfo canceledAudioClip = new AudioClipInfo();
        public AudioClipInfo failedAudioClip = new AudioClipInfo();

        /// <summary>
        /// The items required for this blueprint.
        /// </summary>
        public ItemAmountRow[] requiredItems = new ItemAmountRow[0];

        /// <summary>
        /// Usage requirement properties. For example, the player needs at least 10 strength to craft this item.
        /// </summary>
        public StatRequirement[] ssageRequirement = new StatRequirement[0];

        /// <summary>
        /// Can we craft this item already? disable if you want to unlock it through code.
        /// </summary>
        public bool playerLearnedBlueprint = true;
    
        /// <summary>
        /// The price to craft this item once.
        /// </summary>
        public CurrencyDecorator craftingCost;

        /// <summary>
        /// The success factor 0.0f will always fail, while 1.0f will always succeed.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float successChanceFactor = 1.0f;

        /// <summary>
        /// The item gained after crafting.
        /// </summary>
        public ItemAmountRow[] resultItems = new ItemAmountRow[0];

//        /// <summary>
//        /// Amount of items you get when craft succeeded.
//        /// </summary>
//        public int itemResultCount = 1;

        /// <summary>
        /// How many seconds does it take to craft the item?
        /// </summary>
        public float craftingTimeDuration = 0.0f;

        /// <summary>
        /// How much faster does crafting become after an item is created?
        /// </summary>
        public float craftingTimeSpeedupFactor = 1.0f;

        /// <summary>
        /// The maximum speed the crafting can be spedup?
        /// </summary>
        public float craftingTimeSpeedupMax = 1.0f;

        /// <summary>
        /// Games like minecraft have a layout. Items have to be placed in a certain order to unlock the craft.
        /// [] #1 first is for all layouts
        /// [] #2 is for horizontal row in layout
        /// [] #3 is for vertical column in layout
        /// </summary>
        public CraftingBlueprintLayout[] blueprintLayouts = new CraftingBlueprintLayout[0];





//        public virtual bool IsLayoutValid(InventoryItemBase[] items)
//        {
//            Debug.LogWarning("Not written yet!");
//            return true;
//        }

        public override string ToString()
        {
            return name;
        }
    }
}