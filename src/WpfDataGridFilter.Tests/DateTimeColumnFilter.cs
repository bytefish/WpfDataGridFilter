using System.Linq.Expressions;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Filters
{
    /// <summary>
    /// A Filter for a Date Time Column.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DateTimeColumnFilter<TEntity>
    {
        /// <summary>
        /// Gets or sets the Column Name.
        /// </summary>
        public required string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the Start Date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the End Date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Filter Operator.
        /// </summary>
        public FilterOperatorEnum FilterOperator { get; set; }

        /// <summary>
        /// The Property of the TEntity, that we are going to filter on.
        /// </summary>
        public required Expression<Func<TEntity, DateTime>> PropertyGetter { get; set; }

        /// <summary>
        /// Applies the Column Filter on a given <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="source">The Queryable to load the Data with</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Expression<Func<TEntity, bool>> GetFilterPredicate()
        {
            // Now get the MemberExpression from the Property Getter, which is used to get 
            // the Value from the actual TEntity, which is passed to the Predicate.
            MemberExpression memberExpression = (MemberExpression)PropertyGetter.Body;

            // Create a Parameter for the Lambda Function, that takes a TEntity type and 
            // names it "entity" for sake of simplicity.
            ParameterExpression parameterExpressionEntity = PropertyGetter.Parameters[0];

            // Build the two Constant Expressions for the given Start and End Date, which, depending
            // on the Operator are needed to filter the data against.
            ConstantExpression constantExpressionStartDate = StartDate == null ?
                Expression.Constant(null, typeof(DateTime?)) :
                Expression.Constant(Convert.ToDateTime(StartDate), typeof(DateTime));

            ConstantExpression constantExpressionEndDate = EndDate == null ?
                Expression.Constant(null, typeof(DateTime?)) :
                Expression.Constant(Convert.ToDateTime(EndDate), typeof(DateTime));

            Expression.Constant(EndDate, typeof(DateTime?));

            // Build the BinaryExpression for the given Filter Operator
            BinaryExpression? binaryExpression = null;

            switch (FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    binaryExpression = Expression.Equal(memberExpression, Expression.Constant(null, typeof(object)));
                    break;
                case FilterOperatorEnum.IsNotNull:
                    binaryExpression = Expression.NotEqual(memberExpression, Expression.Constant(null, typeof(object)));
                    break;
                case FilterOperatorEnum.IsEqualTo:
                    binaryExpression = Expression.Equal(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.IsNotEqualTo:
                    binaryExpression = Expression.NotEqual(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    binaryExpression = Expression.GreaterThan(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    binaryExpression = Expression.GreaterThanOrEqual(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    binaryExpression = Expression.LessThan(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    binaryExpression = Expression.LessThanOrEqual(memberExpression, constantExpressionStartDate);
                    break;
                case FilterOperatorEnum.BetweenExclusive:
                    binaryExpression = Expression.AndAlso(
                        Expression.GreaterThan(memberExpression, constantExpressionStartDate),
                        Expression.LessThan(memberExpression, constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.BetweenInclusive:
                    binaryExpression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(memberExpression, constantExpressionStartDate),
                        Expression.LessThanOrEqual(memberExpression, constantExpressionStartDate));
                    break;
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{FilterOperator}'");
            }

            // Build the Predicate, that takes the BinaryExpression
            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(binaryExpression, parameterExpressionEntity);

            // Add the Predicate to the IQueryable<TEntity> 
            return predicate;
        }
    }
}
