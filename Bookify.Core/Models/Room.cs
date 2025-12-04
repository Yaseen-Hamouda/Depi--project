using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Core.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
        public string Status { get; set; }  // Available, Booked, Maintenance

        public string? ImageUrl { get; set; }

        // Navigation
        [ValidateNever]
        public RoomType RoomType { get; set; }

        [ValidateNever]
        public ICollection<Booking> Bookings { get; set; }
    }
}
