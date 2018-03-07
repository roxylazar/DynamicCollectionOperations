using System.Collections.Generic;

namespace DynamicCollectionOperations.Services
{
    public interface IFilterService
    {
        ICollection<TClass> ApplyFilters<TClass>(ICollection<TClass> collection, IList<FilterDescriptor> filters);

        ICollection<TClass> ApplyFilter<TClass>(ICollection<TClass> collection, FilterDescriptor filter);
    }
}
