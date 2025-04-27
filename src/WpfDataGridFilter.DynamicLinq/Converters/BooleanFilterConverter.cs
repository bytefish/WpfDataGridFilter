using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Converters
{
    public class BooleanFilterConverter : FilterConverter
    {
        public override string FilterType => "BooleanFilter";

        public override IQueryable<TEntity> Convert<TEntity>(IQueryable<TEntity> source, FilterDescriptor filterDescriptor)
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
