using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicCollectionOperations.Services
{
    /*todo: extend this class when needed for the following scenarios:
    * 1. Filter by multiple values (FilterDescriptor.Value is of type list: eg. value.Contains(TClass.Property))
    */
    public class ExpressionBuilderService : IExpressionBuilderService
    {

        private const string ParameterName = "x";
        private const string CollectionParameterName = "list";
        private const string PropertySeparator = ".";
        private const string HasValueProperty = "HasValue";
        private const string ValueProperty = "Value";
        private const string AnyMethod = "Any";
        private const string Descending = "desc";
        private const string OrderByDescendingCommand = "OrderByDescending";
        private const string OrderByCommand = "OrderBy";

        private readonly Dictionary<Type, object> _defaultValues = new Dictionary<Type, object> {
            {typeof(int), int.MinValue},
            {typeof(double), double.MinValue},
            {typeof(float), float.MinValue},
            {typeof(long), long.MinValue},
            {typeof(short), short.MinValue},
            {typeof(decimal), decimal.MinValue}
        };

        public Expression<Func<TClass, bool>> GetFilterExpression<TClass>(FilterDescriptor filterDescriptor)
        {
            var parameterExpression = GetParameterExpression(typeof(TClass));
            var expression = IsList(typeof(TClass), filterDescriptor.PropertyName) ?
                BuildCollectionExpression(filterDescriptor, parameterExpression) :
                BuildPropertyExpression(filterDescriptor, parameterExpression);

            return Expression.Lambda<Func<TClass, bool>>(expression, parameterExpression);
        }


        public MethodCallExpression GetSortExpression<TClass>(Sort criteria, ICollection<TClass> source)
        {
            var type = typeof(TClass);
            var command = criteria.SortDirection.Equals(Descending, StringComparison.CurrentCultureIgnoreCase) ? OrderByDescendingCommand : OrderByCommand;
            var parameter = GetParameterExpression(typeof(TClass));
            var propertyAccess = GetPropertyExpression(parameter, criteria.SortColumn);
            propertyAccess = AddNullConditional(parameter, criteria.SortColumn, propertyAccess);

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var queryableSource = source.AsQueryable();
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, propertyAccess.Type },
                queryableSource.Expression, Expression.Quote(orderByExpression));

            return resultExpression;
        }

        private Expression BuildCollectionExpression(FilterDescriptor filterDescriptor, ParameterExpression parameterExpression)
        {
            var separatorIndex = filterDescriptor.PropertyName.IndexOf(PropertySeparator, StringComparison.Ordinal);
            var collectionProperty = filterDescriptor.PropertyName.Substring(0, separatorIndex);
            var propertyName = filterDescriptor.PropertyName.Substring(separatorIndex + 1);
            var propertyType = GetCollectionGenericType(parameterExpression, collectionProperty);
            var listParameter = Expression.Parameter(propertyType, CollectionParameterName);
            var lambda =
                Expression.Lambda(
                    BuildPropertyExpression(
                        new FilterDescriptor { PropertyName = propertyName, Value = filterDescriptor.Value },
                        listParameter),
                    listParameter);

            var propertyExpression = GetPropertyExpression(parameterExpression, collectionProperty);
            var anyExpression = BuildAnyExpression(propertyType, propertyExpression, lambda);

            return AddNullCheck(parameterExpression, filterDescriptor.PropertyName, anyExpression);
        }

        private static MethodCallExpression BuildAnyExpression(Type propertyType, Expression propertyExpression,
            Expression expression)
        {
            var enumerableType = typeof(Enumerable);
            var anyInfo = enumerableType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == AnyMethod && m.GetParameters().Length == 2);
            anyInfo = anyInfo.MakeGenericMethod(propertyType);
            return Expression.Call(anyInfo, propertyExpression, expression);
        }

        private Type GetCollectionGenericType(ParameterExpression parameterExpression, string collectionProperty)
        {
            var property = parameterExpression.Type.GetProperty(collectionProperty);
            return property == null ? default(Type) : property.PropertyType.GetGenericArguments().Single();
        }

        private Expression BuildPropertyExpression(FilterDescriptor filterDescriptor, ParameterExpression parameterExpression)
        {
            var propertyExpression = BuildBaseExpression(parameterExpression, filterDescriptor);
            return AddNullCheck(parameterExpression, filterDescriptor.PropertyName, propertyExpression);
        }

        private ParameterExpression GetParameterExpression(Type type)
        {
            return Expression.Parameter(type, ParameterName);
        }

        private Expression AddNullCheck(ParameterExpression parameter, string propertyName, Expression expression)
        {
            if (!propertyName.Contains(PropertySeparator))
            {
                return expression;
            }

            var objectProperty = propertyName.Substring(0, propertyName.IndexOf(PropertySeparator, StringComparison.Ordinal));
            var propertyExpression = GetPropertyExpression(parameter, objectProperty);
            return Expression.AndAlso(Expression.NotEqual(propertyExpression, Expression.Constant(null)), expression);
        }

        private Expression AddNullConditional(ParameterExpression parameter, string propertyName, Expression expression)
        {
            if (!propertyName.Contains(PropertySeparator))
            {
                return expression;
            }

            var objectProperty = propertyName.Substring(0, propertyName.IndexOf(PropertySeparator, StringComparison.Ordinal));
            var propertyExpression = GetPropertyExpression(parameter, objectProperty);
            return Expression.Condition(Expression.Equal(propertyExpression, Expression.Constant(null)),
                    GetDefaultValue(expression.Type), expression);
        }

        private Expression GetDefaultValue(Type type)
        {
            if (_defaultValues.ContainsKey(type))
            {
                return Expression.Constant(_defaultValues[type]); ;
            }
            return Expression.Default(type);
        }

        private Expression BuildBaseExpression(ParameterExpression parameter, FilterDescriptor filterDescriptor)
        {
            var leftSide = GetPropertyExpression(parameter, filterDescriptor.PropertyName);

            var underlyingType = Nullable.GetUnderlyingType(leftSide.Type);
            if (IsNullableType(underlyingType, filterDescriptor))
            {
                var hasValue = Expression.Property(leftSide, HasValueProperty);
                var value = Expression.Property(leftSide, ValueProperty);
                var equality = Expression.Equal(value, GetConstantValueExpression(filterDescriptor.Value, underlyingType));
                return Expression.AndAlso(hasValue, equality);
            }

            var right = GetConstantValueExpression(filterDescriptor.Value, leftSide.Type);
            return Expression.Equal(leftSide, right);
        }

        private ConstantExpression GetConstantValueExpression(string value, Type underlyingType)
        {
            if (underlyingType.IsEnum)
            {
                return Expression.Constant(Enum.Parse(underlyingType, value));
            }
            return Expression.Constant(Convert.ChangeType(value, underlyingType));
        }

        private bool IsNullableType(Type underlyingType, FilterDescriptor filterDescriptor)
        {
            return underlyingType != null && filterDescriptor.Value != null;
        }

        private Expression GetPropertyExpression(Expression parameterExpression, string propertyName)
        {
            if (!propertyName.Contains(PropertySeparator))
            {
                return Expression.Property(parameterExpression, propertyName);
            }

            var index = propertyName.IndexOf(PropertySeparator, StringComparison.Ordinal);
            var subProperty = Expression.Property(parameterExpression, propertyName.Substring(0, index));
            return GetPropertyExpression(subProperty, propertyName.Substring(index + 1));
        }

        private bool IsList(Type type, string propertyName)
        {
            if (propertyName.Contains(PropertySeparator))
            {
                propertyName = propertyName.Substring(0, propertyName.IndexOf(PropertySeparator, StringComparison.Ordinal));
            }

            return IsCollectionType(type.GetProperty(propertyName)?.PropertyType);
        }

        private bool IsCollectionType(Type type)
        {
            return type?.GetInterface(nameof(ICollection)) != null;
        }
    }
}