using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.SortServiceTests
{

    [TestClass]
    public class SortsBy
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
        public void WhenPropertyNotProvided_SortsByIdAscending()
        {
            //Arrange
            var source = Builder<SortContext.DummyClassWithId>.CreateListOfSize(2)
                .TheFirst(1)
                    .With(x => x.Id = 33)
                .TheNext(1)
                    .With(x => x.Id = 5)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort());

            //Assert
            result.Should().BeInAscendingOrder(x => x.Id);
        }

        [TestMethod]
        public void SortAscendingByStringValues()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(2)
                .All()
                    .With(x => x.Name = Faker.Name.First())
                .TheFirst(1)
                    .With(x => x.Email = "xda")
                .TheNext(1)
                    .With(x => x.Email = "abc")
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Email", SortDirection = "asc" });

            //Assert
            result.Should().BeInAscendingOrder(x => x.Email);
        }

        [TestMethod]
        public void SortAscendingBooleanValues()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                    .With(x => x.IsTrue = true)
                .TheNext(1)
                    .With(x => x.IsTrue = false)
                .TheNext(1)
                    .With(x => x.IsTrue = true)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "IsTrue" });

            //Assert
            result.Should().BeInAscendingOrder(x => x.IsTrue);
        }

        [TestMethod]
        public void SortDescendingByNullableInt()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                    .With(x => x.Value = null)
                .TheNext(1)
                    .With(x => x.Value = 30)
                .TheNext(1)
                    .With(x => x.Value = 1)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Value", SortDirection = "desc" });

            //Assert
            result.Should().BeInDescendingOrder(x => x.Value);
        }

        [TestMethod]
        public void SortDescendingByDouble()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                    .With(x => x.Amount = 1.5)
                .TheNext(1)
                    .With(x => x.Amount = 3.5)
                .TheNext(1)
                    .With(x => x.Amount = -1.98)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Amount", SortDirection = "desc" });

            //Assert
            result.Should().BeInDescendingOrder(x => x.Amount);
        }

        [TestMethod]
        public void SortDescendingByEnumeration()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                    .With(x => x.Type = SortContext.Type.Question)
                .TheNext(1)
                    .With(x => x.Type = SortContext.Type.Transition)
                .TheNext(1)
                    .With(x => x.Type = SortContext.Type.Transition)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Type", SortDirection = "desc" });

            //Assert
            result.Should().BeInDescendingOrder(x => x.Type);
        }

        [TestMethod]
        public void SortDescendingByObjectProperty()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                    .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                        .With(y => y.Amount = 22.33).Build())
                .TheNext(1)
                     .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                        .With(y => y.Amount = 32.14).Build())
                .TheNext(1)
                     .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                        .With(y => y.Amount = 28.2).Build())
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Dummy.Amount", SortDirection = "desc" });

            //Assert
            result.Should().BeInDescendingOrder(x => x.Dummy.Amount);
        }

        [TestMethod]
        public void SortAscendingBooleanValues_WhenPropertyNameIsNotFormattedAsPascalCase()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(3)
                .TheFirst(1)
                .With(x => x.IsTrue = true)
                .TheNext(1)
                .With(x => x.IsTrue = false)
                .TheNext(1)
                .With(x => x.IsTrue = true)
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "is-true" });

            //Assert
            result.Should().BeInAscendingOrder(x => x.IsTrue);
        }

        [TestMethod]
        public void WhenObjectIsNull_SortDescendingByObjectProperty()
        {
            //Arrange
            var source = Builder<SortContext.DummyClass>.CreateListOfSize(4)
                .TheFirst(1)
                .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                    .With(y => y.Amount = 22.33).Build())
                .TheNext(1)
                .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                    .With(y => y.Amount = 32.14).Build())
                .TheNext(1)
                .With(x => x.Dummy = Builder<SortContext.DummyClass>.CreateNew()
                    .With(y => y.Amount = 28.2).Build())
                .Build();

            //Act
            var result = _sortService.Sort(source, new Sort { SortColumn = "Dummy.Amount", SortDirection = "desc" });

            //Assert
            result.Last().Dummy.Should().BeNull();
            result.First().Dummy.Amount.Should().Be(32.14);
        }
    }

}
