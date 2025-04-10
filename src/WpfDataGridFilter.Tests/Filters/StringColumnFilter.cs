﻿using System.Linq.Expressions;
using System.Reflection;
using WpfDataGridFilter.Filters.Models;

namespace WpfDataGridFilter.Tests.Filters
{
    /// <summary>
    /// A Filter for a Date Time Column.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class StringColumnFilter<TEntity>
    {
        #region Method Infos

        private static readonly MethodInfo MethodInfoContains = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
        
        private static readonly MethodInfo MethodInfoStartsWith = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
        
        private static readonly MethodInfo MethodInfoEndsWith = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;
        
        private static readonly MethodInfo MethodInfoIsNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty), [typeof(string)])!;
        
        private static readonly MethodInfo MethodInfoIsNullOrWhitespace = typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace), [typeof(string)])!;

        #endregion

        /// <summary>
        /// Gets or sets the Column Name.
        /// </summary>
        public required string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the Filter Value.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the Filter Operator.
        /// </summary>
        public FilterOperatorEnum FilterOperator { get; set; }

        /// <summary>
        /// The Property of the TEntity, that we are going to filter on.
        /// </summary>
        public required Expression<Func<TEntity, string?>> PropertyGetter { get; set; }


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
            ConstantExpression constantExpressionValue = Value == null ?
                Expression.Constant(null, typeof(string)) :
                Expression.Constant(Value, typeof(string));

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
                    expression = Expression.Equal(PropertyGetter.Body, constantExpressionValue);
                    break;
                case FilterOperatorEnum.IsNotEqualTo:
                    expression = Expression.NotEqual(PropertyGetter.Body, constantExpressionValue);
                    break;

                case FilterOperatorEnum.Contains:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.Call(PropertyGetter.Body, MethodInfoContains, constantExpressionValue));
                    break;
                case FilterOperatorEnum.NotContains:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.Not(Expression.Call(PropertyGetter.Body, MethodInfoContains, constantExpressionValue)));
                    break;
                case FilterOperatorEnum.IsEmpty:
                    expression = Expression.Call(MethodInfoIsNullOrEmpty, PropertyGetter.Body);
                    break;
                case FilterOperatorEnum.IsNotEmpty:
                    expression = Expression.Not(Expression.Call(MethodInfoIsNullOrEmpty, PropertyGetter.Body));
                    break;
                case FilterOperatorEnum.IsNullOrWhitespace:
                    expression = Expression.Call(MethodInfoIsNullOrWhitespace, PropertyGetter.Body);
                    break;
                case FilterOperatorEnum.IsNotNullOrWhitespace:
                    expression = Expression.Not(Expression.Call(MethodInfoIsNullOrWhitespace, PropertyGetter.Body));
                    break;
                case FilterOperatorEnum.StartsWith:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.Call(PropertyGetter.Body, MethodInfoStartsWith, constantExpressionValue));
                    break;
                case FilterOperatorEnum.EndsWith:
                    expression =
                        Expression.AndAlso(
                            left: Expression.NotEqual(left: PropertyGetter.Body, right: Expression.Constant(null)),
                            right: Expression.Call(PropertyGetter.Body, MethodInfoEndsWith, constantExpressionValue));
                    break;
                default:
                    throw new ArgumentException($"Could not translate Filter Operator '{FilterOperator}'");
            }

            // Build the Predicate, that takes the BinaryExpression
            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameterExpressionEntity);

            // Add the Predicate to the IQueryable<TEntity> 
            return predicate;
        }
    }
}
