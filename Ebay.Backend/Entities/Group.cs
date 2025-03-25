using System;
using System.Collections.Generic;

namespace Ebay.Backend.Entities;

public partial class Group
{
    public int Id { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }

    public virtual ICollection<RoleGroup> RoleGroups { get; set; } = new List<RoleGroup>();

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
