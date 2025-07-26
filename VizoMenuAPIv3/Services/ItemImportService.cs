using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Models.Import;

public class ItemImportService
{
    private readonly VizoMenuDbContext _db;

    public ItemImportService(VizoMenuDbContext db)
    {
        _db = db;
    }

    public async Task<List<Item>> ImportItemsFromCsvAsync(Stream csvStream)
    {
        var importedItems = new List<Item>();

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<ImportItemDTO>();
        foreach (var dto in records)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                BaseName = dto.BaseName,
                Category = dto.Category,
                ImageID = dto.ImageID
            };

            importedItems.Add(item);
            _db.Items.Add(item);
        }

        await _db.SaveChangesAsync();
        return importedItems;
    }
}
