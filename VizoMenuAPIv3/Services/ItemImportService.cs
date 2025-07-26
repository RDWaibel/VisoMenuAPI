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

        csv.Read();
        csv.ReadHeader();
        Console.WriteLine("🧠 Header Record:");
        foreach (var header in csv.HeaderRecord)
        {
            Console.WriteLine($"🔹 '{header}'");
        }

        var records = csv.GetRecords<ImportItemDTO>();
        foreach (var dto in records)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error mapping record: {ex.Message}");
                Console.WriteLine($"Raw Record: BaseName='{dto.BaseName}', Category='{dto.Category}', ImageId='{dto.ImageID}'");
            }
        }
        try
        {
            await _db.SaveChangesAsync();
            Console.WriteLine($"✅ Saved {records.Count()} items.");
            return importedItems;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 EF Save Error: {ex.Message}");
            throw;
        }
    }
}
