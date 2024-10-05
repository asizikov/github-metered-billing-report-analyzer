namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class CodespacesConsumption
{
    public decimal AccumulatedCost { get; set; }
    public Dictionary<string, decimal> PricePerRepository { get; set; } = new();
    public Dictionary<string, (string unit, decimal cost)> ConsumptionPerSku { get; set; } = new();
}