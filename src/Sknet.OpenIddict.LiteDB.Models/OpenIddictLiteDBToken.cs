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
/// Represents an OpenIddict token.
/// </summary>
[DebuggerDisplay("Id = {Id.ToString(),nq} ; Subject = {Subject,nq} ; Type = {Type,nq} ; Status = {Status,nq}")]
public class OpenIddictLiteDBToken
{
    /// <summary>
    /// Gets or sets the identifier of the application associated with the current token.
    /// </summary>
    [BsonField("application_id")]
    public virtual ObjectId ApplicationId { get; set; } = ObjectId.Empty;

    /// <summary>
    /// Gets or sets the identifier of the authorization associated with the current token.
    /// </summary>
    [BsonField("authorization_id")]
    public virtual ObjectId AuthorizationId { get; set; } = ObjectId.Empty;

    /// <summary>
    /// Gets or sets the concurrency token.
    /// </summary>
    [BsonField("concurrency_token")]
    public virtual string? ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the UTC creation date of the current token.
    /// </summary>
    [BsonField("creation_date")]
    public virtual DateTimeOffset? CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the UTC expiration date of the current token.
    /// </summary>
    [BsonField("expiration_date")]
    public virtual DateTimeOffset? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier associated with the current token.
    /// </summary>
    [BsonId]
    public virtual ObjectId Id { get; set; } = ObjectId.Empty;

    /// <summary>
    /// Gets or sets the payload of the current token, if applicable.
    /// Note: this property is only used for reference tokens
    /// and may be encrypted for security reasons.
    /// </summary>
    [BsonField("payload")]
    public virtual string? Payload { get; set; }

    /// <summary>
    /// Gets or sets the additional properties associated with the current token.
    /// </summary>
    [BsonField("properties")]
    public virtual ImmutableDictionary<string, JsonElement>? Properties { get; set; }

    /// <summary>
    /// Gets or sets the UTC redemption date of the current token.
    /// </summary>
    [BsonField("redemption_date")]
    public virtual DateTimeOffset? RedemptionDate { get; set; }

    /// <summary>
    /// Gets or sets the reference identifier associated
    /// with the current token, if applicable.
    /// Note: this property is only used for reference tokens
    /// and may be hashed or encrypted for security reasons.
    /// </summary>
    [BsonField("reference_id")]
    public virtual string? ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the status of the current token.
    /// </summary>
    [BsonField("status")]
    public virtual string? Status { get; set; }

    /// <summary>
    /// Gets or sets the subject associated with the current token.
    /// </summary>
    [BsonField("subject")]
    public virtual string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the type of the current token.
    /// </summary>
    [BsonField("type")]
    public virtual string? Type { get; set; }
}