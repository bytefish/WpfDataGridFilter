// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class DateTimeFilterTranslator : IFilterTranslator
    {
        public string FilterType => "DateTimeFilter";

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not DateTimeFilterDescriptor f)
            {
                return source;
            }

            switch (f.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{f.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{f.PropertyName} neq null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{f.PropertyName} eq @0", f.StartDate);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{f.PropertyName} neq  @0", f.StartDate);
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({f.PropertyName} neq null) and ({f.PropertyName} gt  @0)", f.StartDate);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({f.PropertyName} neq null) and ({f.PropertyName} ge  @0)", f.StartDate);
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({f.PropertyName} neq null) and ({f.PropertyName} lt  @0)", f.StartDate);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({f.PropertyName}  neq null) and ({f.PropertyName} le  @0)", f.StartDate);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({f.PropertyName} neq null) and ({f.PropertyName} gt @0)) and (({f.PropertyName} neq null) and ({f.PropertyName} lt @1))", f.StartDate, f.EndDate);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({f.PropertyName} neq null) and ({f.PropertyName} ge @0)) and (({f.PropertyName} neq null) and ({f.PropertyName} le @1))", f.StartDate, f.EndDate);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
