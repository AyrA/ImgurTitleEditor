using Newtonsoft.Json;
using System;

namespace ImgurTitleEditor
{
    public class NullIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(int) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return 0;
            }
            return int.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
