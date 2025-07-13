using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class Organization
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;

        public DateTime EnteredUTC { get; set; }
        public string EnteredBy { get; set; } = default!;
        public ICollection<Venue> Venues { get; set; } = new List<Venue>();
    }
}
