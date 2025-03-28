using System;
using System.Collections.Generic;

namespace DataAccess.Models;
public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public decimal? Balance { get; set; }

    public string? Address { get; set; }

    public bool? IsDisabled { get; set; } = false;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Report> ReportProcessedByNavigations { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportReporters { get; set; } = new List<Report>();

    public virtual ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
