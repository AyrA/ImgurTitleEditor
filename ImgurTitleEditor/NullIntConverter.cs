using Newtonsoft.Json;
using System;

namespace ImgurTitleEditor
{
    /// <summary>
    /// Converts nullable integers
    /// </summary>
    public class NullIntConverter : JsonConverter
    {
        /// <summary>
        /// Checks if a type is processable by this class
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <returns><see cref="true"/>, if processable</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(int) == objectType;
        }

        /// <summary>
        /// Reads an optional integer from the JSON
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serializer</param>
        /// <returns>Read number, or zero if <see cref="null"/></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return 0;
            }
            return int.Parse(reader.Value.ToString());
        }

        /// <summary>
        /// Writes a value to the JSON
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">Value to write</param>
        /// <param name="serializer">JSON serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
