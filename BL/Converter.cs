using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    static class Converter
    {
        public static TResult Convert<TOriginal, TResult>(this TOriginal Original) where TResult : new()
        {
            TResult result = new TResult();
            foreach (var property in typeof(TResult).GetProperties())
            {
                PropertyInfo propertyInfo = typeof(TOriginal).GetProperty(property.Name);
                if (propertyInfo != null && propertyInfo.PropertyType == property.PropertyType)
                {
                    if(property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                        property.SetValue(result, propertyInfo.GetValue(Original));
                }
            }
            return result;
        }
    }
}
