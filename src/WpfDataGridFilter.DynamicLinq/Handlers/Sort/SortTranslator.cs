// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Dynamic.Core;
using WpfDataGridFilter.DynamicLinq.Infrastructure;
using WpfDataGridFilter.Models;

namespace WpfDataGridFilter.DynamicLinq.Converters.Sorts
{
    public class SortTranslator : ISortTranslator
    {
        public IQueryable<TEntity> Sort<TEntity>(IQueryable<TEntity> source, SortColumn sortColumn)
        {
            switch (sortColumn.SortDirection)
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
    }
}
