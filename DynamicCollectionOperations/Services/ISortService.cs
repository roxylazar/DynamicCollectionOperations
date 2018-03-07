using System.Collections.Generic;

namespace DynamicCollectionOperations.Services
{
    public interface ISortService
    {
        ICollection<TClass> Sort<TClass>(ICollection<TClass> collection, Sort criteria);
    }
}