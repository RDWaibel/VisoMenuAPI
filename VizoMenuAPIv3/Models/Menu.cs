using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
    public class Menu
    {
        public Guid Id { get; set; } // Menu ID
        public Guid SiteId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public int SortOrder { get; set; }

        public string DisplayText { get; set; }
        public string AdditionalText1 { get; set; }
        public string? AdditionalText2 { get; set; }

        public bool IsActive { get; set; } = true;
        public string EnteredBy { get; set; } = string.Empty;
        public DateTime EnteredUTC { get; set; } = DateTime.UtcNow;
        public DateTime? LastChangedUTC { get; set; }
        public string? LastChangedBy { get; set; }

        public Guid? ButtonImageId { get; set; }

        // Navigation
        public Site Site { get; set; } = null!;
    }

}
