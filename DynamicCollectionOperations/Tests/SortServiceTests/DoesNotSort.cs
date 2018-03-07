using System.Collections.Generic;
using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DynamicCollectionOperations.Tests.SortServiceTests
{
    [TestClass]
    public class DoesNotSort
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
        public void WhenSourceIsEmpty()
        {
            //Arrange
            var source = (ICollection<SortContext.DummyClass>)Enumerable.Empty<SortContext.DummyClass>();
            var sort = new Sort { SortColumn = "email" };

            //Act
            var result = _sortService.Sort(source, sort);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void WhenSourceDoesNotHaveAnIdProperty()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(2)
                .All()
                    .With(x => x.Name = Faker.Name.First())
                    .With(x => x.Email = Faker.Internet.Email())
                .Build();
            var sort = new Sort();

            //Act
            var result = _sortService.Sort(source, sort);

            //Assert
            result.Should().BeEquivalentTo(source);
        }

        [TestMethod]
        public void WhenPropertyNotFound()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(2)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "todo", SortDirection = "asc" });

            //Assert
            result.ShouldBeEquivalentTo(source);
        }

        [TestMethod]
        public void WhenSortDirectionIsWrong()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(2)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "name", SortDirection = "blahblah" });

            //Assert
            result.ShouldBeEquivalentTo(source);
        }

        [TestMethod]
        public void WhenPropertyIsList()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .All()
                    .With(x => x.Values = new List<string> { "a", "c", "b" })
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Values" });

            //Assert
            result.ShouldBeEquivalentTo(source);
        }

        [TestMethod]
        public void WhenPropertyIsCollectionOfObjects()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .All()
                    .With(x => x.Datas = Builder<SortContext.DummyClass>.CreateListOfSize(5)
                        .Build().ToList())
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Datas.Amount" });

            //Assert
            result.ShouldBeEquivalentTo(source);
        }

    }
}
