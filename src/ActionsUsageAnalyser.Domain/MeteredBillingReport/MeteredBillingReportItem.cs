using System.Globalization;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class MeteredBillingReportItem
{
    public DateTime Date { get; set; }
    public string Product { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitType { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public decimal Multiplier { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string RepositorySlug { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ActionsWorkflow { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public static MeteredBillingReportItem FromCsv(string[] values)
    {
        return new MeteredBillingReportItem
        {
            Date = DateTime.Parse(values[0]),
            Product = values[1],
            SKU = values[2],
            Quantity = decimal.Parse(values[3]),
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
}