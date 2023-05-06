using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventBus2
{
    public static class JsonSerializerExtenstions
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Error
        };

        public static string ToJson(this object item, JsonSerializerSettings settings = null) => JsonConvert.SerializeObject(item, settings ?? JsonSerializerExtenstions._settings);

        public static T FromJsonToObj<T>(this string jsonString, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(jsonString, settings ?? JsonSerializerExtenstions._settings);

        public static object FromJsonToObj(
          this string value,
          Type type,
          JsonSerializerSettings settings)
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));
            return value == null ? (object)null : JsonConvert.DeserializeObject(value, type, settings);
        }
    }
}
