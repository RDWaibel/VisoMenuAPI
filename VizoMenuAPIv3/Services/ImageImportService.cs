using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Models.Import;

namespace VizoMenuAPIv3.Services
{
    public class ImageImportService
    {
        private readonly VizoMenuDbContext _db;

        public ImageImportService(VizoMenuDbContext db)
        {
            _db = db;
        }

        public async Task<List<Image>> ImportImagesFromCsvAsync(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });

            var records = csv.GetRecords<ImportImageDTO>().ToList();

            var images = records.Select(r => new Image
            {
                ImageID = r.ImageID,
                Description = r.Description,
                BlobContainer = r.BlobContainer,
                ImagePath = r.ImagePath,
                FileName = r.FileName
            }).ToList();

            _db.Images.AddRange(images);
            await _db.SaveChangesAsync();
            return images;
        }
    }
}
