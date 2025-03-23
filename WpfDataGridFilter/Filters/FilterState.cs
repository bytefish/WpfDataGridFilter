// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Filters
{
    /// <summary>
    /// Holds state to represent filters in a <see cref="FluentDataGrid{TGridItem}"/>.
    /// </summary>
    public class FilterState
    {
        /// <summary>
        /// Returns the Current FilterState.
        /// </summary>
        public class FilterStateChangedEventArgs : EventArgs
        {
            public required FilterState FilterState { get; set; }
        }

        /// <summary>
        /// Invoked, when the Filter Changed.
        /// </summary>
        public event EventHandler<FilterStateChangedEventArgs>? FilterStateChanged;
    
        /// <summary>
        /// Read-Only View of Column Filters.
        /// </summary>
        public IReadOnlyDictionary<string, FilterDescriptor> Filters => _filters;

        /// <summary>
        /// Column Filters.
        /// </summary>
        private readonly ConcurrentDictionary<string, FilterDescriptor> _filters = new();


        /// <summary>
        /// Gets a Typed Filter from the Filter Descriptor Dictionary.
        /// </summary>
        /// <param name="filter"></param>
        public bool TryGetFilter<TFilter>(string propertyName, [NotNullWhen(true)] out TFilter? filter)
            where TFilter : FilterDescriptor
        {
            filter = null;

            if(!_filters.TryGetValue(propertyName, out FilterDescriptor? filterDescriptor))
            {
                return false;
            }

            if (filterDescriptor is TFilter typedFilterDescriptor)
            {
                filter = typedFilterDescriptor;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies a Filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(FilterDescriptor filter)
        {
            _filters[filter.PropertyName] = filter;

            FilterStateChanged?.Invoke(this, new FilterStateChangedEventArgs { FilterState = this });
        }

        /// <summary>
        /// Removes a Filter.
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveFilter(string propertyName)
        {
            _filters.Remove(propertyName, out var _);

            FilterStateChanged?.Invoke(this, new FilterStateChangedEventArgs { FilterState = this });
        }
    }
}