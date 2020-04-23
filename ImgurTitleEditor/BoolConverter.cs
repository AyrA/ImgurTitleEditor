using Newtonsoft.Json;
using System;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Converts imgur specific boolean values
    /// </summary>
    public class BoolConverter : JsonConverter
    {
        /// <summary>
        /// Write value "as-is"
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">value to write</param>
        /// <param name="serializer">Current serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        /// <summary>
        /// Read JSON value into boolean
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Oject type to expect</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serializer</param>
        /// <returns>convertedc boolean value</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
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
            if (double.TryParse(reader.Value.ToString(), out double d))
            {
                return !double.IsNaN(d) && d != 0.0;
            }
            throw new JsonSerializationException($"Unable to convert {reader.Value} to bool");
        }

        /// <summary>
        /// Checks if a type is processable by this class
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <returns>true, if processable</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}
