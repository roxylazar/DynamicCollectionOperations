using System;
using System.Collections.Generic;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    public class FilterContext
    {

        public enum DummyType
        {
            None,
            All,
            Partial
        }

        public class DummyData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
            public bool IsActive { get; set; }
            public DummyType Type { get; set; }
            public DateTime Created { get; set; }
            public int? NumberOf { get; set; }
            public DummyData Data { get; set; }
            public WithoutId WithoutId { get; set; }
            public List<DummyData> Datas { get; set; }
            public List<WithoutId> DataWithoutId { get; set; }
        }

        public class WithoutId
        {
            public string Name { get; set; }
        }
    }


}
