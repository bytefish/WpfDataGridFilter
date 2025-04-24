// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WpfDataGridFilter.Infrastructure
{
    public interface IFilterControlProvider
    {
        /// <summary>
        /// Creates a new <see cref="IFilterControl"> for the given Fitler Type.
        /// </summary>
        /// <param name="filterType">Filter Type to create</param>
        /// <returns>The <see cref="IFilterControl"/></returns>
        FilterControl CreateFilterControl(string name);
    }
}