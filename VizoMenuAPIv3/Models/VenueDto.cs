using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class VenueDto
    {
        public Guid Id { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool Is24Hours { get; set; }
        public Guid OrganizationId { get; set; }
        public int SiteCount { get; set; }
    }
}
