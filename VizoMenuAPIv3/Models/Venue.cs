using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class Venue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; } = null!;
        public ICollection<Site> Sites { get; set; } = new List<Site>();
    }
}
