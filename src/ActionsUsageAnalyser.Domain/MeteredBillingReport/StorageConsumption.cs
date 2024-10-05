namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class StorageConsumption
{
    public decimal AccumulatedPrice { get; set; } = new();
    public Dictionary<string, decimal> PricePerRepository { get; set; } = new();
}