class LiteDatabaseConverter : WriteOnlyJsonConverter<ILiteDatabase>
{
    public override void Write(VerifyJsonWriter writer, ILiteDatabase value)
    {
        var collectionNames = value.GetCollectionNames().OrderBy(x => x);
        foreach (var collectionName in collectionNames)
        {
            var collection = value.GetCollection(collectionName);
            var documents = collection.FindAll().OrderBy(x => x["_id"]);
            writer.WriteStartObject();
            writer.WritePropertyName(collectionName);
            writer.WriteStartArray();
            foreach (var document in documents)
            {
                WriteBsonDocument(writer, document);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    private void WriteBsonDocument(VerifyJsonWriter writer, BsonDocument value)
    {
        writer.WriteStartObject();
        foreach (var item in value.OrderBy(x => x.Key))
        {
            writer.WritePropertyName(item.Key);
            WriteBsonValue(writer, item.Value);
        }
        writer.WriteEndObject();
    }

    private void WriteBsonValue(VerifyJsonWriter writer, BsonValue value)
    {
        switch (value.Type)
        {
            case BsonType.ObjectId:
                writer.WriteValue(value.AsObjectId.ToString());
                break;
            case BsonType.Document:
                WriteBsonDocument(writer, value.AsDocument);
                break;
            case BsonType.Array:
                writer.WriteStartArray();
                foreach (var item in value.AsArray)
                {
                    WriteBsonValue(writer, item);
                }
                writer.WriteEndArray();
                break;
            case BsonType.String:
                SortedDictionary<string, object>? dictionary = null;
                try { dictionary = JsonConvert.DeserializeObject<SortedDictionary<string, object>>(value.AsString); }
                catch { }

                if (dictionary != null)
                {
                    var json = JsonConvert.SerializeObject(dictionary);
                    writer.WriteValue(json);
                    return;
                }

                writer.WriteValue(value.AsString);
                break;
            case BsonType.Double:
                writer.WriteValue(value.AsDouble);
                break;
            case BsonType.Int32:
                writer.WriteValue(value.AsInt32);
                break;
            case BsonType.Int64:
                writer.WriteValue(value.AsInt64);
                break;
            case BsonType.Boolean:
                writer.WriteValue(value.AsBoolean);
                break;
            case BsonType.Guid:
                writer.WriteValue(value.AsGuid);
                break;
            case BsonType.DateTime:
                writer.WriteValue(value.AsDateTime);
                break;
            case BsonType.Decimal:
                writer.WriteValue(value.AsDecimal);
                break;
            case BsonType.Null:
                writer.WriteNull();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
