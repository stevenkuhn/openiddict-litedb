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
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Sknet.OpenIddict.LiteDB.Tests;

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
            .RuleFor(x => x.DisplayNames, f =>
            {
                return ImmutableDictionary.Create<CultureInfo, string>();
            })
            .RuleFor(x => x.Permissions, f => f.Random.ListItems<string>(new() { "scope1", "scope2", "scope3" }).ToImmutableArray())
            .RuleFor(x => x.PostLogoutRedirectUris, f => ImmutableArray.Create<string>())
            .RuleFor(x => x.Properties, f => ImmutableDictionary.Create<string, JsonElement>())
            .RuleFor(x => x.RedirectUris, f => ImmutableArray.Create<string>())
            .RuleFor(x => x.Requirements, f => ImmutableArray.Create<string>())
            .RuleFor(x => x.Type, f => f.PickRandom(new[] { ClientTypes.Confidential, ClientTypes.Public }));
    }
}
