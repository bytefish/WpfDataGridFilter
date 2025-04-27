using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Converters
{
    /// <summary>
    /// The abstract base class for all FilterDescriptor handlers.
    /// </summary>
    public abstract class FilterConverter
    {
        /// <summary>
        /// Applies the given Filter Descriptor to the given IQueryable.
        /// </summary>
        /// <param name="source">Source Queryable to provide unfiltered data</param>
        /// <param name="filterDescriptor">FilterDescriptor to apply on the data</param>
        /// <returns>An IQueryable with the Filters applied</returns>
        public abstract IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor);

        /// <summary>
        /// FilterType this filter applies to.
        /// </summary>
        public abstract string FilterType { get; }
    }


}
