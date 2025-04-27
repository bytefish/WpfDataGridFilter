// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.Infrastructure
{
    public interface IFilterControlProvider
    {
        /// <summary>
        /// Creates a new <see cref="IFilterControl"> for the given Filter Type.
        /// </summary>
        /// <param name="filterType">Filter Type to create</param>
        /// <returns>The <see cref="IFilterControl"/></returns>
        FilterControl CreateFilterControl(FilterType filterType);
    }
}