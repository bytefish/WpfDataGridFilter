// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Controls;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.Infrastructure
{
    /// <summary>
    /// Default <see cref="IFilterControlProvider"> implementation.
    /// </summary>
    public class FilterControlProvider : IFilterControlProvider
    {
        private readonly Dictionary<FilterType, Func<FilterControl>> _filterControls = [];

        public FilterControlProvider()
        {
            _filterControls.Add(FilterType.BooleanFilter, () => new BooleanFilterControl());
            _filterControls.Add(FilterType.DateTimeFilter, () => new DateTimeFilterControl());
            _filterControls.Add(FilterType.IntNumericFilter, () => new IntNumericFilterControl());
            _filterControls.Add(FilterType.DoubleNumericFilter, () => new DoubleNumericFilterControl());
            _filterControls.Add(FilterType.StringFilter, () => new StringFilterControl());
        }

        public void AddFilterControl(FilterType filterType, Func<FilterControl> filterControlFunc)
        {
            if (_filterControls.ContainsKey(filterType))
            {
                throw new InvalidOperationException($"The Provider already contains a Filter Control for '{filterType.Name}'");
            }

            _filterControls.Add(filterType, filterControlFunc);
        }

        public FilterControl CreateFilterControl(FilterType filterType)
        {
            if(!_filterControls.TryGetValue(filterType, out var filterControlFunc))
            {
                throw new InvalidOperationException($"No Filter Control for Name '{filterType.Name}' available");
            }

            return filterControlFunc.Invoke();
        }
    }
}