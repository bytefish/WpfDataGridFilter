// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfDataGridFilter.Models
{
    /// <summary>
    /// A SortColumn in a Filter.
    /// </summary>
    public class SortColumn
    {
        public readonly string PropertyName;

        public readonly SortDirectionEnum? SortDirection;

        public SortColumn(string propertyName, SortDirectionEnum? sortDirection)
        {
            PropertyName = propertyName;
            SortDirection = sortDirection;
        }
    }
}