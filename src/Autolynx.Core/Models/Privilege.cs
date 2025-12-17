// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class Privilege
{
    public Guid PrivilegeId { get; set; }
    public Guid RoleId { get; set; }
    public AccessRight AccessRight { get; set; }
    public string Aggregate { get; set; } = string.Empty;
}
