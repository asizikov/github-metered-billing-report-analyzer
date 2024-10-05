namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

public class StorageConsumption
{
    public decimal AccumulatedCost { get; set; } = new();
    public Dictionary<string, decimal> CostPerRepository { get; set; } = new();
}