namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Codespaces;

public class CodespacesConsumption
{
    public decimal AccumulatedCost { get; set; }
    public Dictionary<string, decimal> CostPerRepository { get; set; } = new();
    public Dictionary<string, (string unit, decimal cost)> ConsumptionPerSku { get; set; } = new();
}