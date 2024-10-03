namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class PackagesConsumption
{
    public decimal AccumulatedCost { get; set; } = new();
    public Dictionary<string, decimal> PricePerRepository { get; set; } = new();
}