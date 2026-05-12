using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Converters
{
    public class EnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (value == null) return default;

            foreach (var field in typeof(T).GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();

                if (attribute?.Value == value)
                {
                    return (T)field.GetValue(null)!;
                }

                if (field.Name == value)
                {
                    return (T)field.GetValue(null)!;
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var field = typeof(T).GetField(value.ToString());

            var attribute = field?.GetCustomAttribute<EnumMemberAttribute>();

            writer.WriteStringValue(attribute?.Value ?? value.ToString());
        }
    }
}
