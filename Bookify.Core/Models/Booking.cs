using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Core.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string PaymentStatus { get; set; }  // Pending, Paid, Failed

        // Navigation

        [ValidateNever]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        public Room Room { get; set; }

        [ValidateNever]
        public Payment Payment { get; set; }
    }
}
