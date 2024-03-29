﻿/*
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
namespace Sknet.OpenIddict.LiteDB.Tests.Builders;

public class OpenIddictLiteDBScopeStoreBuilder
{
    private ILiteDatabase? _database;
    private readonly List<OpenIddictLiteDBScope> _scopes = new();

    public OpenIddictLiteDBScopeStoreBuilder WithDatabase(ILiteDatabase database)
    {
        _database = database;
        return this;
    }

    public OpenIddictLiteDBScopeStoreBuilder WithScopes(params OpenIddictLiteDBScope[] scopes)
    {
        _scopes.AddRange(scopes);
        return this;
    }

    public OpenIddictLiteDBScopeStoreBuilder WithScopes(IEnumerable<OpenIddictLiteDBScope> scopes)
    {
        _scopes.AddRange(scopes);
        return this;
    }

    public OpenIddictLiteDBScopeStore<OpenIddictLiteDBScope> Build()
    {
        var database = _database ?? new OpenIddictLiteDatabase(":memory:");
        var options = new OpenIddictLiteDBOptions();

        if (_scopes.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBScope>(options.ScopesCollectionName)
                .InsertBulk(_scopes);
        }

        var contextMock = new Mock<IOpenIddictLiteDBContext>();
        contextMock
            .Setup(x => x.GetDatabaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(database);

        var optionsMock = new Mock<IOptionsMonitor<OpenIddictLiteDBOptions>>();
        optionsMock
            .SetupGet(x => x.CurrentValue)
            .Returns(options);

        return new OpenIddictLiteDBScopeStore<OpenIddictLiteDBScope>(contextMock.Object, optionsMock.Object);
    }
}
