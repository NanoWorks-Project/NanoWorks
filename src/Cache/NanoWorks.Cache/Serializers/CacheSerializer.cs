using System;
using System.Text.Json;
using NanoWorks.Cache.Options;

namespace NanoWorks.Cache.Serializers
{
    /// <summary>
    /// Serializes and deserializes objects to and from JSON for caching.
    /// </summary>
    public static class CacheSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };

        /// <summary>
        /// Serializes the specified value to a JSON string.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="exceptionBehavior">Behavior for handling exceptions thrown by the serializer.</param>
        /// <typeparam name="TItem">Type of item to serialize.</typeparam>
        public static string Serialize<TItem>(TItem value, SerializerExceptionBehavior exceptionBehavior)
        {
            try
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return JsonSerializer.Serialize(value, Options);
            }
            catch (JsonException)
            {
                if (exceptionBehavior == SerializerExceptionBehavior.Ignore)
                {
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deserializes the specified JSON string to an object of the specified type.
        /// </summary>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <param name="exceptionBehavior">Behavior for handling exceptions thrown by the serializer.</param>
        /// <typeparam name="TItem">Type of item to deserialize.</typeparam>
        public static TItem Deserialize<TItem>(string value, SerializerExceptionBehavior exceptionBehavior)
            where TItem : class, new()
        {
            try
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return JsonSerializer.Deserialize<TItem>(value, Options);
            }
            catch (JsonException)
            {
                if (exceptionBehavior == SerializerExceptionBehavior.Ignore)
                {
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
