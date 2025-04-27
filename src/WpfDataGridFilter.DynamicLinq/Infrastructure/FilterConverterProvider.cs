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

        public static IFilterConverterProvider GetDefault(params FilterConverter[] additionalFilters)
        {
            FilterConverter[] filterConverters =
            [
                .. DefaultFilterConverters,
                .. additionalFilters
            ];

            return new FilterConverterProvider(filterConverters);
        }

        /// <inheritdoc/>
        public FilterConverter GetFilterConverterByName(string name)
        {
            if(!FilterConverters.ContainsKey(name))
            {
                throw new InvalidOperationException($"No FilterConverter with Name '{name}' available");
            }

            return FilterConverters[name];
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
