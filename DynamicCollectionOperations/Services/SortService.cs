using System.Collections.Generic;
using System.Linq;

namespace DynamicCollectionOperations.Services
{
    public class SortService : BaseCollectionService, ISortService
    {
        private readonly IExpressionBuilderService _expressionBuilder;

        public SortService(IExpressionBuilderService expressionBuilder)
        {
            _expressionBuilder = expressionBuilder;
        }

        public ICollection<TClass> Sort<TClass>(ICollection<TClass> collection, Sort criteria)
        {
            MatchPropertyProvided<TClass>(criteria.SortColumn);
            if (!PropertyMatched())
            {
                return collection;
            }

            if (PropertyIsCollection<TClass>())
            {
                return collection;
            }

            criteria.SortColumn = PropertyName;
            var concreteCollection = collection.ToList();
            var orderByExpression = _expressionBuilder.GetSortExpression(criteria, concreteCollection);
            return concreteCollection.AsQueryable().Provider.CreateQuery<TClass>(orderByExpression).ToList();
        }

        private bool PropertyIsCollection<TClass>()
        {
            var type = typeof(TClass);
            return type.PropertyIsCollection(PropertyName);
        }
    }
}