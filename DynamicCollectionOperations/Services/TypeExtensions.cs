using System;
using System.Collections;
using System.Linq;

namespace DynamicCollectionOperations.Services
{
    public static class TypeExtensions
    {
        private const string PropertySeparator = ".";
        private const string System = "system";
        private const string MsAssemblyName = "mscorlib";

        public static bool IsCollection(this Type type)
        {
            return type?.GetInterface(nameof(ICollection)) != null;
        }

        public static bool HasProperty(this Type type, string propertyName)
        {
            if (type.IsCollection())
            {
                type = type.GetGenericArguments().Single();
            }

            if (!IsNestedProperty(propertyName))
            {
                return type.GetProperty(propertyName) != null;
            }

            var propertyData = GetNestedValues(type, propertyName);
            return HasProperty(propertyData.NestedType, propertyData.NestedPropertyName);
        }


        public static bool PropertyIsCollection(this Type type, string propertyName)
        {
            if (!IsNestedProperty(propertyName))
            {
                return type.GetProperty(propertyName).PropertyType.IsCollection();
            }

            var propertyData = GetNestedValues(type, propertyName);
            return propertyData.NestedType.IsCollection() ||
                propertyData.NestedType.PropertyIsCollection(propertyData.NestedPropertyName);
        }

        public static bool IsCustomObject(this Type type, string propertyName)
        {
            if (type.IsCollection())
            {
                type = type.GetGenericArguments().Single();
            }

            if (!IsNestedProperty(propertyName))
            {
                return type.GetProperty(propertyName).PropertyType.IsCustom();
            }

            var propertyData = GetNestedValues(type, propertyName);
            return propertyData.NestedType.IsCustomObject(propertyData.NestedPropertyName);
        }

        private static bool IsCustom(this Type type)
        {
            if (type.IsCollection())
            {
                type = type.GetGenericArguments().Single();
            }

            return type.IsClass &&
                   !(type.FullName.ToLower().Contains(System) &&
                     type.Assembly.FullName.ToLower().Contains(MsAssemblyName));
        }

        private static PropertyData GetNestedValues(Type type, string propertyName)
        {
            var index = propertyName.IndexOf(PropertySeparator, StringComparison.Ordinal);
            var objectProperty = propertyName.Substring(0, index);
            return new PropertyData
            {
                NestedType = type.GetProperty(objectProperty)?.PropertyType,
                NestedPropertyName = propertyName.Substring(index + 1)
            };
        }

        private static bool IsNestedProperty(string propertyName)
        {
            return propertyName.Contains(PropertySeparator);
        }
    }
}