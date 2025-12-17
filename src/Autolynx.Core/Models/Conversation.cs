// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class Conversation
{
    public Guid ConversationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Profile> Participants { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}
