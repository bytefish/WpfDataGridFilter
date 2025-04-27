// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public interface ISortTranslator
    {
        /// <summary>
        /// Sorts a given IQueryable by the given sort column.
        /// </summary>
        /// <typeparam name="TEntity">Entity we are sorting</typeparam>
        /// <param name="source">IQueryable with the available Entities</param>
        /// <param name="sortColumn">Sort Column to sort for</param>
        /// <returns>The sorted IQueryable</returns>
        IQueryable<TEntity> Sort<TEntity>(IQueryable<TEntity> source, SortColumn sortColumn);
    }
}
