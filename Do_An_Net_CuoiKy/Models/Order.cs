using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Do_An_Net_CuoiKy.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled

        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [StringLength(100)]
        public string? ShippingName { get; set; }

        [StringLength(20)]
        public string? ShippingPhone { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
