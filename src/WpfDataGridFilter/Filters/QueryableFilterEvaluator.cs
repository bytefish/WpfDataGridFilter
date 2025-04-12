//using System.Linq.Expressions;
//using WpfDataGridFilter.Filters.Models;
//using WpfDataGridFilter.Filters.Predicates;

//namespace WpfDataGridFilter.Filters
//{
//    public abstract class PropertyMapper<TEntity, TFilterDescriptor> 
//        where TFilterDescriptor : FilterDescriptor
//    {
//        /// <summary>
//        /// Property Name.
//        /// </summary>
//        public required string PropertyName { get; set; }

//        /// <summary>
//        /// Builds the Filter Predicate.
//        /// </summary>
//        public abstract Expression<Func<TEntity, bool>> GetPredicate(TFilterDescriptor filterDescriptor);
//    }

//    public class BooleanPropertyMapper<TEntity> 
//        : PropertyMapper<TEntity, BooleanFilterDescriptor>
//    {
//        /// <summary>
//        /// The Property Getter.
//        /// </summary>
//        public required Expression<Func<TEntity, bool?>> PropertyGetter { get; set; }

//        /// <summary>
//        /// The Predicate.
//        /// </summary>
//        /// <param name="filterDescriptor"></param>
//        /// <returns></returns>
//        public override Expression<Func<TEntity, bool>> GetPredicate(BooleanFilterDescriptor filterDescriptor)
//        {
//            return FilterPredicates.GetBoolFilterExpression(PropertyGetter, filterDescriptor.FilterOperator);
//        }
//    }

//    public class StringPropertyMapper<TEntity> 
//        : PropertyMapper<TEntity, BooleanFilterDescriptor>
//    {
//        /// <summary>
//        /// The Property Getter.
//        /// </summary>
//        public required Expression<Func<TEntity, bool?>> PropertyGetter { get; set; }

//        /// <summary>
//        /// The Predicate.
//        /// </summary>
//        /// <param name="filterDescriptor"></param>
//        /// <returns></returns>
//        public override Expression<Func<TEntity, bool>> GetPredicate(BooleanFilterDescriptor filterDescriptor)
//        {
//            return FilterPredicates.GetBoolFilterExpression(PropertyGetter, filterDescriptor.FilterOperator);
//        }
//    }

//    public class QueryableFilterEvaluator<TEntity>
//    {
//        public static IQueryable<TEntity> Apply(IQueryable<TEntity> source, List<FilterDescriptor> filterDescriptors)
//        {
//            if (filterDescriptors.Count == 0)
//            {
//                return source;
//            }

//            IQueryable<TEntity> result = source;

//            foreach (FilterDescriptor filterDescriptor in filterDescriptors)
//            {
//                if (filterDescriptor.FilterOperator == FilterOperatorEnum.None)
//                {
//                    continue;
//                }

//                Expression<Func<TEntity, bool>> predicate = TranslateFilter(filterDescriptor);

//                result = result.Where(predicate);
//            }

//            return result;
//        }

//    }
//}
