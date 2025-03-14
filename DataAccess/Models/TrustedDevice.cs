using System;
using System.Collections.Generic;

namespace DataAccess.Models;
public partial class TrustedDevice
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string DeviceToken { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
