// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using WpfDataGridFilter.Models;

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
        IReadOnlyList<Translation<FilterOperator>> FilterOperatorTranslations { get; }

        /// <summary>
        /// Translations for Filter Operators.
        /// </summary>
        IReadOnlyList<Translation<SortDirectionEnum>> SortDirectionTranslations { get; }
    }

    /// <summary>
    /// An Enumeration Translation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record Translation<T>
    {
        /// <summary>
        /// Enumeration Value.
        /// </summary>
        public required T Value { get; set; }

        /// <summary>
        /// Enumeration Translation.
        /// </summary>
        public required string Text { get; set; }
    }

    /// <summary>
    /// Neutral Translations
    /// </summary>
    public class NeutralTranslations : ITranslations
    {
        public string ApplyButton => "Apply";

        public string ResetButton => "Reset";

        public IReadOnlyList<Translation<FilterOperator>> FilterOperatorTranslations =>
        [
            new () { Value = FilterOperator.None, Text =  "None" },
            new () { Value = FilterOperator.All, Text =  "All" },
            new () { Value = FilterOperator.IsEqualTo, Text =  "Is Equal To" },
            new () { Value = FilterOperator.IsNotEqualTo, Text = "Is Not Equal To" },
            new () { Value = FilterOperator.IsLessThan, Text = "Is Less Than" },
            new () { Value = FilterOperator.IsGreaterThan, Text = "Is Greater Than" },
            new () { Value = FilterOperator.IsLessThanOrEqualTo, Text = "Is Less Than or Equal To" },
            new () { Value = FilterOperator.IsGreaterThanOrEqualTo, Text = "Is Greater Than or Equal To" },
            new () { Value = FilterOperator.NotContains, Text = "Does Not Contain" },
            new () { Value = FilterOperator.Contains, Text = "Contains" },
            new () { Value = FilterOperator.StartsWith, Text = "Starts with" },
            new () { Value = FilterOperator.EndsWith, Text = "Ends with" },
            new () { Value = FilterOperator.BetweenInclusive, Text = "Between (Inclusive)" },
            new () { Value = FilterOperator.BetweenExclusive, Text = "Between (Exclusive)" },
            new () { Value = FilterOperator.Yes, Text = "Yes" },
            new () { Value = FilterOperator.No, Text = "No" },
            new () { Value = FilterOperator.IsNull, Text = "Is Null" },
            new () { Value = FilterOperator.IsNotNull, Text = "Is Not Null" },
            new () { Value = FilterOperator.IsEmpty, Text = "Is Empty" },
            new () { Value = FilterOperator.IsNotEmpty, Text = "Is Not Empty" },
            new () { Value = FilterOperator.Before, Text = "Before" },
            new () { Value = FilterOperator.After, Text = "After" },
        ];

        public IReadOnlyList<Translation<SortDirectionEnum>> SortDirectionTranslations =>
        [
            new ()  { Value = SortDirectionEnum.Ascending, Text =  "Ascending" },
            new ()  { Value = SortDirectionEnum.Descending, Text =  "Descending" },
        ];

    }
}
