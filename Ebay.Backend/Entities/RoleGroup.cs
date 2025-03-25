using System;
using System.Collections.Generic;

namespace Ebay.Backend.Entities;

public partial class RoleGroup
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int GroupId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
