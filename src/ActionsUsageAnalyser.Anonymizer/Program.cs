using System.Globalization;
using ActionsUsageAnalyser.Domain.Meta;
using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using ActionsUsageAnalyzer.Infrastructure;

var inputDirectory = Environment.GetEnvironmentVariable("INPUT_DIRECTORY") ?? @"/input/";
var outputDirectory = Environment.GetEnvironmentVariable("OUTPUT_DIRECTORY") ?? @"/output/";
var fileName = args[1];

var filePath = Path.Combine(inputDirectory, fileName);

if (!File.Exists(filePath))
{
    Console.WriteLine("Input file does not exist");
    return;
}

var reader = new MeteredBillingReportReader(new FileContentStreamer(), new MeteredBillingReportItemParser());

await using var csvWriter = File.Open(Path.Combine(outputDirectory, "processed.csv"), FileMode.Create, FileAccess.Write, FileShare.Read);
await using var writer = new StreamWriter(csvWriter);

writer.WriteLine("Date,Product,SKU,Quantity,Unit Type,Price Per Unit ($),Multiplier,Owner,Repository Slug,Username,Actions Workflow,Notes");

await foreach(var reportItem in reader.ReadFromSourceAsync(filePath))
{
    ProcessReportItem(reportItem);
    
    await writer.WriteLineAsync($"{reportItem.Date:yyyy-MM-dd},{reportItem.Product},{reportItem.SKU},{reportItem.Quantity},{reportItem.UnitType},{reportItem.PricePerUnit.ToString(CultureInfo.InvariantCulture)},{reportItem.Multiplier.ToString(CultureInfo.InvariantCulture)},{reportItem.Owner},{reportItem.RepositorySlug},{reportItem.Username},{reportItem.ActionsWorkflow},{reportItem.Notes}");

    void ProcessReportItem(MeteredBillingReportItem item)
    {
        // enumerate all properties and find those with SensitiveDataAttribute
        var sensitiveProperties = typeof(MeteredBillingReportItem).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(SensitiveDataAttribute), true).Any())
            .ToList();
        
        // enumerate all sensitive properties and replace their values with a hash
        foreach(var property in sensitiveProperties)
        {
            // check value of DataType property of SensitiveDataAttribute
            var dataType = ((SensitiveDataAttribute)property.GetCustomAttributes(typeof(SensitiveDataAttribute), true).First()).DataType;
            var value = property.GetValue(item);
            if (value is string stringValue)
            {
                property.SetValue(item, BogusValue(stringValue, dataType ));
            }
        }
    }
    
    string BogusValue(string str, DataType dataType)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        var seed = str.GetHashCode();
        var random = new Random(seed);
        return dataType switch
        {
            DataType.Owner => $"owner-{random.Next(0,1000)}",
            DataType.Repository => $"repo-{random.Next(0,1000)}",
            DataType.Username => $"user-{random.Next(0,1000)}",
            DataType.Workflow => $".github/workflows/workflow-{random.Next(0, 1000)}.yml",
            _ => str
        };
    }
}

