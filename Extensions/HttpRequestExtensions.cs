using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Web;

namespace NetCoreExampleApi.Extensions;

public static class HttpRequestExtensions
{
    public static string ToQueryString(this object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        List<string> result = new List<string>();

        IEnumerable<PropertyInfo> props = obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) != null);

        foreach (var p in props)
        {
            if (Attribute.IsDefined(p, typeof(IgnoreDataMemberAttribute)))
            {
                continue;
            }

            string name = p.Name;
            
            if (Attribute.IsDefined(p, typeof(JsonPropertyNameAttribute)))
            {
                dynamic attribute = p.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).First();
                name = attribute.Name;
            }

            object value = p.GetValue(obj, null);

            if (value is ICollection enumerable)
            {
                result.AddRange(from object v in enumerable select $"{name}={HttpUtility.UrlEncode(v.ToString())}");
            }
            else if (value != null)
            {
                result.Add(value is DateTime ? $"{name}={HttpUtility.UrlEncode($"{value:yyyy-MM-dd HH:mm:ss.fff}")}" : $"{name}={HttpUtility.UrlEncode(value.ToString())}");
            }
        }

        return string.Join("&", result.ToArray());
    }
}