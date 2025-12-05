
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bookify.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }


        public int BookingId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public string? StripeTransactionId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [Column(TypeName = "decimal(18,2)")] // تحديد Precision و Scale
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ValidateNever]
        public Booking Booking { get; set; } = null!;
    


       
    }
}