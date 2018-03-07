using System.Collections.Generic;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    [TestClass]
    public class ReturnsProvidedCollection
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
        public void WhenCollectionIsNull()
        {
            //Arrange
            var filters = new List<FilterDescriptor> { new FilterDescriptor() };

            //Act
            var result = _filter.ApplyFilters<FilterContext.DummyData>(null, filters);

            //Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void WhenCollectionIsEmpty()
        {
            //Arrange
            var filters = new List<FilterDescriptor> { new FilterDescriptor() };

            //Act
            var result = _filter.ApplyFilters(new List<FilterContext.DummyData>(), filters);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void WhenNullDescriptorProvided()
        {
            //Arrange
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10).Build();

            //Act
            var result = _filter.ApplyFilters(collection, null);

            //Assert
            result.Should().BeEquivalentTo(collection);
        }

        [TestMethod]
        public void WhenNoDescriptorProvided()
        {
            //Arrange
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10).Build();

            //Act
            var result = _filter.ApplyFilters(collection, new List<FilterDescriptor>());

            //Assert
            result.Should().BeEquivalentTo(collection);
        }
    }
}
