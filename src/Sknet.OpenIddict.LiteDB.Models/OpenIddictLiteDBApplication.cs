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
/// Represents an OpenIddict application.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; ClientId = {ClientId,nq} ; Type = {Type,nq}")]
public class OpenIddictLiteDBApplication
{
    /// <summary>
    /// Gets or sets the client identifier associated with the current application.
    /// </summary>
    [BsonField("client_id")]
    public virtual string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret associated with the current application.
    /// Note: depending on the application manager used to create this instance,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    [BsonField("client_secret")]
    public virtual string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the concurrency token.
    /// </summary>
    [BsonField("concurrency_token")]
    public virtual string? ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the consent type associated with the current application.
    /// </summary>
    [BsonField("consent_type")]
    public virtual string? ConsentType { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the current application.
    /// </summary>
    [BsonField("display_name")]
    public virtual string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the localized display names associated with the current application.
    /// </summary>
    [BsonField("display_names")]
    public virtual IReadOnlyDictionary<string, string>? DisplayNames { get; set; }
        = ImmutableDictionary.Create<string, string>();

    /// <summary>
    /// Gets or sets the unique identifier associated with the current application.
    /// </summary>
    [BsonId]
    public virtual ObjectId Id { get; set; } = ObjectId.Empty;

    /// <summary>
    /// Gets or sets the permissions associated with the current application.
    /// </summary>
    [BsonField("permissions")]
    public virtual IReadOnlyList<string>? Permissions { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the logout callback URLs associated with the current application.
    /// </summary>
    [BsonField("post_logout_redirect_uris")]
    public virtual IReadOnlyList<string>? PostLogoutRedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the additional properties associated with the current application.
    /// </summary>
    [BsonField("properties")]
    public virtual IReadOnlyDictionary<string, JsonElement>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the callback URLs associated with the current application.
    /// </summary>
    [BsonField("redirect_uris")]
    public virtual IReadOnlyList<string>? RedirectUris { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the requirements associated with the current application.
    /// </summary>
    [BsonField("requirements")]
    public virtual IReadOnlyList<string>? Requirements { get; set; } = ImmutableList.Create<string>();

    /// <summary>
    /// Gets or sets the application type
    /// associated with the current application.
    /// </summary>
    [BsonField("type")]
    public virtual string? Type { get; set; }
}
