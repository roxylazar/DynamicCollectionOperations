using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    [TestClass]
    public class SearchesForIdPropertyOfSubObjects
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
            var descriptor = new FilterDescriptor { PropertyName = "WithoutId", Value = "12" };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.WithoutId = Builder<FilterContext.WithoutId>.CreateNew().Build())
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
            var descriptor = new FilterDescriptor { PropertyName = "Data", Value = idValue.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew().Build())
                .Random(4)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(y => y.Id = idValue)
                        .Build())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Data.Id == idValue).Should().BeTrue();
        }
    }
}
