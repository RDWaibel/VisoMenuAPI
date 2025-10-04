using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Models
{

        public class Image
        {
            [Key]
            public Guid ImageID { get; set; }

            [Required]
            public string Description { get; set; } = string.Empty;

            [Required]
            public string BlobContainer { get; set; } = string.Empty;

            [Required]
            public string ImagePath { get; set; } = string.Empty;

            [Required]
            public string FileName { get; set; } = string.Empty;

            // Navigation
            public ICollection<Item> Items { get; set; } = new List<Item>();
        }

    }
