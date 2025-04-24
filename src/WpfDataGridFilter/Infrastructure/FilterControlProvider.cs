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
        private readonly Dictionary<string, Func<FilterControl>> _filterControls = [];

        public FilterControlProvider()
        {
            _filterControls.Add(FilterTypes.BooleanFilter, () => new BooleanFilterControl());
            _filterControls.Add(FilterTypes.DateTimeFilter, () => new DateTimeFilterControl());
            _filterControls.Add(FilterTypes.IntNumericFilter, () => new IntNumericFilterControl());
            _filterControls.Add(FilterTypes.DoubleNumericFilter, () => new DoubleNumericFilterControl());
            _filterControls.Add(FilterTypes.StringFilter, () => new StringFilterControl());
        }

        public void AddFilterControl(string name, Func<FilterControl> filterControlFunc)
        {
            if (_filterControls.ContainsKey(name))
            {
                throw new InvalidOperationException($"The Provider already contains a Filter Control for '{name}'");
            }

            _filterControls.Add(name, filterControlFunc);
        }

        public FilterControl CreateFilterControl(string filterType)
        {
            if(!_filterControls.TryGetValue(filterType, out var filterControlFunc))
            {
                throw new InvalidOperationException($"No Filter Control for Name '{filterType}' available");
            }

            return filterControlFunc.Invoke();
        }
    }
}