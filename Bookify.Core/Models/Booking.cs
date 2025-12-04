using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Core.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]  // تحديد Precision و Scale
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending";  // Pending, Paid, Failed, Refunded

        [StringLength(50)]
        public string Status { get; set; } = "Pending";  // Pending, Confirmed, Cancelled, Completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation

        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;

        [ValidateNever]
        public Room Room { get; set; } = null!;

        [ValidateNever]
        public ICollection<Payment> Payments { get; set; } = new List<Payment>(); // One-to-Many
    }
}