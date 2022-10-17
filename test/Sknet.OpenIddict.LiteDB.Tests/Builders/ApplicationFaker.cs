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

public class ApplicationFaker : Faker<OpenIddictLiteDBApplication>
{
    public ApplicationFaker()
    {
        RuleFor(x => x.Id, f => new ObjectId(f.Random.Hexadecimal(24, prefix: "")))
            .RuleFor(x => x.ClientId, f => f.Random.AlphaNumeric(32))
            .RuleFor(x => x.ClientSecret, f => f.Random.AlphaNumeric(32))
            .RuleFor(x => x.ConcurrencyToken, f => f.Random.Guid().ToString())
            .RuleFor(x => x.ConsentType, f => f.PickRandom(new[] { ConsentTypes.Explicit, ConsentTypes.External, ConsentTypes.Implicit, ConsentTypes.Systematic }))
            .RuleFor(x => x.DisplayName, f => f.Commerce.ProductName())
            .RuleFor(x => x.DisplayNames, f => f.Random.ListItems<(CultureInfo Culture, string DisplayName)>(new()
                {
                    (CultureInfo.GetCultureInfo("en"), f.Commerce.ProductName()),
                    (CultureInfo.GetCultureInfo("fr"), f.Commerce.ProductName()),
                    (CultureInfo.GetCultureInfo("de"), f.Commerce.ProductName()),
                    (CultureInfo.GetCultureInfo("es"), f.Commerce.ProductName())
                })
                .OrderBy(x => x.Culture.Name)
                .ToImmutableDictionary(x => x.Culture, x => x.DisplayName))
            .RuleFor(x => x.Permissions, f => f.Random.ListItems<string>(new() { "scope1", "scope2", "scope3" })
                .ToImmutableArray())
            .RuleFor(x => x.PostLogoutRedirectUris, f => f.Random.ListItems<string>(new()
                {
                    f.Internet.UrlWithPath(),
                    f.Internet.UrlWithPath(),
                    f.Internet.UrlWithPath()
                })
                .ToImmutableArray())
            .RuleFor(x => x.Properties, f => f.Random.ListItems<(string PropertyName, JsonElement JsonValue)>(new()
                {
                    ("property1", JsonDocument.Parse("true").RootElement),
                    ("property2", JsonDocument.Parse("false").RootElement),
                    ("property3", JsonDocument.Parse("null").RootElement),
                    ("property4", JsonDocument.Parse("1").RootElement),
                    ("property5", JsonDocument.Parse("1.1").RootElement),
                    ("property6", JsonDocument.Parse(@"""""").RootElement),
                    ("property7", JsonDocument.Parse(@"""value""").RootElement),
                    ("property8", JsonDocument.Parse(@"[""value1"", ""value2""]").RootElement),
                    ("property9", JsonDocument.Parse(@"{""key"": ""value""}").RootElement)
                })
                .OrderBy(x => x.PropertyName)
                .ToImmutableDictionary(x => x.PropertyName, x => x.JsonValue))
            .RuleFor(x => x.RedirectUris, f => f.Random.ListItems<string>(new()
                {
                    f.Internet.UrlWithPath(),
                    f.Internet.UrlWithPath(),
                    f.Internet.UrlWithPath()
                })
                .ToImmutableArray())
            .RuleFor(x => x.Requirements, f => ImmutableArray.Create<string>())
            .RuleFor(x => x.Type, f => f.PickRandom(new[] { ClientTypes.Confidential, ClientTypes.Public }));

        UseSeed(39571);
    }
}
