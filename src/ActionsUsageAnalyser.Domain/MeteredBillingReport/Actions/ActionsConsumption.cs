namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;

public class ActionsConsumption
{
    public Dictionary<string, decimal> MinutesPerSku { get; set; } = new();
    public Dictionary<string, decimal> CostPerRepository { get; set; } = new();
}