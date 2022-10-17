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

public class DatabaseBuilder
{
    private readonly List<OpenIddictLiteDBApplication> _applications = new();
    private readonly List<OpenIddictLiteDBAuthorization> _authorizations = new();
    private readonly List<OpenIddictLiteDBToken> _tokens = new();
    private readonly ApplicationFaker _applicationFaker = new();
    private readonly AuthorizationFaker _authorizationFaker = new();
    private readonly TokenFaker _tokenFaker = new();

    public DatabaseBuilder WithApplication()
    {
        return WithApplications(1);
    }

    public DatabaseBuilder WithApplication(string clientId)
    {
        _applications.Add(_applicationFaker
            .RuleFor(x => x.ClientId, clientId)
            .Generate());
        return this;
    }

    public DatabaseBuilder WithApplication(
        OpenIddictLiteDBApplication application,
        bool includeAuthorizations = false,
        bool includeTokens = false)
    {
        _applications.Add(application);

        if (includeAuthorizations)
        {
            var authorizations = _authorizationFaker
                .RuleFor(x => x.ApplicationId, application.Id)
                .Generate(2);

            if (includeTokens)
            {
                foreach (var authorization in authorizations)
                {
                    _tokens.AddRange(_tokenFaker
                        .RuleFor(x => x.ApplicationId, application.Id)
                        .RuleFor(x => x.AuthorizationId, authorization.Id)
                        .Generate(2));
                }
            }

            _authorizations.AddRange(authorizations);
        }

        return this;
    }

    public DatabaseBuilder WithApplications(int count,
        bool includeAuthorizations = false,
        bool includeTokens = false)
    {
        var applications = _applicationFaker.Generate(count);

        if (includeAuthorizations)
        {
            foreach (var application in applications)
            {
                var authorizations = _authorizationFaker
                    .RuleFor(x => x.ApplicationId, application.Id)
                    .Generate(2);

                if (includeTokens)
                {
                    foreach (var authorization in authorizations)
                    {
                        _tokens.AddRange(_tokenFaker
                            .RuleFor(x => x.ApplicationId, application.Id)
                            .RuleFor(x => x.AuthorizationId, authorization.Id)
                            .Generate(2));
                    }
                }

                _authorizations.AddRange(authorizations);
            }
        }

        _applications.AddRange(applications);
        return this;
    }

    public ILiteDatabase Build()
    {
        var database = new OpenIddictLiteDatabase(":memory:");
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

        if (_tokens.Count > 0)
        {
            database
                .GetCollection<OpenIddictLiteDBToken>(options.TokensCollectionName)
                .InsertBulk(_tokens);
        }

        return database;
    }
}
