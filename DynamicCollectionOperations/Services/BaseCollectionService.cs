using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DynamicCollectionOperations.Services
{
    public class BaseCollectionService
    {
        private const string IdProperty = "Id";
        private const string CultureInfoName = "en-US";
        private const char StringSeparator = '-';

        protected string PropertyName;

        protected void MatchPropertyProvided<TClass>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = IdProperty;
            }

            propertyName = HandleCasing(propertyName);

            var type = typeof(TClass);
            if (!type.HasProperty(propertyName))
            {
                return;
            }

            if (type.IsCustomObject(propertyName))
            {
                if (!HasIdProperty(type, propertyName))
                {
                    return;
                }
                propertyName = $"{propertyName}.{IdProperty}";
            }

            PropertyName = propertyName;
        }

        protected bool PropertyMatched()
        {
            return !string.IsNullOrEmpty(PropertyName);
        }

        private string HandleCasing(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var textInfo = new CultureInfo(CultureInfoName, false).TextInfo;
            if (!value.Contains(StringSeparator.ToString()))
            {
                return char.IsLower(value.First()) ? textInfo.ToTitleCase(value) : value;
            }

            var values = value.Split(StringSeparator);
            var builder = new StringBuilder();
            foreach (var word in values)
            {
                builder.Append(textInfo.ToTitleCase(word));
            }

            return builder.ToString();
        }

        private bool HasIdProperty(Type type, string propertyName)
        {
            var idProperty = $"{propertyName}.{IdProperty}";
            return type.HasProperty(idProperty);
        }
    }
}
