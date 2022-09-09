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
using LiteDB;
using LiteDB.Engine;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Sknet.OpenIddict.LiteDB.Tests;

public class DatabaseBuilder
{
    private readonly List<OpenIddictLiteDBApplication> _applications = new();

    public DatabaseBuilder WithApplication(OpenIddictLiteDBApplication application)
    {
        _applications.Add(application);
        return this;
    }

    public DatabaseBuilder WithApplication(string clientId)
    {
        _applications.Add(new OpenIddictLiteDBApplication()
        {
            ClientId = clientId
        });
        return this;
    }

    public ILiteDatabase Build()
    {
        var database = new OpenIddictLiteDatabase(":memory:");
        var options = new OpenIddictLiteDBOptions();

        database
            .GetCollection<OpenIddictLiteDBApplication>(options.ApplicationsCollectionName)
            .InsertBulk(_applications);

        return database;
    }
}
