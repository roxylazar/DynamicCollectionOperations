using System;
using System.Globalization;
using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{
    [TestClass]
    public class AppliesFilterOnASingleProperty
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
        public void FiltersCollectionByStringValue()
        {
            //Arrange
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(3)
                    .With(x => x.Name = name)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Name == name).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersCollectionByDoubleValue()
        {
            //Arrange
            const double amount = 2.98;
            var descriptor = new FilterDescriptor { PropertyName = "Amount", Value = amount.ToString(CultureInfo.CurrentCulture) };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(3)
                    .With(x => x.Amount = amount)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => Math.Abs(x.Amount - amount) < 0.001).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersCollectionByDateTimeValue()
        {
            //Arrange
            var date = new DateTime(2012, 5, 17);
            var descriptor = new FilterDescriptor { PropertyName = "Created", Value = date.ToString(CultureInfo.InvariantCulture) };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(4)
                    .With(x => x.Created = date)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Created == date).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersCollectionByEnumerationValue()
        {
            //Arrange
            const FilterContext.DummyType type = FilterContext.DummyType.All;
            var descriptor = new FilterDescriptor { PropertyName = "Type", Value = type.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(2)
                    .With(x => x.Type = type)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Type == type).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersCollectionByBooleanValue()
        {
            //Arrange
            const bool boolValue = true;
            var descriptor = new FilterDescriptor { PropertyName = "IsActive", Value = boolValue.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(6)
                    .With(x => x.IsActive = boolValue)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.IsActive == boolValue).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersCollectionByNullableValue()
        {
            //Arrange
            const int value = 45;
            var descriptor = new FilterDescriptor { PropertyName = "NumberOf", Value = value.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(3)
                    .With(x => x.NumberOf = null)
                .Random(4)
                    .With(x => x.NumberOf = value)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.NumberOf == value).Should().BeTrue();
        }
    }

}
