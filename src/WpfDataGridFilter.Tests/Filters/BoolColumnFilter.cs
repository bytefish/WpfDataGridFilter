using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;
using WpfDataGridFilter.Filters.Models;
using WpfDataGridFilter.Tests.Filters;

namespace WpfDataGridFilter.Filters
{
    /// <summary>
    /// A Filter for a Date Time Column.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BoolColumnFilter<TEntity>
    {
        /// <summary>
        /// Gets or sets the Column Name.
        /// </summary>
        public required string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the Filter Operator.
        /// </summary>
        public FilterOperatorEnum FilterOperator { get; set; }

        /// <summary>
        /// The Property of the TEntity, that we are going to filter on.
        /// </summary>
        public required Expression<Func<TEntity, bool?>> PropertyGetter { get; set; }

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
            var a = PropertyGetter.Body;

            // Create a Parameter for the Lambda Function, that takes a TEntity type and 
            // names it "entity" for sake of simplicity.
            ParameterExpression parameterExpressionEntity = PropertyGetter.Parameters[0];

            // Build the BinaryExpression for the given Filter Operator
            Expression? expression = null;

            switch (FilterOperator)
            {
                case FilterOperatorEnum.Yes:
                    expression = Expression.AndAlso(
                        Expression.NotEqual(PropertyGetter.Body, Expression.Constant(null)),
                            Expression.IsTrue(Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType())));
                    break;
                case FilterOperatorEnum.No:
                    expression = Expression.AndAlso(
                         Expression.NotEqual(PropertyGetter.Body, Expression.Constant(null)),
                         Expression.IsFalse(Expression.Convert(PropertyGetter.Body, PropertyGetter.Body.Type.GetNonNullableType())));
                    break;
                case FilterOperatorEnum.All:
                    expression = Expression.NotEqual(PropertyGetter.Body, Expression.Constant(null));
                    break;
                case FilterOperatorEnum.IsNull:
                    expression = Expression.Equal(PropertyGetter.Body, Expression.Constant(null));
                    break;
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{FilterOperator}'");
            }

            // Build the Predicate, that takes the BinaryExpression
            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(expression!, parameterExpressionEntity);

            // Add the Predicate to the IQueryable<TEntity> 
            return predicate;
        }
    }
}
