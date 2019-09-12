using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.InventoryPro
{
    [Serializable]
    public partial class ItemFilter
    {
        public enum RestrictionType
        {
            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Type,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Category,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Rarity,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Stat,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Droppable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Sellable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Storable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Weight
        }

        [Flags]
        public enum FilterType
        {
            Equal,
            NotEqual,
            GreatherThan,
            LessThan
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class AllowedFiltersAttribute : Attribute
        {

            public AllowedFiltersAttribute(FilterType types)
            {
                
            }
        }


        public RestrictionType restrictionType;
        public FilterType filterType;

        public ItemCategory categoryValue;
        public StatDefinition statDefinitionValue;
        public ItemRarity rarityValue;

        public string stringValue;
        public bool boolValue;
        public float floatValue;

        public Type typeValue
        {
            get
            {
                return Type.GetType(stringValue);
            }
        }

        public ItemFilter()
            : this(RestrictionType.Type, FilterType.Equal)
        {

        }

        public ItemFilter(RestrictionType restrictionType, FilterType filterType)
        {
            this.restrictionType = restrictionType;
            this.filterType = filterType;
        }


        public bool IsItemAbidingFilter(InventoryItemBase item)
        {
            switch (restrictionType)
            {
                case RestrictionType.Type:
                    return VerifyFilter(item, typeValue, filterType);
                case RestrictionType.Category:
                    return VerifyFilterScriptableObject(item.category, categoryValue, filterType);
                case RestrictionType.Rarity:
                    return VerifyFilterScriptableObject(item.rarity, rarityValue, filterType);
                case RestrictionType.Stat:
                    return VerifyFilter(item.stats, statDefinitionValue, filterType);
                case RestrictionType.Droppable:
                    return VerifyFilterEqualNotEqual(item.isDroppable, boolValue, filterType);
                case RestrictionType.Sellable:
                    return VerifyFilterEqualNotEqual(item.isSellable, boolValue, filterType);
                case RestrictionType.Storable:
                    return VerifyFilterEqualNotEqual(item.isStorable, boolValue, filterType);
                case RestrictionType.Weight:
                    return VerifyFilterFloats(item.weight, floatValue, filterType);
                default:
                    Debug.LogWarning("Type " + restrictionType + " not found");
                    break;
            }

            return false;
        }

        private bool VerifyFilter(StatDecorator[] stats, StatDefinition stat, FilterType filterType)
        {
            switch (filterType)
            {
                case FilterType.Equal:
                    return stats.Any(o => o.stat.ID == stat.ID);
                case FilterType.NotEqual:
                    return stats.All(o => o.stat.ID != stat.ID);
                case FilterType.LessThan:
                    var prop = stats.FirstOrDefault(o => o.stat.ID == stat.ID);
                    if (prop == null)
                        return true; // None is also considered less than... Use Equal instead.

                    return prop.floatValue < floatValue;
                case FilterType.GreatherThan:
                    var prop2 = stats.FirstOrDefault(o => o.stat.ID == stat.ID);
                    if (prop2 == null)
                        return false;

                    return prop2.floatValue > floatValue;
                default:

                    break;
            }

            return false;
        }

        public bool VerifyFilterEqualNotEqual(object a, object b, FilterType filterType)
        {
            if (filterType == FilterType.Equal)
                return a.Equals(b);

            if (filterType == FilterType.NotEqual)
                return !a.Equals(b);

            //Debug.LogWarning("Only equal and not equal are supported for this check (" + a.ToString() + " given)");
            return false;
        }

        public bool VerifyFilterScriptableObject(ScriptableObject a, ScriptableObject b, FilterType filterType)
        {
            if (filterType == FilterType.Equal)
                return a == b;

            if (filterType == FilterType.NotEqual)
                return a != b;

            return false;
        }

        public bool VerifyFilter(InventoryItemBase item, System.Type type, FilterType filterType)
        {
            return VerifyFilterEqualNotEqual(item.GetType(), type, filterType);
        }
        public bool VerifyFilter(InventoryItemBase item, ItemCategory category, FilterType filterType)
        {
            return VerifyFilterEqualNotEqual(item.category, category, filterType);
        }
        public bool VerifyFilterInts(int a, int b, FilterType filterType)
        {
            bool equality = VerifyFilterEqualNotEqual(a, b, filterType);
            if (equality)
                return equality;

            if (filterType == FilterType.GreatherThan)
                return a > b;

            if (filterType == FilterType.LessThan)
                return a < b;

            return false;
        }

        public bool VerifyFilterFloats(float a, float b, FilterType filterType)
        {
            bool equality = VerifyFilterEqualNotEqual(a, b, filterType);
            if (equality)
                return equality;

            if (filterType == FilterType.GreatherThan)
                return a > b;

            if (filterType == FilterType.LessThan)
                return a < b;

            return false;
        }
    }
}
