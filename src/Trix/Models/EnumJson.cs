using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trix.Models;

/// <summary>
/// A <see cref="JsonConverterFactory"/> for enums that serializes each member
/// using the wire value declared by its <see cref="JsonPropertyNameAttribute"/>.
/// </summary>
/// <remarks>
/// The built-in <see cref="JsonStringEnumConverter"/> <b>ignores</b>
/// <c>[JsonPropertyName]</c> on enum members — it only applies an optional naming
/// policy to the C# member name. That means <c>MemoryType.Text</c> serializes as
/// <c>"Text"</c> instead of the <c>"text"</c> the API requires, and reading a
/// snake_case value such as <c>"auto_delete"</c> throws. This factory honors the
/// annotations, producing the correct wire contract on both net8.0 and net10.0
/// (the <c>[JsonStringEnumMemberName]</c> attribute is .NET 9+ only, so it cannot
/// be used while the SDK still targets net8.0). Members that lack a
/// <c>[JsonPropertyName]</c> fall back to the snake_case of the member name.
/// </remarks>
public sealed class JsonStringEnumMemberConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter)Activator.CreateInstance(
            typeof(EnumMemberConverter<>).MakeGenericType(typeToConvert))!;

    private sealed class EnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for enum {typeof(T).Name}, got {reader.TokenType}.");

            var raw = reader.GetString()!;
            if (EnumWireMap.For(typeof(T)).FromWire.TryGetValue(raw, out var value))
                return (T)value;

            throw new JsonException($"\"{raw}\" is not a valid {typeof(T).Name} value.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(EnumWireMap.ToWire(value));
    }
}

/// <summary>
/// Builds and caches the bidirectional wire-value maps for an enum from the
/// <see cref="JsonPropertyNameAttribute"/> on each member. Shared by the JSON
/// converter and by non-JSON serialization paths (e.g. query-string parameters)
/// so both use a single source of truth.
/// </summary>
internal static class EnumWireMap
{
    internal sealed record Maps(
        IReadOnlyDictionary<Enum, string> ToWire,
        IReadOnlyDictionary<string, object> FromWire);

    private static readonly ConcurrentDictionary<Type, Maps> Cache = new();

    /// <summary>Gets (building on first use) the wire maps for an enum type.</summary>
    public static Maps For(Type enumType) => Cache.GetOrAdd(enumType, Build);

    /// <summary>Returns the wire value for a single enum value.</summary>
    public static string ToWire(Enum value) =>
        For(value.GetType()).ToWire.TryGetValue(value, out var wire) ? wire : value.ToString();

    private static Maps Build(Type enumType)
    {
        var toWire = new Dictionary<Enum, string>();
        var fromWire = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var value = (Enum)field.GetValue(null)!;
            var wire = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                       ?? JsonNamingPolicy.SnakeCaseLower.ConvertName(field.Name);

            toWire[value] = wire;
            fromWire[wire] = value;               // accept the wire value on read
            fromWire.TryAdd(field.Name, value);   // also tolerate the C# member name
        }

        return new Maps(toWire, fromWire);
    }
}

/// <summary>
/// Helpers for emitting enum values on non-JSON transports (query strings, path
/// segments) using the same <c>[JsonPropertyName]</c> wire contract as the body
/// serializer, so a multi-word member such as <c>RetentionPolicy.AutoDelete</c>
/// is sent as <c>auto_delete</c> rather than a lower-cased C# name.
/// </summary>
internal static class EnumWireExtensions
{
    /// <summary>Returns the API wire value for an enum (e.g. for a query parameter).</summary>
    public static string ToWireValue(this Enum value) => EnumWireMap.ToWire(value);
}
