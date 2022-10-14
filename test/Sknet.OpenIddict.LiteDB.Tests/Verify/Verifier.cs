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
using System.Runtime.CompilerServices;

namespace Sknet.OpenIddict.LiteDB.Tests;

public static class Verifier
{
    public static SettingsTask Verify<T>(
        T target,
        ILiteDatabase database,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var collections = new Dictionary<string, IEnumerable<JsonDocument>>();
        foreach (var name in database.GetCollectionNames())
        {
            var collection = database.GetCollection(name);
            collections[name] = collection.FindAll().Select(x => JsonDocument.Parse(global::LiteDB.JsonSerializer.Serialize(x)));
        }

        return VerifyXunit.Verifier.Verify(new { target, database = collections }, settings, sourceFile);
    }

    public static SettingsTask Verify(
        ILiteDatabase database,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        var collections = new Dictionary<string, IEnumerable<JsonDocument>>();
        foreach (var name in database.GetCollectionNames())
        {
            var collection = database.GetCollection(name);
            collections[name] = collection.FindAll().Select(x => JsonDocument.Parse(global::LiteDB.JsonSerializer.Serialize(x)));
        }

        return VerifyXunit.Verifier.Verify(new { database = collections }, settings, sourceFile);
    }
}
