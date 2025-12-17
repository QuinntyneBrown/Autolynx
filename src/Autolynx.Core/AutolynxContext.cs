// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Autolynx.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Autolynx.Core;

public class AutolynxContext : DbContext, IAutolynxContext
{
    public AutolynxContext(DbContextOptions<AutolynxContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Privilege> Privileges { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<InvitationToken> InvitationTokens { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between User and Role
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        // Configure many-to-many relationship between Profile and Conversation
        modelBuilder.Entity<Profile>()
            .HasMany(p => p.Conversations)
            .WithMany(c => c.Participants);

        // Configure Address as owned entity
        modelBuilder.Entity<Profile>()
            .OwnsOne(p => p.Address);

        // Configure Privilege-Role relationship
        modelBuilder.Entity<Privilege>()
            .HasOne<Role>()
            .WithMany(r => r.Privileges)
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Profile-User relationship
        modelBuilder.Entity<Profile>()
            .HasOne(p => p.User)
            .WithMany(u => u.Profiles)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Message-Profile relationship
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Profile)
            .WithMany(p => p.Messages)
            .HasForeignKey(m => m.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
