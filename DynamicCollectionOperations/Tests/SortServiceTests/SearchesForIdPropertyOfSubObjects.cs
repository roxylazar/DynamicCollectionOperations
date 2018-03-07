using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.SortServiceTests
{
    [TestClass]
    public class SearchesForIdPropertyOfSubObjects
    {
        private ISortService _sortService;
        private IExpressionBuilderService _expressionBuilder;

        [TestInitialize]
        public void Setup()
        {
            _expressionBuilder = new ExpressionBuilderService();
            _sortService = new SortService(_expressionBuilder);
        }

        [TestMethod]
        public void WhenIdNotFound_DoesNotSort()
        {
            //Arrange
            var source = Builder<SortContext.DummyClassWithId>.CreateListOfSize(2)
                .All()
                    .With(x => x.DummyClass = Builder<SortContext.DummyClass>.CreateNew().Build())
                .TheFirst(1)
                    .With(x => x.Id = 7)
                .TheNext(1)
                    .With(x => x.Id = 5)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "DummyClass" });

            //Assert
            result.Should().BeInDescendingOrder(x => x.Id);
        }

        [TestMethod]
        public void WhenIdFound_ItSorts()
        {
            //Arrange
            var source = Builder<SortContext.DummyClassWithId>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(x => x.ClassWithId = Builder<SortContext.DummyClassWithId>.CreateNew()
                        .With(y => y.Id = 44)
                    .Build())
                .TheNext(1)
                    .With(x => x.ClassWithId = Builder<SortContext.DummyClassWithId>.CreateNew()
                        .With(y => y.Id = 17)
                    .Build())
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "ClassWithId" });

            //Assert
            result.Should().BeInAscendingOrder(x => x.ClassWithId.Id);
        }

    }
}
