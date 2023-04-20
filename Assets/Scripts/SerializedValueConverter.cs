#nullable enable
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public class SerializedValueConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var contractResolver = serializer.ContractResolver as DefaultContractResolver;

        writer.WriteStartObject();

        var properties = value.GetType().GetProperties();
        foreach (var property in properties)
        {
            var isSimpleType =
                property.PropertyType.IsPrimitive
                || property.PropertyType == typeof(string)
                || property.PropertyType == typeof(DateTime)
                || property.PropertyType.IsValueType;

            var propertyName =
                property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName
                ?? contractResolver?.GetResolvedPropertyName(property.Name)
                ?? property.Name;

            writer.WritePropertyName(propertyName);

            if (isSimpleType)
            {
                var propertyValue = property.GetValue(value);
                if (propertyValue == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.WriteValue(propertyValue);
                }
            }
            else
            {
                string serializedComplexProperty = JsonConvert.SerializeObject(
                    property.GetValue(value),
                    serializer.Formatting,
                    new JsonSerializerSettings { ContractResolver = contractResolver });

                writer.WriteValue(serializedComplexProperty);
            }
        }

        writer.WriteEndObject();
    }

    // Original implementation: https://stackoverflow.com/a/62265454
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var contract = serializer.ContractResolver.ResolveContract(objectType) as JsonObjectContract
                       ?? throw new JsonException($"{objectType} is not a JSON object");

        var jo = JToken.Load(reader);
        if (jo.Type == JTokenType.Null)
        {
            return null;
        }
        else if (jo.Type != JTokenType.Object)
        {
            throw new JsonSerializationException($"Unexpected token {jo.Type}");
        }

        var targetObj = contract.DefaultCreator!();

        // Handle deserialization callbacks
        foreach (var callback in contract.OnDeserializingCallbacks)
        {
            callback(targetObj, serializer.Context);
        }

        foreach (var property in contract.Properties)
        {
            // Check that property isn't ignored, and can be deserialized.
            if (property.Ignored || !property.Writable)
            {
                continue;
            }

            if (property.ShouldDeserialize != null && !property.ShouldDeserialize(targetObj))
            {
                continue;
            }

            var jsonPath = property.PropertyName;
            var token = jo.SelectToken(jsonPath!);

            // TODO: default values, skipping nulls, PreserveReferencesHandling, ReferenceLoopHandling, ...
            if (token != null && token.Type != JTokenType.Null)
            {
                object value;
                // Call the property's converter if present, otherwise deserialize directly.
                if (property.Converter != null && property.Converter.CanRead)
                {
                    using (var subReader = token.CreateReader())
                    {
                        if (subReader.TokenType == JsonToken.None)
                        {
                            subReader.Read();
                        }

                        value = property.Converter.ReadJson(subReader, property.PropertyType!, property.ValueProvider!.GetValue(targetObj), serializer)!;
                    }
                }
                else if (property.PropertyType == typeof(string))
                {
                    value = token.ToString();
                }
                else if (property.PropertyType?.IsValueType == true || property.PropertyType?.IsPrimitive == true)
                {
                    value = token.ToObject(property.PropertyType)!;
                }
                else
                {
                    value = JsonConvert.DeserializeObject(token.ToString(), property.PropertyType!)!;
                }

                property.ValueProvider!.SetValue(targetObj, value);
            }
        }

        // Handle deserialization callbacks
        foreach (var callback in contract.OnDeserializedCallbacks)
            callback(targetObj, serializer.Context);

        return targetObj;
    }

    public override bool CanConvert(Type objectType) => true;
}