// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class StringFilterTranslator : IFilterTranslator
    {
        public FilterType FilterType => FilterType.StringFilter;

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not StringFilterDescriptor f)
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
                    return source.Where($"{f.PropertyName} eq @0", f.Value);
                case var _ when f.FilterOperator == FilterOperator.IsNotEqualTo:
                    return source.Where($"{f.PropertyName} neq @0", f.Value);
                case var _ when f.FilterOperator == FilterOperator.IsEmpty:
                    return source.Where($"({f.PropertyName} eq null) or ({f.PropertyName} eq \"\")");
                case var _ when f.FilterOperator == FilterOperator.IsNullOrWhitespace:
                    return source.Where($"({f.PropertyName} eq null) or ({f.PropertyName}.Trim() eq \"\")");
                case var _ when f.FilterOperator == FilterOperator.IsNotEmpty:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName} ne \"\")");
                case var _ when f.FilterOperator == FilterOperator.IsNotNullOrWhitespace:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.Trim() ne \"\")");
                case var _ when f.FilterOperator == FilterOperator.Contains:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.Contains(@0))", f.Value);
                case var _ when f.FilterOperator == FilterOperator.NotContains:
                    return source.Where($"({f.PropertyName} ne null) and (not {f.PropertyName}.Contains(@0))", f.Value);
                case var _ when f.FilterOperator == FilterOperator.StartsWith:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.StartsWith(@0))", f.Value);
                case var _ when f.FilterOperator == FilterOperator.EndsWith:
                    return source.Where($"({f.PropertyName} ne null) and ({f.PropertyName}.EndsWith(@0))", f.Value);
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
