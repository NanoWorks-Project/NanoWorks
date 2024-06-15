// Ignore Spelling: Nano

using System;
using System.Text.Json;

namespace NanoWorks.Messaging.Serialization
{
    /// <summary>
    /// Serializes and deserializes objects to and from JSON for messaging.
    /// </summary>
    public static class MessageSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };

        /// <summary>
        /// Serializes the specified value to a byte array of utf8 characters.
        /// </summary>
        /// <typeparam name="TItem">Type of item to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        public static byte[] Serialize<TItem>(TItem value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return JsonSerializer.SerializeToUtf8Bytes(value, typeof(TItem), Options);
        }

        /// <summary>
        /// Deserializes the specified JSON stream to an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="data">The JSON stream to deserialize.</param>
        public static object Deserialize(Type type, ReadOnlySpan<byte> data)
        {
            var reader = new Utf8JsonReader(data);
            return JsonSerializer.Deserialize(ref reader, type, Options);
        }

        /// <summary>
        /// Deserializes the specified JSON string to an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="data">The JSON string to deserialize.</param>
        public static object Deserialize(Type type, string data)
        {
            return JsonSerializer.Deserialize(data, type, Options);
        }
    }
}