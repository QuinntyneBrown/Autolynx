// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class InvitationToken
{
    public Guid InvitationTokenId { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateTime? Expiry { get; set; }
    public InvitationTokenType Type { get; set; }
}
