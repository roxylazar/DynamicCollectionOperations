using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    [TestClass]
    public class AppliesFilterOnObjectCollection
    {
        private IFilterService _filter;
        private IExpressionBuilderService _expressionBuilder;

        [TestInitialize]
        public void Setup()
        {
            _expressionBuilder = new ExpressionBuilderService();
            _filter = new FilterService(_expressionBuilder);
        }

        [TestMethod]
        public void FiltersWhenCollectionIsNull()
        {
            //Arrange
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Datas.Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Datas = null)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FiltersByObjectFromCollectionStringProperty()
        {
            //Arrange
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Datas.Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(3)
                    .Build().ToList())
                .Random(5)
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .Random(2)
                        .With(y => y.Name = name)
                    .Build().ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Datas.Any(y => y.Name == name)).Should().BeTrue();
            result.Count.Should().Be(5);
        }

        [TestMethod]
        public void FiltersByObjectFromCollectionNullableProperty()
        {
            //Arrange
            const int number = 34;
            var descriptor = new FilterDescriptor { PropertyName = "Datas.NumberOf", Value = number.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(3)
                        .Build().ToList())
                .Random(2)
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .All()
                        .With(y => y.NumberOf = null)
                    .Build().ToList())
                .Random(3)
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                        .All()
                            .With(y => y.NumberOf = 56)
                        .Build().ToList())
                .Random(4)
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                        .Random(3)
                            .With(y => y.NumberOf = number)
                        .Build().ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Datas.Any(y => y.NumberOf == number)).Should().BeTrue();
            result.Count.Should().Be(4);
        }

        [TestMethod]
        public void FiltersByObjectFromCollectionObjectProperty()
        {
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Datas.Data.Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(3)
                    .Build().ToList())
                .Random(6)
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .Random(3)
                        .With(y => y.Data = Builder<FilterContext.DummyData>.CreateNew()
                            .With(z => z.Name = name).Build())
                    .Build().ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Datas.Any(y => y.Data != null && y.Data.Name == name)).Should().BeTrue();
            result.Count.Should().Be(6);
        }

        [TestMethod]
        public void FiltersByObjectFromCollectionNullableProperty_AndHandlesCasing()
        {
            //Arrange
            const int number = 34;
            var descriptor = new FilterDescriptor { PropertyName = "datas.number-of", Value = number.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(3)
                    .Build().ToList())
                .Random(2)
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .All()
                    .With(y => y.NumberOf = null)
                    .Build().ToList())
                .Random(3)
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .All()
                    .With(y => y.NumberOf = 56)
                    .Build().ToList())
                .Random(4)
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(5)
                    .Random(3)
                    .With(y => y.NumberOf = number)
                    .Build().ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Datas.Any(y => y.NumberOf == number)).Should().BeTrue();
            result.Count.Should().Be(4);
        }

    }
}
