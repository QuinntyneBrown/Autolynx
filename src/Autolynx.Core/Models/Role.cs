// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class Role
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new();
    public List<Privilege> Privileges { get; set; } = new();
}
