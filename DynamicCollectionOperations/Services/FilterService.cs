using System.Collections.Generic;
using System.Linq;

namespace DynamicCollectionOperations.Services
{
    public class FilterService : BaseCollectionService, IFilterService
    {
        private readonly IExpressionBuilderService _expressionBuilder;

        public FilterService(IExpressionBuilderService expressionBuilder)
        {
            _expressionBuilder = expressionBuilder;
        }

        public ICollection<TClass> ApplyFilters<TClass>(ICollection<TClass> collection, IList<FilterDescriptor> filters)
        {
            if (!CollectionHasValues(collection) || !FiltersProvided(filters))
            {
                return collection;
            }

            return filters.Aggregate(collection, ApplyFilter);
        }

        public ICollection<TClass> ApplyFilter<TClass>(ICollection<TClass> collection, FilterDescriptor filterDescriptor)
        {
            if (!CollectionHasValues(collection))
            {
                return collection;
            }

            if (!FilterProvided(filterDescriptor))
            {
                return collection;
            }

            MatchPropertyProvided<TClass>(filterDescriptor.PropertyName);
            if (!PropertyMatched())
            {
                return collection;
            }

            filterDescriptor.PropertyName = PropertyName;
            var expression = _expressionBuilder.GetFilterExpression<TClass>(filterDescriptor);
            return collection.Where(expression.Compile()).ToList();
        }

        private static bool CollectionHasValues<TClass>(ICollection<TClass> collection)
        {
            return collection != null && collection.Any();
        }

        private static bool FiltersProvided(IList<FilterDescriptor> filters)
        {
            return filters != null && filters.Any();
        }

        private static bool FilterProvided(FilterDescriptor filterDescriptor)
        {
            return filterDescriptor != null;
        }
    }
}