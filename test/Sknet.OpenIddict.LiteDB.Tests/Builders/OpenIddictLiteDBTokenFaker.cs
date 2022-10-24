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

public class OpenIddictLiteDBTokenFaker : Faker<OpenIddictLiteDBToken>
{
    public OpenIddictLiteDBTokenFaker()
    {
        UseSeed(1);
        
        RuleFor(x => x.Id, f => new ObjectId(f.Random.Hexadecimal(24, prefix: "")))
            .RuleFor(x => x.ApplicationId, f => new ObjectId(f.Random.Hexadecimal(24, prefix: "")))
            .RuleFor(x => x.AuthorizationId, f => new ObjectId(f.Random.Hexadecimal(24, prefix: "")))
            .RuleFor(x => x.ConcurrencyToken, f => f.Random.Guid().ToString())
            .RuleFor(x => x.CreationDate, f => f.Date.PastOffset())
            .RuleFor(x => x.ExpirationDate, f => f.Date.FutureOffset())
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
            .RuleFor(x => x.ReferenceId, f => f.Random.Guid().ToString())
            .RuleFor(x => x.Status, f => f.PickRandom(new[]
                {
                    Statuses.Inactive,
                    Statuses.Redeemed,
                    Statuses.Rejected,
                    Statuses.Revoked,
                    Statuses.Valid
                }))
            .RuleFor(x => x.Subject, f => f.Person.UserName)
            .RuleFor(x => x.Type, f => f.PickRandom(new[] { TokenTypes.Bearer }));
    }
}