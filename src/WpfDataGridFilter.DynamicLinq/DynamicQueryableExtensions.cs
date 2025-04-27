using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Converters;
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
        /// The Default Converters as a Static Field, so we don't create a new one for each call.
        /// </summary>
        private static IFilterConverterProvider DefaultFilterConverterProvider => FilterConverterProvider.GetDefault();

        public static int GetTotalItemCount<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterConverterProvider? filterConverterProvider = null)
        {
            if (filterConverterProvider == null)
            {
                filterConverterProvider = DefaultFilterConverterProvider;
            }

            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            return source
                .ApplyFilters(filters, filterConverterProvider)
                .Count();
        }

        public static IQueryable<TEntity> ApplyDataGridState<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, IFilterConverterProvider? filterConverterProvider = null)
        {
            if (filterConverterProvider == null)
            {
                filterConverterProvider = DefaultFilterConverterProvider;
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

        public static IQueryable<TEntity> ApplySort<TEntity>(this IQueryable<TEntity> source, SortColumn? sortColumn)
        {
            if(sortColumn == null)
            {
                return source;
            }

            switch(sortColumn.SortDirection)
            {
                case null:
                    return source;
                case SortDirectionEnum.Ascending:
                    return source.OrderBy($"{sortColumn.PropertyName} asc");
                case SortDirectionEnum.Descending:
                    return source.OrderBy($"{sortColumn.PropertyName} desc");
                default:
                    throw new InvalidOperationException($"Could not sort Column '{sortColumn.PropertyName}' by Direction '{sortColumn.SortDirection}'");
            }
        }

        public static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> source, ICollection<FilterDescriptor> filterDescriptors, IFilterConverterProvider? filterConverterProvider = null)
        {
            if (filterDescriptors.Count == 0)
            {
                return source;
            }

            if (filterConverterProvider == null)
            {
                filterConverterProvider = DefaultFilterConverterProvider;
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
            // Gets the FilterConverter
            FilterConverter converter = DefaultFilterConverterProvider.GetFilterConverterByName(filterDescriptor.FilterType);

            return converter.Convert(source, filterDescriptor);
        }
    }
}