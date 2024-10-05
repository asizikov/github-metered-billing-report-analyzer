namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Packages;

public class PackagesConsumption
{
    public decimal AccumulatedCost { get; set; } = new();
    public Dictionary<string, decimal> CostPerRepository { get; set; } = new();
}