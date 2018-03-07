using System.Collections.Generic;

namespace DynamicCollectionOperations.Tests.SortServiceTests
{
    public class SortContext
    {
        public class DummyClass
        {
            public int? Value { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public double Amount { get; set; }
            public bool IsTrue { get; set; }
            public Type Type { get; set; }
            public DummyClass Dummy { get; set; }
            public List<string> Values { get; set; }
            public List<DummyClass> Datas { get; set; }
        }

        public class DummyClassWithId
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DummyClass DummyClass { get; set; }
            public DummyClassWithId ClassWithId { get; set; }
        }

        public enum Type
        {
            Question,
            Transition
        }
    }
}
