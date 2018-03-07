using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    [TestClass]
    public class SearchesForIdPropertyOfCollectionObject
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
        public void WhenIdNotFound_DoesNotFilter()
        {
            //Arrange
            var descriptor = new FilterDescriptor { PropertyName = "DataWithoutId", Value = "12" };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.DataWithoutId = Builder<FilterContext.WithoutId>.CreateListOfSize(2).Build().ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.ShouldBeEquivalentTo(collection);
        }

        [TestMethod]
        public void WhenIdFound_FiltersData()
        {
            //Arrange
            const int idValue = 12;
            var descriptor = new FilterDescriptor { PropertyName = "Datas", Value = idValue.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(2).Build().ToList())
                .Random(4)
                .With(x => x.Datas = Builder<FilterContext.DummyData>.CreateListOfSize(3)
                    .Random(1)
                        .With(y => y.Id = idValue)
                    .Build()
                    .ToList())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Datas.Any(y => y.Id == idValue)).Should().BeTrue();
        }
    }
}
