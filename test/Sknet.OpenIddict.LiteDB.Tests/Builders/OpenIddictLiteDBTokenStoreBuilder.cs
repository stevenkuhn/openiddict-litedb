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
namespace Sknet.OpenIddict.LiteDB.Tests.Builders;

public class OpenIddictLiteDBTokenStoreBuilder
{
    private ILiteDatabase? _database;
    private readonly List<OpenIddictLiteDBAuthorization> _authorizations = new();
    private readonly List<OpenIddictLiteDBToken> _tokens = new();

    public OpenIddictLiteDBTokenStoreBuilder WithAuthorizations(IEnumerable<OpenIddictLiteDBAuthorization> authorizations)
    {
        _authorizations.AddRange(authorizations);
        return this;
    }
    
    public OpenIddictLiteDBTokenStoreBuilder WithDatabase(ILiteDatabase database)
    {
        _database = database;
        return this;
    }

    public OpenIddictLiteDBTokenStoreBuilder WithTokens(params OpenIddictLiteDBToken[] tokens)
    {
        _tokens.AddRange(tokens);
        return this;
    }

    public OpenIddictLiteDBTokenStoreBuilder WithTokens(IEnumerable<OpenIddictLiteDBToken> tokens)
    {
        _tokens.AddRange(tokens);
        return this;
    }

    public OpenIddictLiteDBTokenStore<OpenIddictLiteDBToken> Build()
    {
        var database = _database ?? new OpenIddictLiteDatabase(":memory:");
        var options = new OpenIddictLiteDBOptions();

        if (_authorizations.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBAuthorization>(options.AuthorizationsCollectionName)
                .InsertBulk(_authorizations);
        }
        
        if (_tokens.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBToken>(options.TokensCollectionName)
                .InsertBulk(_tokens);
        }

        var contextMock = new Mock<IOpenIddictLiteDBContext>();
        contextMock
            .Setup(x => x.GetDatabaseAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(database);
        
        var optionsMock = new Mock<IOptionsMonitor<OpenIddictLiteDBOptions>>();
        optionsMock
            .SetupGet(x => x.CurrentValue)
            .Returns(options);

        return new OpenIddictLiteDBTokenStore<OpenIddictLiteDBToken>(contextMock.Object, optionsMock.Object);
    }
}
