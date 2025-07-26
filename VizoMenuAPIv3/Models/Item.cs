using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{
        public class Item
        {
            [Key]
            public Guid Id { get; set; }

            [Required]
            public string BaseName { get; set; } = string.Empty;

            public string? Category { get; set; } = string.Empty;

            public Guid? ImageID { get; set; }

            // Navigation
            public Image? Image { get; set; }
        }

    }
