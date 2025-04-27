// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Converters.Sorts;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
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
        private static IFilterTranslatorProvider DefaultFilterTranslatorProvider = FilterTranslatorProvider.GetDefault();
        
        /// <summary>
        /// The default Sort Translator as a Static Field, so we don't create a new one for each call.
        /// </summary>
        private static ISortTranslator DefaultSortTranslator = new SortTranslator();

        public static int GetTotalItemCount<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterTranslatorProvider? FilterTranslatorProvider = null)
        {
            if (FilterTranslatorProvider == null)
            {
                FilterTranslatorProvider = DefaultFilterTranslatorProvider;
            }

            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            return source
                .ApplyFilters(filters, FilterTranslatorProvider)
                .Count();
        }

        public static IQueryable<TEntity> ApplyDataGridState<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterTranslatorProvider? FilterTranslatorProvider = null)
        {
            if (FilterTranslatorProvider == null)
            {
                FilterTranslatorProvider = DefaultFilterTranslatorProvider;
            }

            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            IQueryable<TEntity> query = source
                // First Apply the Filters:
                .ApplyFilters(filters)
                // Then Sort them by the current Sort Column
                .ApplySort(dataGridState.SortColumn);

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

        public static IQueryable<TEntity> ApplySort<TEntity>(this IQueryable<TEntity> source, SortColumn? sortColumn, ISortTranslator? sortTranslator = null)
        {
            if(sortColumn == null)
            {
                return source;
            }

            if(sortTranslator == null)
            {
                sortTranslator = new SortTranslator();
            }

            return sortTranslator.Sort(source, sortColumn);

        }

        public static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> source, ICollection<FilterDescriptor> filterDescriptors, IFilterTranslatorProvider? FilterTranslatorProvider = null)
        {
            if (filterDescriptors.Count == 0)
            {
                return source;
            }

            if (FilterTranslatorProvider == null)
            {
                FilterTranslatorProvider = DefaultFilterTranslatorProvider;
            }


            // Translate all Filters
            List<string> filters = new();

            foreach (FilterDescriptor filterDescriptor in filterDescriptors)
            {
                if (filterDescriptor.FilterOperator == FilterOperatorEnum.None)
                {
                    continue;
                }

                source = TranslateFilter(source, filterDescriptor);
            }

            return source;
        }

        private static IQueryable<TEntity> TranslateFilter<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            // Gets the FilterTranslator
            IFilterTranslator converter = DefaultFilterTranslatorProvider.GetFilterTranslatorByName(filterDescriptor.FilterType);

            return converter.Convert(source, filterDescriptor);
        }
    }
}