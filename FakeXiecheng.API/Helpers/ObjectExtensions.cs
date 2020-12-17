using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace FakeXiecheng.API.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var propertyInfos = GetProperties<TSource>(fields);
            var dataShapeObject = new ExpandoObject();

            foreach (var propertyInfo in propertyInfos)
                ((IDictionary<string, object>) dataShapeObject).Add(
                    propertyInfo.Name,
                    propertyInfo.GetValue(source));

            return dataShapeObject;
        }

        private static IEnumerable<PropertyInfo> GetProperties<TSource>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                foreach (var propertyInfo in typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                {
                    yield return propertyInfo;
                }

            if (string.IsNullOrWhiteSpace(fields))
                yield break;

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var fieldAfterTrim = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(
                    fieldAfterTrim,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    throw new Exception($"{typeof(TSource)}，找不到{fieldAfterTrim}屬性");

                yield return propertyInfo;
            }
        }
    }
}