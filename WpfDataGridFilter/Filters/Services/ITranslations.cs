﻿using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Filters.Services
{
    /// <summary>
    /// An Enumeration Translation
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumTranslation<TEnum>
    {
        public required TEnum Value { get; set; }

        public required string Translation { get; set; }
    }

    /// <summary>
    /// Translations.
    /// </summary>
    public interface ITranslations
    {
        /// <summary>
        /// Translations for Filter Operators.
        /// </summary>
        IReadOnlyList<EnumTranslation<FilterOperatorEnum>> FilterOperatorTranslations { get; }

        /// <summary>
        /// Translations for Filter Operators.
        /// </summary>
        IReadOnlyList<EnumTranslation<SortDirectionEnum>> SortDirectionTranslations { get; }
    }

    /// <summary>
    /// Neutral Translations
    /// </summary>
    public class NeutralTranslations : ITranslations
    {
        public IReadOnlyList<EnumTranslation<FilterOperatorEnum>> FilterOperatorTranslations =>
        [
            new ()  { Value = FilterOperatorEnum.None, Translation =  "None" },
            new ()  { Value = FilterOperatorEnum.All, Translation =  "All" },
            new ()  { Value = FilterOperatorEnum.IsEqualTo, Translation =  "Is Equal To" },
            new ()  { Value = FilterOperatorEnum.IsNotEqualTo, Translation = "Is Not Equal To" },
            new ()  { Value = FilterOperatorEnum.IsLessThan, Translation = "Is Less Than or Equal To" },
            new ()  { Value = FilterOperatorEnum.IsGreaterThan, Translation = "Is Greater Than or Equal To" },
            new ()  { Value = FilterOperatorEnum.IsLessThanOrEqualTo, Translation = "Is Less Than or Equal To" },
            new ()  { Value = FilterOperatorEnum.IsGreaterThanOrEqualTo, Translation = "Is Greater Than or Equal To" },
            new ()  { Value = FilterOperatorEnum.NotContains, Translation = "Does Not Contain" },
            new ()  { Value = FilterOperatorEnum.Contains, Translation = "Contains" },
            new ()  { Value = FilterOperatorEnum.StartsWith, Translation = "Starts with" },
            new ()  { Value = FilterOperatorEnum.EndsWith, Translation = "Ends with" },
            new ()  { Value = FilterOperatorEnum.BetweenInclusive, Translation = "Between (Inclusive)" },
            new ()  { Value = FilterOperatorEnum.BetweenExclusive, Translation = "Between (Exclusive)" },
            new ()  { Value = FilterOperatorEnum.Yes, Translation = "Yes" },
            new ()  { Value = FilterOperatorEnum.No, Translation = "No" },
            new ()  { Value = FilterOperatorEnum.IsNull, Translation = "Is Null" },
            new ()  { Value = FilterOperatorEnum.IsNotNull, Translation = "Is Not Null" },
        ];

        public IReadOnlyList<EnumTranslation<SortDirectionEnum>> SortDirectionTranslations =>
        [
            new ()  { Value = SortDirectionEnum.Ascending, Translation =  "Ascending" },
            new ()  { Value = SortDirectionEnum.Descending, Translation =  "Descending" },
        ];
    }
}
