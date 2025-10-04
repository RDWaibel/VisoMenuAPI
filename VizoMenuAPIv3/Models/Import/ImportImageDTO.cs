using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace VizoMenuAPIv3.Models.Import
{
    public class ImportImageDTO
    {
        [Name("ImageID")]
        public Guid ImageID { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        [Name("BlobContainer")]
        public string BlobContainer { get; set; }

        [Name("ImagePath")]
        public string ImagePath { get; set; }

        [Name("FileName")]
        public string FileName { get; set; }
    }
}
