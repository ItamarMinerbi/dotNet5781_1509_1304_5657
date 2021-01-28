using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DAL
{
    static class Cloning
    {
        public static T Clone<T>(this T source) where T : new()
        {
            T result = new T();
            foreach (var property in typeof(T).GetProperties())
                property.SetValue(result, property.GetValue(source));
            return result;
        }
    }
}
