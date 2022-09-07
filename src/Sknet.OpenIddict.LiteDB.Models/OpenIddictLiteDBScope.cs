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
namespace Sknet.OpenIddict.LiteDB.Models;

/// <summary>
/// Represents an OpenIddict scope.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; Name = {Name,nq}")]
public class OpenIddictLiteDBScope
{
    /// <summary>
    /// Gets or sets the concurrency token.
    /// </summary>
    [BsonField("concurrency_token")]
    public virtual string? ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the public description associated with the current scope.
    /// </summary>
    [BsonField("description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets or sets the localized public descriptions associated with the current scope.
    /// </summary>
    [BsonField("descriptions")]
    public virtual IReadOnlyDictionary<string, string>? Descriptions { get; set; }
        = ImmutableDictionary.Create<string, string>();

    /// <summary>
    /// Gets or sets the display name associated with the current scope.
    /// </summary>
    [BsonField("display_name")]
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the localized display names associated with the current scope.
    /// </summary>
    [BsonField("display_names")]
    public virtual IReadOnlyDictionary<string, string>? DisplayNames { get; set; }
        = ImmutableDictionary.Create<string, string>();

    /// <summary>
    /// Gets or sets the unique identifier associated with the current scope.
    /// </summary>
    [BsonId]
    public virtual ObjectId Id { get; set; } = ObjectId.Empty;

    /// <summary>
    /// Gets or sets the unique name associated with the current scope.
    /// </summary>
    [BsonField("name")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the additional properties associated with the current scope.
    /// </summary>
    [BsonField("properties")]
    public virtual IReadOnlyDictionary<string, JsonElement>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the resources associated with the current scope.
    /// </summary>
    [BsonField("resources")]
    public virtual IReadOnlyList<string>? Resources { get; set; } = ImmutableList.Create<string>();
}
