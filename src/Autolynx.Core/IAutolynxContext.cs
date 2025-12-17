// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Autolynx.Core;

public interface IAutolynxContext
{
    DbSet<User> Users { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<Privilege> Privileges { get; set; }
    DbSet<Profile> Profiles { get; set; }
    DbSet<InvitationToken> InvitationTokens { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<Conversation> Conversations { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
