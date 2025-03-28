﻿using System;
using System.Collections.Generic;

namespace DataAccess.Models;
public partial class Violation
{
    public int ViolationId { get; set; }

    public int ReportId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public int? ViolatorId { get; set; }

    public DateTime? FineDate { get; set; }

    public decimal? FineAmount { get; set; }

    public bool PaidStatus { get; set; }

    public string Response { get; set; }

    public int? ResponseCount { get; set; }

    public int? ViolationTypeId { get; set; }

    public bool IsResponseRejected { get; set; }

    public virtual Vehicle PlateNumberNavigation { get; set; } = null!;

    public virtual Report Report { get; set; } = null!;

    public virtual ViolationType? ViolationType { get; set; }

    public virtual User? Violator { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
