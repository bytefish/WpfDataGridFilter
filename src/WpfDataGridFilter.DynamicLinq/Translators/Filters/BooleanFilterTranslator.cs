// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class BooleanFilterTranslator : IFilterTranslator
    {
        public FilterType FilterType => FilterType.BooleanFilter;

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not BooleanFilterDescriptor f)
            {
                return source;
            }

            switch (f.FilterOperator)
            {
                case var _ when f.FilterOperator == FilterOperator.IsNull:
                    return source.Where($"{f.PropertyName} eq null");
                case var _ when f.FilterOperator == FilterOperator.IsNotNull:
                    return source.Where($"{f.PropertyName} neq null");
                case var _ when f.FilterOperator == FilterOperator.All:
                    return source.Where($"{f.PropertyName} neq null");
                case var _ when f.FilterOperator == FilterOperator.Yes:
                    return source.Where($"{f.PropertyName} eq true");
                case var _ when f.FilterOperator == FilterOperator.No:
                    return source.Where($"{f.PropertyName} eq false");
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
