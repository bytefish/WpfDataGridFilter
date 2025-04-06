using System.Data.Common;
using System.Linq.Expressions;
using System.Windows.Controls;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Tests.Filters
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
        public required Expression<Func<TEntity, DateTime?>> PropertyGetter { get; set; }

        /// <summary>
        /// Applies the Column Filter on a given <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="source">The Queryable to load the Data with</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Expression<Func<TEntity, bool>> GetFilterPredicate()
        {

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
            Expression? expression = null;

            switch (FilterOperator)
            {
                case FilterOperatorEnum.IsNull:
                    expression = Expression.Equal(PropertyGetter.Body, Expression.Constant(null));
                    break;
                case FilterOperatorEnum.IsNotNull:
                    expression = Expression.NotEqual(PropertyGetter.Body, Expression.Constant(null));
                    break;
                case FilterOperatorEnum.IsEqualTo:
                    expression = StartDate == null ?
                         Expression.Equal(left: PropertyGetter.Body, right: Expression.Constant(null)) :
                         Expression.AndAlso(
                                    left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                                    right: Expression.Equal(
                                        left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                        right: constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.IsNotEqualTo:
                    expression = StartDate == null ?
                         Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)) :
                         Expression.Or(
                             Expression.Equal(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             Expression.AndAlso(
                                    left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                                    right: Expression.NotEqual(
                                        left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                        right: constantExpressionStartDate)));
                    break;
                case FilterOperatorEnum.After:
                case FilterOperatorEnum.IsGreaterThan:
                    expression = StartDate == null ? Expression.Constant(false) : 
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    expression = StartDate == null ? Expression.Constant(false) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.Before:
                case FilterOperatorEnum.IsLessThan:
                    expression = StartDate == null ? Expression.Constant(false) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    expression = StartDate == null ? Expression.Constant(false) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));
                    break;
                case FilterOperatorEnum.BetweenExclusive:
                    Expression leftBetweenExclusive = StartDate == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));

                    Expression rightBetweenExclusive = EndDate == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionEndDate));

                    expression = Expression.AndAlso(leftBetweenExclusive, rightBetweenExclusive);
                    break;
                case FilterOperatorEnum.BetweenInclusive:
                    Expression leftBetweenInclusive = StartDate == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionStartDate));
                    Expression rightBetweenInclusive = EndDate == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: constantExpressionEndDate));

                    expression = Expression.AndAlso(leftBetweenInclusive, rightBetweenInclusive);
                    break;
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{FilterOperator}'");
            }

            // Build the Predicate, that takes the BinaryExpression
            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpressionEntity);

            // Add the Predicate to the IQueryable<TEntity> 
            return predicate;
        }

        static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

    }
}
