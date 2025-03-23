// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Translations
{
    /// <summary>
    /// Translations.
    /// </summary>
    public interface ITranslations
    {
        /// <summary>
        /// Translation for the Apply Button.
        /// </summary>
        string ApplyButton { get; }
        
        /// <summary>
        /// Translation for the Reset Button.
        /// </summary>
        string ResetButton { get; }

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
    /// An Enumeration Translation
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumTranslation<TEnum>
    {
        /// <summary>
        /// Enumeration Value.
        /// </summary>
        public required TEnum Value { get; set; }

        /// <summary>
        /// Enumeration Translation.
        /// </summary>
        public required string Translation { get; set; }
    }

    /// <summary>
    /// Neutral Translations
    /// </summary>
    public class NeutralTranslations : ITranslations
    {

        public string ApplyButton => "Apply";

        public string ResetButton => "Reset";

        public IReadOnlyList<EnumTranslation<FilterOperatorEnum>> FilterOperatorTranslations =>
        [
            new () { Value = FilterOperatorEnum.None, Translation =  "None" },
            new () { Value = FilterOperatorEnum.All, Translation =  "All" },
            new () { Value = FilterOperatorEnum.IsEqualTo, Translation =  "Is Equal To" },
            new () { Value = FilterOperatorEnum.IsNotEqualTo, Translation = "Is Not Equal To" },
            new () { Value = FilterOperatorEnum.IsLessThan, Translation = "Is Less Than" },
            new () { Value = FilterOperatorEnum.IsGreaterThan, Translation = "Is Greater Than" },
            new () { Value = FilterOperatorEnum.IsLessThanOrEqualTo, Translation = "Is Less Than or Equal To" },
            new () { Value = FilterOperatorEnum.IsGreaterThanOrEqualTo, Translation = "Is Greater Than or Equal To" },
            new () { Value = FilterOperatorEnum.NotContains, Translation = "Does Not Contain" },
            new () { Value = FilterOperatorEnum.Contains, Translation = "Contains" },
            new () { Value = FilterOperatorEnum.StartsWith, Translation = "Starts with" },
            new () { Value = FilterOperatorEnum.EndsWith, Translation = "Ends with" },
            new () { Value = FilterOperatorEnum.BetweenInclusive, Translation = "Between (Inclusive)" },
            new () { Value = FilterOperatorEnum.BetweenExclusive, Translation = "Between (Exclusive)" },
            new () { Value = FilterOperatorEnum.Yes, Translation = "Yes" },
            new () { Value = FilterOperatorEnum.No, Translation = "No" },
            new () { Value = FilterOperatorEnum.IsNull, Translation = "Is Null" },
            new () { Value = FilterOperatorEnum.IsNotNull, Translation = "Is Not Null" },
            new () { Value = FilterOperatorEnum.IsEmpty, Translation = "Is Empty" },
            new () { Value = FilterOperatorEnum.IsNotEmpty, Translation = "Is Not Empty" },
            new () { Value = FilterOperatorEnum.Before, Translation = "Before" },
            new () { Value = FilterOperatorEnum.After, Translation = "After" },
        ];

        public IReadOnlyList<EnumTranslation<SortDirectionEnum>> SortDirectionTranslations =>
        [
            new ()  { Value = SortDirectionEnum.Ascending, Translation =  "Ascending" },
            new ()  { Value = SortDirectionEnum.Descending, Translation =  "Descending" },
        ];

    }
}
