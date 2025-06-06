﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter
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
        /// The current Filters applied to the Result Set.
        /// </summary>
        public IReadOnlyDictionary<string, FilterDescriptor> Filters => _filters;

        /// <summary>
        /// Numbers of Elements to Skip.
        /// </summary>
        public int? Skip { get; private set; }
        
        /// <summary>
        /// Numbers of Elements to Select.
        /// </summary>
        public int? Top { get; private set; }

        /// <summary>
        /// The Current Sort Column, that is filtered upon.
        /// </summary>
        public SortColumn? SortColumn { get; private set; }

        /// <summary>
        /// Invoked, when the Filter Changes.
        /// </summary>
        public event EventHandler<DataGridStateChangedEventArgs>? DataGridStateChanged;

        /// <summary>
        /// Creates a new DataGridState from Existing Values
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <param name="sortColumn">Sort Columns</param>
        /// <param name="skip">Skipped Elements</param>
        /// <param name="top">Top Elements</param>
        public DataGridState(List<FilterDescriptor> filters, SortColumn? sortColumn = null, int? skip = null, int? top = null)
        {
            _filters = filters.ToDictionary(x => x.PropertyName, x => x);
            SortColumn = sortColumn;
            Skip = skip;
            Top = top;
        }

        // Current Filters
        private Dictionary<string, FilterDescriptor> _filters = new();

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
        /// Sets the Top Value.
        /// </summary>
        /// <param name="sortColumn">Column to Sort</param>
        public void SetSkipTop(int? skip, int? top)
        {
            Skip = skip;
            Top = top;

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }

        /// <summary>
        /// Applies a Filter.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(FilterDescriptor filter)
        {
            _filters[filter.PropertyName] = filter;

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }

        /// <summary>
        /// Removes a Filter.
        /// </summary>
        /// <param name="propertyName"></param>
        public void RemoveFilter(string propertyName)
        {
            _filters.Remove(propertyName, out var _);

            DataGridStateChanged?.Invoke(this, new DataGridStateChangedEventArgs { DataGridState = this });
        }
    }
}