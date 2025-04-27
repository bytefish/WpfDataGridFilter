// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class StringFilterTranslator : IFilterTranslator
    {
        public string FilterType => "StringFilter";

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not StringFilterDescriptor f)
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
                    return source.Where($"{f.PropertyName} eq @0", f.Value);
                case FilterOperatorEnum.IsNotEqualTo:
                    return source.Where($"{f.PropertyName} neq @0", f.Value);
                case FilterOperatorEnum.IsEmpty:
                    return source.Where($"({f.PropertyName} eq null) or ({f.PropertyName} eq \"\")");
                case FilterOperatorEnum.IsNullOrWhitespace:
                    return source.Where($"({f.PropertyName} eq null) or ({f.PropertyName}.Trim() eq \"\")");
                case FilterOperatorEnum.IsNotEmpty:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} ne \"\")");
                case FilterOperatorEnum.IsNotNullOrWhitespace:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.Trim() ne \"\")");
                case FilterOperatorEnum.Contains:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.Contains(@0))", f.Value);
                case FilterOperatorEnum.NotContains:
                    return source.Where($"({f.PropertyName} ne null) and (not {f.PropertyName}.Contains(@0))", f.Value);
                case FilterOperatorEnum.StartsWith:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.StartsWith(@0))", f.Value);
                case FilterOperatorEnum.EndsWith:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.EndsWith(@0))", f.Value);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
