using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DynamicCollectionOperations.Services;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicCollectionOperations.Tests.FilterServiceTests
{

    [TestClass]
    public class AppliesFilterOnSubObjectProperty
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
        public void FiltersWhenObjectPropertyIsNull()
        {
            //Arrange
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Data.Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(3)
                    .With(x => x.Name = name)
                    .With(x => x.Data = null)
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FiltersByObjectStringProperty()
        {
            //Arrange
            const string name = "John";
            var descriptor = new FilterDescriptor { PropertyName = "Data.Name", Value = name };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew().Build())
                .Random(3)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.Name = name).Build())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Data.Name == name).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersByObjectNullableProperty()
        {
            //Arrange
            const int value = 45;
            var descriptor = new FilterDescriptor { PropertyName = "Data.NumberOf", Value = value.ToString() };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew().Build())
                .Random(3)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.NumberOf = value).Build())
                .Random(2)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.NumberOf = null).Build())
                .Build();

            //Act
            var result = _filter.ApplyFilter(collection, descriptor);

            //Assert
            result.All(x => x.Data.NumberOf == value).Should().BeTrue();
        }

        [TestMethod]
        public void FiltersTwoPropertiesOfObject()
        {
            //Arrange
            const string name = "John";
            DateTime created = new DateTime(2016, 2, 12);

            var descriptors = new List<FilterDescriptor> {
                new FilterDescriptor {PropertyName = "Data.Name", Value = name},
                new FilterDescriptor {PropertyName = "Data.Created", Value = created.ToString(CultureInfo.InvariantCulture)}
            };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .All()
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew().Build())
                .Random(3)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.Name = name)
                        .With(d => d.Created = created)
                        .Build())
                .Random(2)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.Name = name).Build())
                .Random(1)
                    .With(x => x.Data = Builder<FilterContext.DummyData>.CreateNew()
                        .With(d => d.Created = created).Build())
                .Build();

            //Act
            var result = _filter.ApplyFilters(collection, descriptors);

            //Assert
            result.All(x => x.Data.Name == name && x.Data.Created == created).Should().BeTrue();
            result.Count.Should().Be(3);
        }
    }
}
