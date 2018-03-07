using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicCollectionOperations.Services
{
    public interface IExpressionBuilderService
    {
        Expression<Func<TClass, bool>> GetFilterExpression<TClass>(FilterDescriptor filterDescriptor);
        MethodCallExpression GetSortExpression<TClass>(Sort criteria, ICollection<TClass> source);
    }
}
