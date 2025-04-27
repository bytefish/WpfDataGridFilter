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
        public Dictionary<FilterType, IFilterTranslator> FilterTranslators => _registrations;

        private Dictionary<FilterType, IFilterTranslator> _registrations => new();

        public FilterTranslatorProvider(params IFilterTranslator[] filterTranslators)
        {
            AddOrReplace(new BooleanFilterTranslator());
            AddOrReplace(new DateTimeFilterTranslator());
            AddOrReplace(new DoubleNumericFilterTranslator());
            AddOrReplace(new IntNumericFilterTranslator());
            AddOrReplace(new StringFilterTranslator());
        }

        /// <summary>
        /// Adds or Replaces a Filter.
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="filterTranslator"></param>
        public void AddOrReplace(IFilterTranslator filterTranslator)
        {
            FilterTranslators[filterTranslator.FilterType] = filterTranslator;
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

        public IFilterTranslator[] GetAllFilterTranslators()
        {
            return FilterTranslators.Values.ToArray();
        }
    }
}
