/*
 * Copyright (c) 2022 Steven Kuhn and contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using LiteDB.Engine;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sknet.OpenIddict.LiteDB;

/// <inheritdoc/>
public class OpenIddictLiteDatabase : LiteDatabase
{
    /// <inheritdoc/>
    public OpenIddictLiteDatabase(string connectionString, BsonMapper? mapper = null)
        : base(connectionString, mapper)
    {
        ConfigureMapper();
    }

    /// <inheritdoc/>
    public OpenIddictLiteDatabase(ConnectionString connectionString, BsonMapper? mapper = null)
        : base(connectionString, mapper)
    {
        ConfigureMapper();
    }

    /// <inheritdoc/>
    public OpenIddictLiteDatabase(Stream stream, BsonMapper? mapper = null, Stream? logStream = null)
        : base(stream, mapper, logStream)
    {
        ConfigureMapper();
    }

    /// <inheritdoc/>
    public OpenIddictLiteDatabase(ILiteEngine engine, BsonMapper? mapper = null, bool disposeOnClose = true)
        : base(engine, mapper, disposeOnClose)
    {
        ConfigureMapper();
    }

    private void ConfigureMapper()
    {
        if (Mapper == null)
        {
            throw new NotImplementedException();
        }

        Mapper.RegisterType<ImmutableDictionary<CultureInfo, string>>
        (
            serialize: dictionary =>
            {
                if (dictionary == null) { return null; }

                var document = new BsonDocument();
                foreach (var pair in dictionary)
                {
                    document.Add(pair.Key.Name, pair.Value);
                }
                return document;
            },
            deserialize: bson => bson.AsDocument.ToImmutableDictionary(
                pair => new CultureInfo(pair.Key),
                pair => pair.Value.AsString)
        );
        Mapper.RegisterType<ImmutableArray<string>>
        (
            serialize: items => items != null
                ? new BsonArray(items.Select(x => new BsonValue(x)))
                : null,
            deserialize: bson => bson.AsArray.Select(x => x.AsString).ToImmutableArray()
        );
        Mapper.RegisterType<ImmutableDictionary<string, JsonElement>>
        (
            serialize: dictionary => dictionary != null
                ? JsonSerializer.Serialize(dictionary)
                : null,
            deserialize: bson => JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(bson.AsString)
                ?? ImmutableDictionary<string, JsonElement>.Empty
        );
    }
}
