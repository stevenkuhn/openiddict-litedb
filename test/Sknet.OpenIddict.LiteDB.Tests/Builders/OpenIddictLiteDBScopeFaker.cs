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

public class OpenIddictLiteDBScopeFaker : Faker<OpenIddictLiteDBScope>
{
    public OpenIddictLiteDBScopeFaker()
    {
        UseSeed(1);

        RuleFor(x => x.Id, f => new ObjectId(f.Random.Hexadecimal(24, prefix: "")))
            .RuleFor(x => x.ConcurrencyToken, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.Descriptions, f => f.Random.ListItems<(CultureInfo Culture, string Description)>(new()
                {
                    (CultureInfo.GetCultureInfo("en"), f.Lorem.Sentence()),
                    (CultureInfo.GetCultureInfo("fr"), f.Lorem.Sentence()),
                    (CultureInfo.GetCultureInfo("de"), f.Lorem.Sentence()),
                    (CultureInfo.GetCultureInfo("es"), f.Lorem.Sentence())
                })
                .OrderBy(x => x.Culture.Name)
                .ToImmutableDictionary(x => x.Culture, x => x.Description))
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
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
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
            .RuleFor(x => x.Resources, f => f.Random.ListItems<string>(new()
                {
                    "resource1",
                    "resource2",
                    "resource3",
                    "resource4",
                    "resource5"
                })
                .ToImmutableArray());
    }
}
