using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class Venue
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool Is24Hours { get; set; }

        public DateTime EnteredUTC { get; set; } = DateTime.UtcNow;
        public Guid? EnteredById { get; set; }
        public DateTime? DisabledUTC { get; set; }
        public Guid? DisabledById { get; set; }

        public Organization? Organization { get; set; }
        public ICollection<VenueHour> Hours { get; set; } = new List<VenueHour>();
        public ICollection<Site> Sites { get; set; } = new List<Site>();
    }
}
