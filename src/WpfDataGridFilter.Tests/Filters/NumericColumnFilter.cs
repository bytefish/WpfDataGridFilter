using System.Linq.Expressions;
using System.Reflection;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Tests.Filters
{
    /// <summary>
    /// A Filter for a Numeric Column.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class NumericColumnFilter<TEntity, TNumeric>
    {
        /// <summary>
        /// Gets or sets the Column Name.
        /// </summary>
        public required string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the Filter Value.
        /// </summary>
        public TNumeric? LowerValue { get; set; }
        
        /// <summary>
        /// Gets or sets the Filter Value.
        /// </summary>
        public TNumeric? UpperValue { get; set; }

        /// <summary>
        /// Gets or sets the Filter Operator.
        /// </summary>
        public FilterOperatorEnum FilterOperator { get; set; }

        /// <summary>
        /// The Property of the TEntity, that we are going to filter on.
        /// </summary>
        public required Expression<Func<TEntity, TNumeric?>> PropertyGetter { get; set; }

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

            // Build the two Constant Expressions for the given Filter Values
            ConstantExpression constantExpressionLowerValue = LowerValue == null ?
                Expression.Constant(null, typeof(TNumeric?)) :
                Expression.Constant(LowerValue, typeof(TNumeric));
            
            ConstantExpression constantExpressionUpperValue = UpperValue == null ?
                Expression.Constant(null, typeof(TNumeric?)) :
                Expression.Constant(UpperValue, typeof(TNumeric));

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
                    expression = Expression.Equal(PropertyGetter.Body, constantExpressionLowerValue);
                    break;
                case FilterOperatorEnum.IsNotEqualTo:
                    expression = Expression.NotEqual(PropertyGetter.Body, constantExpressionLowerValue);
                    break;
                case FilterOperatorEnum.IsLessThan:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.LessThan(PropertyGetter.Body, constantExpressionLowerValue));
                    break;
                case FilterOperatorEnum.IsLessThanOrEqualTo:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.LessThanOrEqual(PropertyGetter.Body, constantExpressionLowerValue));
                    break;
                case FilterOperatorEnum.IsGreaterThan:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.GreaterThan(PropertyGetter.Body, constantExpressionLowerValue));
                    break;
                case FilterOperatorEnum.IsGreaterThanOrEqualTo:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.GreaterThanOrEqual(PropertyGetter.Body, constantExpressionLowerValue));
                    break;
                case FilterOperatorEnum.BetweenExclusive:
                    Expression leftBetweenExclusive = LowerValue == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: Expression.Convert(constantExpressionLowerValue, LowerValue.GetType().GetNonNullableType())));

                    Expression rightBetweenExclusive = UpperValue == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThan(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: Expression.Convert(constantExpressionUpperValue, UpperValue.GetType().GetNonNullableType())));

                    expression = Expression.AndAlso(leftBetweenExclusive, rightBetweenExclusive);
                    break;
                case FilterOperatorEnum.BetweenInclusive:
                    Expression leftBetweenInclusive = LowerValue == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.GreaterThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: Expression.Convert(constantExpressionLowerValue, LowerValue.GetType().GetNonNullableType())));

                    Expression rightBetweenInclusive = UpperValue == null ? Expression.Constant(true) :
                        Expression.AndAlso(
                             left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                             right: Expression.LessThanOrEqual(
                                 left: Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType()),
                                 right: Expression.Convert(constantExpressionUpperValue, UpperValue.GetType().GetNonNullableType())));

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
