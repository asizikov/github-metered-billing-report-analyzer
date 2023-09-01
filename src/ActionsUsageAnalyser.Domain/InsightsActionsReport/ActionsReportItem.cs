//Date;Month;Product;SKU;Quantity;Unit Type;Price Per Unit ($);Multiplier;Owner;Repository Slug;Username;Actions Workflow;;;;;;;
namespace ActionsUsageAnalyser.Domain.InsightsActionsReport;

public class ActionsReportItem
{
    public DateTime Date { get; set; }
    public string Month { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; } 
    public string UnitType { get; set; } = string.Empty;
    public string PricePerUnit { get; set; } = string.Empty;
    public double Multiplier { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string RepositorySlug { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    
    public static ActionsReportItem? FromCsv(string[] values)
    {
        if (values[2] != "Actions") return null;
        return new ActionsReportItem
        {
            Date = DateTime.Parse(values[0]),
            Month = values[1],
            Product = values[2],
            SKU = values[3],
            Quantity = int.Parse(values[4]),
            UnitType = values[5],
            PricePerUnit = values[6],
            Multiplier = double.Parse(values[7]),
            Owner = values[8],
            RepositorySlug = values[9],
            Username = values[10],
            WorkflowName = values[11]
        };
    }
}