// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class IntNumericFilterTranslator : IFilterTranslator
    {
        public FilterType FilterType => FilterType.IntNumericFilter;

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not IntNumericFilterDescriptor f)
            {
                return source;
            }

            switch (f.FilterOperator)
            {
                case var _ when f.FilterOperator == FilterOperator.IsNull:
                    return source.Where($"{f.PropertyName} eq null");
                case var _ when f.FilterOperator == FilterOperator.IsNotNull:
                    return source.Where($"{f.PropertyName} ne null");
                case var _ when f.FilterOperator == FilterOperator.IsEqualTo:
                    return source.Where($"{f.PropertyName} eq @0", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.IsNotEqualTo:
                    return source.Where($"{f.PropertyName} ne @0", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.IsGreaterThan:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} gt @0)", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.IsGreaterThanOrEqualTo:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} ge @0)", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.IsLessThan:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} lt @0)", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.IsLessThanOrEqualTo:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} le @0)", f.LowerValue);
                case var _ when f.FilterOperator == FilterOperator.BetweenExclusive:
                    return source.Where($"(({f.PropertyName} ne null) and ({f.PropertyName} gt @0)) and (({f.PropertyName} ne null) and ({f.PropertyName} lt @1))", f.LowerValue, f.UpperValue);
                case var _ when f.FilterOperator == FilterOperator.BetweenInclusive:
                    return source.Where($"(({f.PropertyName} ne null) and ({f.PropertyName} ge @0)) and (({f.PropertyName} ne null) and ({f.PropertyName} le @1))", f.LowerValue, f.UpperValue);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
