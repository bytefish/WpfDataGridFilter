// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Filters
{
    /// <summary>
    /// Returns the Current FilterState.
    /// </summary>
    public class DataGridStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The DataGridState, that has changed.
        /// </summary>
        public required DataGridState DataGridState { get; set; }
    }

    /// <summary>
    /// Holds the DataGrid State with Filters and Sort Column.
    /// </summary>
    public class DataGridState
    {
        /// <summary>
        /// Invoked, when the Filter Changed.
        /// </summary>
        public event EventHandler<DataGridStateChangedEventArgs>? DataGridStateChanged;

        /// <summary>
        /// The current Filters applied to the Result Set.
        /// </summary>
        public Dictionary<string, FilterDescriptor> Filters { get; set; } = [];

        /// <summary>
        /// Numbers of Elements to Skip.
        /// </summary>
        public int? Skip { get; set; }
        
        /// <summary>
        /// Numbers of Elements to Select.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// The Current Sort Column, that is filtered upon.
        /// </summary>
        public SortColumn? SortColumn { get; set; }
        
        /// <summary>
        /// Gets a Typed Filter from the Filter Descriptor Dictionary.
        /// </summary>
        /// <param name="filter"></param>
        public bool TryGetFilter<TFilter>(string propertyName, [NotNullWhen(true)] out TFilter? filter)
            where TFilter : FilterDescriptor
        {
            filter = null;

            if(!Filters.TryGetValue(propertyName, out FilterDescriptor? filterDescriptor))
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
        /// Sets the Sort Column.
        /// </summary>
        /// <param name="sortColumn">Column to Sort</param>
        public void SetSortColumn(SortColumn sortColumn)
        {
            SortColumn = sortColumn;

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }

        /// <summary>
        /// Applies a Filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(FilterDescriptor filter)
        {
            Filters[filter.PropertyName] = filter;

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }

        /// <summary>
        /// Removes a Filter.
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveFilter(string propertyName)
        {
            Filters.Remove(propertyName, out var _);

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }
    }
}