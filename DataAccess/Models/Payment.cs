using System;
using System.Collections.Generic;

namespace DataAccess.Models;
public partial class Payment
{
    public int PaymentId { get; set; }

    public int UserId { get; set; }

    public int ViolationId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Violation Violation { get; set; } = null!;
}
