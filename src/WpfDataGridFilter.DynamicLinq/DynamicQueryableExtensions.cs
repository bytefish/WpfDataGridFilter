// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.DynamicLinq.Translators;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq
{
    /// <summary>
    /// Provides the Dynamic LINQ Converters for translating Filter Descriptors to LINQ statements.
    /// </summary>
    public static class DynamicQueryableExtensions
    {
        /// <summary>
        /// The default Filter Translator as a static field, so we don't create a new one for each call.
        /// </summary>
        private static IFilterTranslatorProvider DefaultFilterTranslatorProvider = new FilterTranslatorProvider();
        
        /// <summary>
        /// The default Sort Translator as a Static Field, so we don't create a new one for each call.
        /// </summary>
        private static ISortTranslator DefaultSortTranslator = new SortTranslator();

        public static int GetTotalItemCount<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterTranslatorProvider? filterTranslatorProvider = null)
        {
            if (filterTranslatorProvider == null)
            {
                filterTranslatorProvider = DefaultFilterTranslatorProvider;
            }

            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            return source
                .ApplyFilters(filters, filterTranslatorProvider)
                .Count();
        }

        /// <summary>
        /// Applies the <see cref="DataGridState"/> to the given <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the Entity to apply the <see cref="DataGridState"/> on</typeparam>
        /// <param name="source">The Data Source to filter for</param>
        /// <param name="dataGridState">The <see cref="DataGridState"/> applied</param>
        /// <param name="filterTranslatorProvider">An optional Provider for FilterTranslators</param>
        /// <returns>The <paramref name="source"> with Filtering, Sorting and Pagination applied</returns>
        public static IQueryable<TEntity> ApplyDataGridState<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterTranslatorProvider? filterTranslatorProvider = null, ISortTranslator? sortTranslator = null)
        {
            if (filterTranslatorProvider == null)
            {
                filterTranslatorProvider = DefaultFilterTranslatorProvider;
            }

            if (sortTranslator == null)
            {
                sortTranslator = DefaultSortTranslator;
            }

            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            IQueryable<TEntity> query = source
                // First Apply the Filters:
                .ApplyFilters(filters, filterTranslatorProvider)
                // Then Sort them by the current Sort Column
                .ApplySort(dataGridState.SortColumn, sortTranslator);

            // Now apply optional Pagination Values
            if(dataGridState.Skip.HasValue)
            {
                query = query.Skip(dataGridState.Skip.Value);
            }

            if(dataGridState.Top.HasValue)
            {
                query = query.Take(dataGridState.Top.Value);
            }

            return query;
        }

        /// <summary>
        /// Applies the Sort to the <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity Type with Sort Properties</typeparam>
        /// <param name="source">Data Source to apply the Sort on</param>
        /// <param name="sortColumn">Column to Sort</param>
        /// <param name="sortTranslator">Optional Translator for custom sorting</param>
        /// <returns>An <see cref="IQueryable{T}"/> with order applied</returns>
        public static IQueryable<TEntity> ApplySort<TEntity>(this IQueryable<TEntity> source, SortColumn? sortColumn, ISortTranslator? sortTranslator = null)
        {
            if(sortColumn == null)
            {
                return source;
            }

            if(sortTranslator == null)
            {
                sortTranslator = DefaultSortTranslator;
            }

            return sortTranslator.Sort(source, sortColumn);
        }

        /// <summary>
        /// Applies a given List of Filters to an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">Entity Type to be filtered for</typeparam>
        /// <param name="source">Source to apply the Filters to</param>
        /// <param name="filterDescriptors">List of FilterDescriptors to apply</param>
        /// <param name="filterTranslatorProvider">Optional Translator to provide custom sorts</param>
        /// <returns>The <paramref name="source"/> with the filters applied</returns>
        public static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> source, ICollection<FilterDescriptor> filterDescriptors, IFilterTranslatorProvider? filterTranslatorProvider = null)
        {
            if (filterDescriptors.Count == 0)
            {
                return source;
            }
            
            if (filterTranslatorProvider == null)
            {
                filterTranslatorProvider = DefaultFilterTranslatorProvider;
            }

            foreach (FilterDescriptor filterDescriptor in filterDescriptors)
            {
                if (filterDescriptor.FilterOperator == FilterOperator.None)
                {
                    continue;
                }

                source = TranslateFilter(source, filterDescriptor, filterTranslatorProvider);
            }

            return source;
        }

        private static IQueryable<TEntity> TranslateFilter<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor, IFilterTranslatorProvider filterTranslatorProvider)
        {
            IFilterTranslator converter = filterTranslatorProvider.GetFilterTranslator(filterDescriptor.FilterType);

            // If we don't have this exact property, it's most likely we are either being attacked or 
            // or the FilterDescriptor has an error. Whatever's the case, it's wrong to just silently 
            // return the original Queryable.
            //
            // Make it visible and stop filtering immediately.
            if(!TypeExtensions.HasProperty(typeof(TEntity), filterDescriptor.PropertyName))
            {
                throw new InvalidOperationException($"The Property '{filterDescriptor.PropertyName}' does not exist on Type '{typeof(TEntity)}'");
            }

            // We can now safely convert the Filter without injection attacks
            return converter.Convert(source, filterDescriptor);
        }
    }
}