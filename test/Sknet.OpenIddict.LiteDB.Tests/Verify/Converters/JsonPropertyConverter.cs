/*
 * MIT License
 *
 * Copyright (c) Simon Cropp
 */
class JsonPropertyConverter : WriteOnlyJsonConverter<JsonProperty>
{
    public override void Write(VerifyJsonWriter writer, JsonProperty value)
    {
        writer.WritePropertyName(value.Name);
        writer.Serialize(value.Value);
    }
}