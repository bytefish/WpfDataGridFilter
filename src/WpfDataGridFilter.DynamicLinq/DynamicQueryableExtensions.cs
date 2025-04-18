using System.Linq.Dynamic.Core;
using WpfDataGridFilter.Filters;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.DynamicLinq
{
    public static class DynamicQueryableExtensions
    {
        public static int GetTotalCount<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState)
        {
            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            return source
                .ApplyFilters(filters)
                .Count();
        }

        public static IQueryable<TEntity> ApplyDataGridState<TEntity>(this IQueryable<TEntity> source, DataGridState dataGridState, int top, int skip)
        {
            List<FilterDescriptor> filters = dataGridState.Filters.Values.ToList();

            return source
                .ApplyFilters(filters)
                .ApplySort(dataGridState.SortColumn)
                .Skip(skip)
                .Take(top);            
        }

        private static IQueryable<TEntity> ApplySort<TEntity>(this IQueryable<TEntity> source, SortColumn? sortColumn)
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

        private static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> source, ICollection<FilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Count == 0)
            {
                return source;
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
            switch (filterDescriptor.FilterType)
            {
                case FilterTypeEnum.BooleanFilter:
                    return TranslateBooleanFilter(source, (BooleanFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DateTimeFilter:
                    return TranslateDateTimeFilter(source, (DateTimeFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DateTimeOffsetFilter:
                    return TranslateDateTimeOffsetFilter(source, (DateTimeOffsetFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.StringFilter:
                    return TranslateStringFilter(source, (StringFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.IntNumericFilter:
                    return TranslateIntNumericFilter(source, (IntNumericFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DoubleNumericFilter:
                    return TranslateDoubleNumericFilter(source, (DoubleNumericFilterDescriptor)filterDescriptor);
                default:
                    throw new ArgumentException($"Could not translate Filter Type '{filterDescriptor.FilterType}'");
            }
        }

        private static IQueryable<TEntity> TranslateBooleanFilter<TEntity>(IQueryable<TEntity> source, BooleanFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} neq null");
                case FilterOperatorEnum.All:
                    return source.Where($"{filterDescriptor.PropertyName} neq null");
                case FilterOperatorEnum.Yes:
                    return source.Where($"{filterDescriptor.PropertyName} eq true");
                case FilterOperatorEnum.No:
                    return source.Where($"{filterDescriptor.PropertyName} eq false");
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static IQueryable<TEntity> TranslateDateTimeFilter<TEntity>(IQueryable<TEntity> source, DateTimeFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} neq null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} eq @0", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} neq  @0", filterDescriptor.StartDate);
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName}  neq null) and ({filterDescriptor.PropertyName} le  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt @0)) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt @1))", filterDescriptor.StartDate, filterDescriptor.EndDate);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge @0)) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} le @1))", filterDescriptor.StartDate, filterDescriptor.EndDate);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static IQueryable<TEntity> TranslateDateTimeOffsetFilter<TEntity>(IQueryable<TEntity> source, DateTimeOffsetFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} neq null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} eq @0", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} neq  @0", filterDescriptor.StartDate);
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName}  neq null) and ({filterDescriptor.PropertyName} le  @0)", filterDescriptor.StartDate);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt @0)) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt @1))", filterDescriptor.StartDate, filterDescriptor.EndDate);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge @0)) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} le @1))", filterDescriptor.StartDate, filterDescriptor.EndDate);

                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static IQueryable<TEntity> TranslateStringFilter<TEntity>(IQueryable<TEntity> source, StringFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} ne null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} eq @0", filterDescriptor.Value);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} neq @0", filterDescriptor.Value);
                case FilterOperatorEnum.IsEmpty:
                    return source.Where($"({filterDescriptor.PropertyName} eq null) or ({filterDescriptor.PropertyName} eq \"\")");
                case FilterOperatorEnum.IsNullOrWhitespace:
                    return source.Where($"({filterDescriptor.PropertyName} eq null) or ({filterDescriptor.PropertyName}.Trim() eq \"\")");
                case FilterOperatorEnum.IsNotEmpty:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ne \"\")");
                case FilterOperatorEnum.IsNotNullOrWhitespace:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.Trim() ne \"\")");
                case FilterOperatorEnum.Contains:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.Contains(@0))", filterDescriptor.Value);
                case FilterOperatorEnum.NotContains:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and (not {filterDescriptor.PropertyName}.Contains(@0))", filterDescriptor.Value);
                case FilterOperatorEnum.StartsWith:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.StartsWith(@0))", filterDescriptor.Value);
                case FilterOperatorEnum.EndsWith:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.EndsWith(@0))", filterDescriptor.Value);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static IQueryable<TEntity> TranslateIntNumericFilter<TEntity>(IQueryable<TEntity> source, IntNumericFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} ne null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} eq @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} ne @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt @0)", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge @0)", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt @0)", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le @0)", filterDescriptor.LowerValue);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt @0)) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt @1))", filterDescriptor.LowerValue, filterDescriptor.UpperValue);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge @0)) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le @1))", filterDescriptor.LowerValue, filterDescriptor.UpperValue);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static IQueryable<TEntity> TranslateDoubleNumericFilter<TEntity>(IQueryable<TEntity> source, DoubleNumericFilterDescriptor filterDescriptor)
        {

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{filterDescriptor.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{filterDescriptor.PropertyName} ne null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} eq @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{filterDescriptor.PropertyName} ne @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le @0", filterDescriptor.LowerValue);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt @0)) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt @1", filterDescriptor.LowerValue, filterDescriptor.UpperValue);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge @0)) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le @1))", filterDescriptor.LowerValue, filterDescriptor.UpperValue);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }
    }
}

