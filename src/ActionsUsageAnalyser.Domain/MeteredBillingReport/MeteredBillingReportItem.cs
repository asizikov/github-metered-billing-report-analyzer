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
}