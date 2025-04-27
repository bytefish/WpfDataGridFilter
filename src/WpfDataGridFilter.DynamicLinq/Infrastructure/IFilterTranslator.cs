// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    /// <summary>
    /// The abstract base class for all FilterDescriptor Translators.
    /// </summary>
    public interface IFilterTranslator
    {
        /// <summary>
        /// Applies the given Filter Descriptor to the given IQueryable.
        /// </summary>
        /// <param name="source">Source Queryable to provide unfiltered data</param>
        /// <param name="filterDescriptor">FilterDescriptor to apply on the data</param>
        /// <returns>An IQueryable with the Filters applied</returns>
        IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor);

        /// <summary>
        /// FilterType this filter applies to.
        /// </summary>
        string FilterType { get; }
    }
}
