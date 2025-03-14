using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public partial class Payment
    {
        public int PaymentID { get; set; }

        public int UserID { get; set; }

        public int ViolationID { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public virtual User? User { get; set; }

        public virtual Violation? Violation { get; set; }
    }
}
