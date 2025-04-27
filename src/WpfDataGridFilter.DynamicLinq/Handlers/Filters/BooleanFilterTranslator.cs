// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Translators
{
    public class BooleanFilterTranslator : IFilterTranslator
    {
        public string FilterType => "BooleanFilter";

        public IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
        {
            if (filterDescriptor is not BooleanFilterDescriptor f)
            {
                return source;
            }

            switch (f.FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    return source.Where($"{f.PropertyName} eq null");
                case FilterOperatorEnum.IsNotNull:
                    return source.Where($"{f.PropertyName} neq null");
                case FilterOperatorEnum.All:
                    return source.Where($"{f.PropertyName} neq null");
                case FilterOperatorEnum.Yes:
                    return source.Where($"{f.PropertyName} eq true");
                case FilterOperatorEnum.No:
                    return source.Where($"{f.PropertyName} eq false");
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{f.FilterOperator}'");
            }
        }
    }
}
