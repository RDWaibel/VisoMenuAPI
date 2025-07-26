using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models.Import
{
    public class ImportItemDTO
    {
        public string BaseName { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        public Guid? ImageID { get; set; }
    }
}
