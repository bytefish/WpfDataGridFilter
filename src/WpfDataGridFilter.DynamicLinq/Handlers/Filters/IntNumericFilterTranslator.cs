// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class IntNumericFilterTranslator : IFilterTranslator
    {
        public string FilterType => "IntNumericFilter";

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not IntNumericFilterDescriptor f)
            {
                return source;
            }

            switch (f.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{f.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{f.PropertyName} ne null");
                case FilterOperatorEnum.IsEqualTo:
                    return source.Where($"{f.PropertyName} eq @0", f.LowerValue);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{f.PropertyName} ne @0", f.LowerValue);
                case FilterOperatorEnum.IsGreaterThan:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} gt @0)", f.LowerValue);
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} ge @0)", f.LowerValue);
                case FilterOperatorEnum.IsLessThan:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} lt @0)", f.LowerValue);
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} le @0)", f.LowerValue);
                case FilterOperatorEnum.BetweenExclusive:
                    return source.Where($"(({f.PropertyName} ne null) and ({f.PropertyName} gt @0)) and (({f.PropertyName} ne null) and ({f.PropertyName} lt @1))", f.LowerValue, f.UpperValue);
                case FilterOperatorEnum.BetweenInclusive:
                    return source.Where($"(({f.PropertyName} ne null) and ({f.PropertyName} ge @0)) and (({f.PropertyName} ne null) and ({f.PropertyName} le @1))", f.LowerValue, f.UpperValue);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
