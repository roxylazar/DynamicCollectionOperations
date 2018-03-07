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
    public class AppliesFilterOnMultipleProperties
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
        public void FiltersByTwoProperties()
        {
            //Arrange
            const string name = "John";
            DateTime created = new DateTime(2016, 2, 12);

            var descriptors = new List<FilterDescriptor> {
                new FilterDescriptor {PropertyName = "Name", Value = name},
                new FilterDescriptor {PropertyName = "Created", Value = created.ToString()}
            };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(3)
                    .With(x => x.Name = name)
                    .With(x => x.Created = created)
                .Random(2)
                    .With(x => x.Name = name)
                .Random(1)
                    .With(x => x.Created = created)
                .Build();

            //Act
            var result = _filter.ApplyFilters(collection, descriptors);

            //Assert
            result.All(x => x.Name == name && x.Created == created).Should().BeTrue();
            result.Count.Should().Be(3);
        }

        [TestMethod]
        public void FiltersByThreeProperties()
        {
            //Arrange
            const string name = "John";
            DateTime created = new DateTime(2016, 2, 12);
            const int numberof = 3;

            var descriptors = new List<FilterDescriptor> {
                new FilterDescriptor {PropertyName = "Name", Value = name},
                new FilterDescriptor {PropertyName = "Created", Value = created.ToString(CultureInfo.InvariantCulture)},
                new FilterDescriptor {PropertyName = "NumberOf", Value = numberof.ToString()}
            };
            var collection = Builder<FilterContext.DummyData>.CreateListOfSize(10)
                .Random(2)
                    .With(x => x.Name = name)
                    .With(x => x.NumberOf = numberof)
                .Random(3)
                    .With(x => x.Name = name)
                    .With(x => x.Created = created)
                    .With(x => x.NumberOf = numberof)
                .Random(2)
                    .With(x => x.Name = name)
                .Random(1)
                    .With(x => x.Created = created)
                .Random(1)
                    .With(x => x.NumberOf = null)
                .Build();

            //Act
            var result = _filter.ApplyFilters(collection, descriptors);

            //Assert
            result.All(x => x.Name == name && x.Created == created && x.NumberOf.Value == numberof)
                .Should().BeTrue();
            result.Count.Should().Be(3);
        }
    }
}
