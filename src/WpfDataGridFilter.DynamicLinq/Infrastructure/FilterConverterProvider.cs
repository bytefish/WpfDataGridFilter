using WpfDataGridFilter.DynamicLinq.Converters;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The default FilterProvider implementation.
    /// </summary>
    public class FilterConverterProvider : IFilterConverterProvider
    {
        public readonly IReadOnlyDictionary<string, FilterConverter> FilterConverters;

        public FilterConverterProvider(params FilterConverter[] filterConverters)
        {
            FilterConverters = filterConverters.ToDictionary(x => x.FilterType, x => x);
        }

        /// <inheritdoc/>
        public FilterConverter GetFilterConverterByName(string name)
        {
            if (!FilterConverters.ContainsKey(name))
            {
                throw new InvalidOperationException($"No FilterConverter with Name '{name}' available");
            }

            return FilterConverters[name];
        }

        /// <summary>
        /// Returns a <see cref="IFilterConverterProvider"/> with the Default Converters and additional 
        /// Converters supplied by the user.
        /// </summary>
        /// <param name="additionalFilters">Additional Filters</param>
        /// <returns>The Provider with the Default and optional user-supplied filters</returns>
        public static IFilterConverterProvider GetDefault(params FilterConverter[] additionalFilters)
        {
            FilterConverter[] filterConverters =
            [
                .. DefaultFilterConverters,
                .. additionalFilters
            ];

            return new FilterConverterProvider(filterConverters);
        }

        /// <summary>
        /// Returns the List of Default Filters the library provides.
        /// </summary>
        public static FilterConverter[] DefaultFilterConverters =>
        [
            new BooleanFilterConverter(),
            new DateTimeFilterConverter(),
            new DoubleNumericFilterConverter(),
            new IntNumericFilterConverter(),
            new StringFilterConverter(),
        ];


    }

}
