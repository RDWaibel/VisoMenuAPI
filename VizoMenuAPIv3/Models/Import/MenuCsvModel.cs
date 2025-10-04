using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models.Import
{
    public class MenuCsvModel
    {
        public string Id { get; set; } = string.Empty;
        public string SiteId { get; set; } = string.Empty;
        public string MenuName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public string AdditionalText1 { get; set; } = string.Empty;
        public string? AdditionalText2 { get; set; }
        public bool IsActive { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public DateTime EnteredUTC { get; set; }
        public DateTime? LastChangedUTC { get; set; }
        public string? LastChangedBy { get; set; }
        public string? ButtonImageId { get; set; }
    }

}
