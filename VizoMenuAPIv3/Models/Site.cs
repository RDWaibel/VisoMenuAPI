using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class Site
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid EnteredById { get; set; }
        public DateTime EnteredUTC { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Venue Venue { get; set; } = null!;
    }
}
