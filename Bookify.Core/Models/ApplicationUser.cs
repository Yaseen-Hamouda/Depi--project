using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<Booking> Bookings { get; set; }
    }
}
