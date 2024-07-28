// Ignore Spelling: Nano

using System.Text.Json;

namespace NanoWorks.Cache.Redis.Extensions;

/// <summary>
/// Serialization extensions.
/// </summary>
public static class Serialization
{
    /// <summary>
    /// Serialize the object to a JSON string.
    /// </summary>
    /// <param name="obj">Object to serialize.</param>
    /// <returns>JSON string.</returns>
    public static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Deserialize the JSON string to an object.
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize.</typeparam>
    /// <param name="json">JSON string.</param>
    /// <returns>Deserialized object.</returns>
    public static T FromJson<T>(this string json)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }
}
