// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.DynamicLinq.Translators;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The default FilterProvider implementation.
    /// </summary>
    public class FilterTranslatorProvider : IFilterTranslatorProvider
    {
        public readonly IReadOnlyDictionary<FilterType, IFilterTranslator> FilterTranslators;

        public FilterTranslatorProvider(params IFilterTranslator[] filterTranslators)
        {
            FilterTranslators = filterTranslators.ToDictionary(x => x.FilterType, x => x);
        }

        /// <inheritdoc/>
        public IFilterTranslator GetFilterTranslator(FilterType filterType)
        {
            if (!FilterTranslators.ContainsKey(filterType))
            {
                throw new InvalidOperationException($"No FilterTranslator with Type '{filterType.Name}' available");
            }

            return FilterTranslators[filterType];
        }

        /// <summary>
        /// Returns a <see cref="IFilterTranslatorProvider"/> with the Default Converters and additional 
        /// Converters supplied by the user.
        /// </summary>
        /// <param name="additionalFilters">Additional Filters</param>
        /// <returns>The Provider with the Default and optional user-supplied filters</returns>
        public static IFilterTranslatorProvider GetDefault(params IFilterTranslator[] additionalFilters)
        {
            IFilterTranslator[] filterTranslators =
            [
                .. DefaultFilterTranslators,
                .. additionalFilters
            ];

            return new FilterTranslatorProvider(filterTranslators);
        }

        /// <summary>
        /// Returns the List of Default Filters the library provides.
        /// </summary>
        public static IFilterTranslator[] DefaultFilterTranslators =>
        [
            new BooleanFilterTranslator(),
            new DateTimeFilterTranslator(),
            new DoubleNumericFilterTranslator(),
            new IntNumericFilterTranslator(),
            new StringFilterTranslator(),
        ];
    }
}
