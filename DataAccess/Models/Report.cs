using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;
public partial class Report
{
    public int ReportId { get; set; }

    public int ReporterId { get; set; }

    public string Description { get; set; } = null!;

    public string PlateNumber { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public string Location { get; set; } = null!;

    public DateTime? ReportDate { get; set; }

    public string? Status { get; set; }

    public int? ProcessedBy { get; set; }

    public string? RejectionReason { get; set; }

    public decimal? FineAmount { get; set; }

    public int ResponseCount { get; set; }

    public int? ViolationTypeId { get; set; }

    public virtual User? ProcessedByNavigation { get; set; }

    public virtual User Reporter { get; set; } = null!;

    public virtual ViolationType? ViolationType { get; set; }   

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();

    [NotMapped]
    public bool NotificationSent { get; set; }
}
