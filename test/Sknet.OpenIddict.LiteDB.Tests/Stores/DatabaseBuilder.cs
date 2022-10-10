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
namespace Sknet.OpenIddict.LiteDB.Tests;

public class DatabaseBuilder
{
    private readonly List<OpenIddictLiteDBApplication> _applications = new();

    public DatabaseBuilder WithApplication(OpenIddictLiteDBApplication application)
    {
        _applications.Add(application);
        return this;
    }

    /*public DatabaseBuilder WithApplications(int count)
    {
        var applications = new Faker<OpenIddictLiteDBApplication>()
            .RuleFor(x => x.ClientId, f => f.Internet.UserName())
            .RuleFor(x => x.ClientSecret, f => f.Internet.Password())
            .RuleFor(x => x.ConcurrencyToken, f => f.Random.Guid().ToString())
            .RuleFor(x => x.DisplayName, f => f.Commerce.ProductName())
            .RuleFor(x => x.Permissions, f => f.PickRandom(new[] { "scope1", "scope2", "scope3" }, f.Random.Int(0, 3)).ToImmutableArray())
            .RuleFor(x => x.PostLogoutRedirectUris, f => f.PickRandom(new[] { "https://example.com" }, f.Random.Int(0, 1)).ToImmutableArray())
            .RuleFor(x => x.Properties, f => f.PickRandom(new[]
            { 
                ImmutableDictionary<string, JsonElement>.Empty,
                ImmutableDictionary.CreateRange(new[] 
                { 
                    new KeyValuePair<string, JsonElement>("key", JsonSerializer.Deserialize<JsonElement>("\"value\""))
                })
            }))
            .RuleFor(x => x.RedirectUris, f => f.PickRandom(new[] 
            { 
                ImmutableArray<string>.Empty, 
                ImmutableArray.Create("https://example.com")
            }))
            .RuleFor(x => x.Type, f => f.PickRandom(new[] 
            { 
                OpenIddictConstants.ClientTypes.Confidential.ToString(),
                OpenIddictConstants.ClientTypes.Public.ToString()
            }));

        _applications.AddRange(applications.Generate(count));
        return this;
    }*/
    
    public DatabaseBuilder WithApplication(string clientId)
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithLocale("en_US");
        });

        _applications.Add(new AutoFaker<OpenIddictLiteDBApplication>()
            //.RuleFor(x => x.ClientId, clientId)
            .Generate());
            
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
