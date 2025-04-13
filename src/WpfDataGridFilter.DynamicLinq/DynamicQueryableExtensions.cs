using System;
using System.Globalization;
using System.Linq.Dynamic.Core;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.DynamicLinq
{
    public static class DynamicQueryableExtensions
    {
        private static string NullStr = "null";

        public static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> source, ICollection<FilterDescriptor> filterDescriptors)
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

                string? filter = TranslateFilter(filterDescriptor);

                filters.Add(filter);
            }

            // Concatenate all Filters
            string dynamicLinqFilter = string.Join(" and ", filters.Select(filter => $"({filter})")); ;

            return source.Where(dynamicLinqFilter);
        }

        private static string TranslateFilter(FilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterType)
            {
                case FilterTypeEnum.BooleanFilter:
                    return TranslateBooleanFilter((BooleanFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DateTimeFilter:
                    return TranslateDateTimeFilter((DateTimeFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DateTimeOffsetFilter:
                    return TranslateDateTimeOffsetFilter((DateTimeOffsetFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.StringFilter:
                    return TranslateStringFilter((StringFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.IntNumericFilter:
                    return TranslateIntNumericFilter((IntNumericFilterDescriptor)filterDescriptor);
                case FilterTypeEnum.DoubleNumericFilter:
                    return TranslateDoubleNumericFilter((DoubleNumericFilterDescriptor)filterDescriptor);
                default:
                    throw new ArgumentException($"Could not translate Filter Type '{filterDescriptor.FilterType}'");
            }
        }

        private static string TranslateBooleanFilter(BooleanFilterDescriptor filterDescriptor)
        {
            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} neq null";
                case FilterOperatorEnum.All:
                    return $"{filterDescriptor.PropertyName} neq null";
                case FilterOperatorEnum.Yes:
                    return $"{filterDescriptor.PropertyName} eq true";
                case FilterOperatorEnum.No:
                    return $"{filterDescriptor.PropertyName} eq false";
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static string TranslateDateTimeFilter(DateTimeFilterDescriptor filterDescriptor)
        {
            string? startDate = ToDynamicLinqDateTime(filterDescriptor.StartDate);
            string? endDate = ToDynamicLinqDateTime(filterDescriptor.EndDate);

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} neq null";
                case FilterOperatorEnum.IsEqualTo:
                    return $"{filterDescriptor.PropertyName} eq {startDate}";
                case FilterOperatorEnum.IsNotEqualTo:
                    return $"{filterDescriptor.PropertyName} neq {startDate}";
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt {startDate})";
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge {startDate})";
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt {startDate})";
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName}  neq null) and ({filterDescriptor.PropertyName} le {startDate})";
                case FilterOperatorEnum.BetweenExclusive:
                    return $"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt {startDate})) and (({filterDescriptor.PropertyName} neq null) and (({filterDescriptor.PropertyName}) lt {endDate}))";
                case FilterOperatorEnum.BetweenInclusive:
                    return $"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge {startDate})) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} le {endDate}))";

                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static string TranslateDateTimeOffsetFilter(DateTimeOffsetFilterDescriptor filterDescriptor)
        {
            string? startDate = ToDynamicLinqDateTimeOffset(filterDescriptor.StartDate);
            string? endDate = ToDynamicLinqDateTimeOffset(filterDescriptor.EndDate);

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} neq null";
                case FilterOperatorEnum.IsEqualTo:
                    return $"{filterDescriptor.PropertyName} eq {startDate}";
                case FilterOperatorEnum.IsNotEqualTo:
                    return $"{filterDescriptor.PropertyName} neq {startDate}";
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt {startDate})";
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge {startDate})";
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    return $"({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} lt {startDate})";
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName}  neq null) and ({filterDescriptor.PropertyName} le {startDate})";
                case FilterOperatorEnum.BetweenExclusive:
                    return $"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} gt {startDate})) and (({filterDescriptor.PropertyName} neq null) and (({filterDescriptor.PropertyName}) lt {endDate}))";
                case FilterOperatorEnum.BetweenInclusive:
                    return $"(({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} ge {startDate})) and (({filterDescriptor.PropertyName} neq null) and ({filterDescriptor.PropertyName} le {endDate}))";

                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static string TranslateStringFilter(StringFilterDescriptor filterDescriptor)
        {
            string? value = ToDynamicLinqString(filterDescriptor.Value);

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} ne null";
                case FilterOperatorEnum.IsEqualTo:
                    return $"{filterDescriptor.PropertyName} eq {value}";
                case FilterOperatorEnum.IsNotEqualTo:
                    return $"{filterDescriptor.PropertyName} neq {value}";
                case FilterOperatorEnum.IsEmpty:
                    return $"({filterDescriptor.PropertyName} eq null) or ({filterDescriptor.PropertyName} eq \"\")";
                case FilterOperatorEnum.IsNullOrWhitespace:
                    return $"({filterDescriptor.PropertyName} eq null) or ({filterDescriptor.PropertyName}.Trim() eq \"\")";
                case FilterOperatorEnum.IsNotEmpty:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ne \"\")";
                case FilterOperatorEnum.IsNotNullOrWhitespace:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.Trim() ne \"\")";
                case FilterOperatorEnum.Contains:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.Contains({value}))";
                case FilterOperatorEnum.NotContains:
                    return $"({filterDescriptor.PropertyName} ne null) and (not {filterDescriptor.PropertyName}.Contains(\"{filterDescriptor.Value}\"))";
                case FilterOperatorEnum.StartsWith:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.StartsWith(\"{filterDescriptor.Value}\"))";
                case FilterOperatorEnum.EndsWith:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName}.EndsWith(\"{filterDescriptor.Value}\"))";
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }


        private static string TranslateIntNumericFilter(IntNumericFilterDescriptor filterDescriptor)
        {
            string? low = ToDynamicLinqInt32(filterDescriptor.LowerValue);
            string? high = ToDynamicLinqInt32(filterDescriptor.UpperValue);

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} ne null";
                case FilterOperatorEnum.IsEqualTo:
                    return $"{filterDescriptor.PropertyName} eq {low}";
                case FilterOperatorEnum.IsNotEqualTo:
                    return $"{filterDescriptor.PropertyName} ne {low}";
                case FilterOperatorEnum.IsGreaterThan:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt {low})";
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge {low})";
                case FilterOperatorEnum.IsLessThan:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt {low})";
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return $"({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le {low})";
                case FilterOperatorEnum.BetweenExclusive:
                    return $"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} gt {low})) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} lt {high}))";
                case FilterOperatorEnum.BetweenInclusive:
                    return $"(({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} ge {low})) and (({filterDescriptor.PropertyName} ne null) and ({filterDescriptor.PropertyName} le {high}))";
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static string TranslateDoubleNumericFilter(DoubleNumericFilterDescriptor filterDescriptor)
        {
            var low = ToDynamicLinqDouble(filterDescriptor.LowerValue);
            var high = ToDynamicLinqDouble(filterDescriptor.UpperValue);

            switch (filterDescriptor.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return $"{filterDescriptor.PropertyName} eq null";
                case FilterOperatorEnum.IsNotNull:
                    return $"{filterDescriptor.PropertyName} ne null";
                case FilterOperatorEnum.IsEqualTo:
                    return $"{filterDescriptor.PropertyName} eq {low}";
                case FilterOperatorEnum.IsNotEqualTo:
                    return $"{filterDescriptor.PropertyName} ne {low}";
                case FilterOperatorEnum.IsGreaterThan:
                    return $"{filterDescriptor.PropertyName} gt {low}";
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return $"{filterDescriptor.PropertyName} ge {low}";
                case FilterOperatorEnum.IsLessThan:
                    return $"{filterDescriptor.PropertyName} lt {low}";
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return $"{filterDescriptor.PropertyName} le {low}";
                case FilterOperatorEnum.BetweenExclusive:
                    return $"({filterDescriptor.PropertyName} gt {low}) and({filterDescriptor.PropertyName} lt {high})";
                case FilterOperatorEnum.BetweenInclusive:
                    return $"({filterDescriptor.PropertyName} ge {low}) and({filterDescriptor.PropertyName} le {high})";
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{filterDescriptor.FilterOperator}'");
            }
        }

        private static string? ToDynamicLinqDateTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return "null";
            }

            return $"DateTime({dateTime.Value.Ticks})";
        }

        private static string? ToDynamicLinqDateTimeOffset(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset == null) 
            {
                return "null";
            }

            return $"DateTimeOffset({dateTimeOffset!.Value.Ticks}, TimeSpan.Parse(\"{dateTimeOffset.Value.Offset.ToString()}\"))";
        }

        private static string? ToDynamicLinqString(string? value)
        {
            if (value == null)
            {
                return "null";
            }

            return $"String(\"{value}\")";
        }

        private static string? ToDynamicLinqInt32(int? value)
        {
            if (value == null) 
            {
                return "null";
            }

            return $"Int32({value.Value.ToString(CultureInfo.InvariantCulture)})";
        }

        private static string? ToDynamicLinqDouble(double? value)
        {
            if (value == null) 
            {
                return "null";
            }

            return $"Double({value.Value.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}

