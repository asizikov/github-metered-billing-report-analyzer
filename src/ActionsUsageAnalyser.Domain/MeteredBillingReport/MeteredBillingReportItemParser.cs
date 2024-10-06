using System.Globalization;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class MeteredBillingReportItemParser : IReportItemParser<MeteredBillingReportItem>
{
    public MeteredBillingReportItem Parse(string[] values)
    {
        try
        {
            return new MeteredBillingReportItem
            {
                Date = DateTime.Parse(values[0]),
                Product = SafeParse(values[1]),
                SKU = values[2],
                Quantity = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                UnitType = values[4],
                PricePerUnit = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                Multiplier = decimal.Parse(values[6], CultureInfo.InvariantCulture),
                Owner = values[7],
                RepositorySlug = values[8],
                Username = values[9],
                ActionsWorkflow = values[10],
                Notes = values[11]
            };
        }
        catch (Exception)
        {
            Console.WriteLine($"Error parsing report item: {string.Join(", ", values)}");
            throw;
        }

        static Product SafeParse(string value)
        {
            var sanitizedValue = value.Replace(" ", string.Empty);
            sanitizedValue = sanitizedValue.Replace("-", string.Empty);
            return Enum.TryParse<Product>(sanitizedValue, out var product) ? product : Product.Unknown;
        }
    }
}