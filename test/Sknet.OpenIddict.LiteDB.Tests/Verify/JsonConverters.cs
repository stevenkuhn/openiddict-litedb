class JsonDocumentConverter :
    WriteOnlyJsonConverter<JsonDocument>
{
    public override void Write(VerifyJsonWriter writer, JsonDocument value) =>
        writer.Serialize(value.RootElement);
}

class JsonElementConverter :
    WriteOnlyJsonConverter<JsonElement>
{
    public override void Write(VerifyJsonWriter writer, JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var item in value.EnumerateObject())
                {
                    writer.Serialize(item);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in value.EnumerateArray())
                {
                    writer.Serialize(item);
                }
                writer.WriteEndArray();
                break;
            case JsonValueKind.String:
                writer.WriteValue(value.GetString());
                break;
            case JsonValueKind.Number:
                writer.WriteValue(value.GetDouble());
                break;
            case JsonValueKind.True:
                writer.WriteValue(true);
                break;
            case JsonValueKind.False:
                writer.WriteValue(false);
                break;
            case JsonValueKind.Null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

class JsonPropertyConverter :
    WriteOnlyJsonConverter<JsonProperty>
{
    public override void Write(VerifyJsonWriter writer, JsonProperty value)
    {
        writer.WritePropertyName(value.Name);
        writer.Serialize(value.Value);
    }
}