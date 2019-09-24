using Newtonsoft.Json;
using System;

namespace ImgurTitleEditor
{
    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            double d = 0;

            //Treat null as false
            if (reader.Value == null)
            {
                return false;
            }

            //Keep booleans as-is
            if (reader.Value is bool)
            {
                return (bool)reader.Value;
            }

            //Treat numbers as booleans
            if (double.TryParse(reader.Value.ToString(), out d))
            {
                return !double.IsNaN(d) && d != 0.0;
            }
            throw new JsonSerializationException($"Unable to convert {reader.Value} to bool");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}
