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

public class OpenIddictLiteDBApplicationStoreBuilder
{
    private readonly List<OpenIddictLiteDBApplication> _applications = new();
    private readonly List<OpenIddictLiteDBAuthorization> _authorizations = new();
    private readonly List<OpenIddictLiteDBScope> _scopes = new();
    private readonly List<OpenIddictLiteDBToken> _tokens = new();
    private ILiteDatabase? _database;

    public OpenIddictLiteDBApplicationStoreBuilder WithApplications(params OpenIddictLiteDBApplication[] applications)
    {
        _applications.AddRange(applications);
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithApplications(IEnumerable<OpenIddictLiteDBApplication> applications)
    {
        _applications.AddRange(applications);
        return this;
    }
    
    public OpenIddictLiteDBApplicationStoreBuilder WithAuthorizations(params OpenIddictLiteDBAuthorization[] authorizations)
    {
        _authorizations.AddRange(authorizations);
        return this;
    }
    
    public OpenIddictLiteDBApplicationStoreBuilder WithAuthorizations(IEnumerable<OpenIddictLiteDBAuthorization> authorizations)
    {
        _authorizations.AddRange(authorizations);
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithDatabase(ILiteDatabase database)
    {
        _database = database;
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithScopes(params OpenIddictLiteDBScope[] scopes)
    {
        _scopes.AddRange(scopes);
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithScopes(IEnumerable<OpenIddictLiteDBScope> scopes)
    {
        _scopes.AddRange(scopes);
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithTokens(params OpenIddictLiteDBToken[] tokens)
    {
        _tokens.AddRange(tokens);
        return this;
    }

    public OpenIddictLiteDBApplicationStoreBuilder WithTokens(IEnumerable<OpenIddictLiteDBToken> tokens)
    {
        _tokens.AddRange(tokens);
        return this;
    }


    public OpenIddictLiteDBApplicationStore<OpenIddictLiteDBApplication> Build()
    {
        var database = _database ?? new OpenIddictLiteDatabase(":memory:");
        var options = new OpenIddictLiteDBOptions();

        if (_applications.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBApplication>(options.ApplicationsCollectionName)
                .InsertBulk(_applications);
        }

        if (_authorizations.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBAuthorization>(options.AuthorizationsCollectionName)
                .InsertBulk(_authorizations);
        }

        if (_scopes.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBScope>(options.ScopesCollectionName)
                .InsertBulk(_scopes);
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

        return new OpenIddictLiteDBApplicationStore<OpenIddictLiteDBApplication>(contextMock.Object, optionsMock.Object);
    }
}
