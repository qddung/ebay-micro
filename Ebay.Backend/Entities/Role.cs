using System;
using System.Collections.Generic;

namespace Ebay.Backend.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }

    public virtual ICollection<RoleGroup> RoleGroups { get; set; } = new List<RoleGroup>();
}
