// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class Profile
{
    public Guid ProfileId { get; set; }
    public Guid? UserId { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public Guid? AvatarDigitalAssetId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public Address? Address { get; set; }
    public User? User { get; set; }
    public ProfileType Type { get; set; }
    public List<Message> Messages { get; set; } = new();
    public List<Conversation> Conversations { get; set; } = new();
}
