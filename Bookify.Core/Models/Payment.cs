using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public string StripeTransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        [ValidateNever]
        public Booking Booking { get; set; }
    }
}
