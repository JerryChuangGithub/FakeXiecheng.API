using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new string[] { "Id" })},
                { "Title", new PropertyMappingValue(new string[] { "Title" })},
                { "Rating", new PropertyMappingValue(new string[] { "Rating" })},
                { "OriginalPrice", new PropertyMappingValue(new string[] { "OriginalPrice" })},
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(
                new PropertyMapping<TouristRouteDto, TouristRoute>(_touristRoutePropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>()
                .FirstOrDefault();

            if (matchingMapping != null)
                return matchingMapping.MappingDictionary;
            
            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}>");
        }

        public bool IsMappingExists<TSource, TDestination>(string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return true;

            var mappingDictionary = GetPropertyMapping<TSource, TDestination>();

            foreach (var order in orderBy.Split(','))
            {
                var trimmedOrder = order.Trim();
                var indexOfFirstSpace = trimmedOrder.IndexOf(" ", StringComparison.Ordinal);

                var propertyName = indexOfFirstSpace == -1
                    ? trimmedOrder
                    : trimmedOrder.Remove(indexOfFirstSpace);

                if (mappingDictionary.ContainsKey(propertyName) == false)
                    return false;
            }

            return true;
        }

        public bool IsPropertiesExists<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var fieldAfterTrim = field.Trim();
                var propertyInfo = typeof(T).GetProperty(fieldAfterTrim, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    return false;
            }

            return true;
        }
    }
}