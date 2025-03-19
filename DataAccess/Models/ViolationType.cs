using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ViolationType
{
    public int ViolationTypeId { get; set; }

    public string ViolationName { get; set; } = null!;

    public decimal FineAmount { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
