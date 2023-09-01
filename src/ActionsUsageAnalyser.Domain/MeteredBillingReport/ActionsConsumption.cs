namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class ActionsConsumption
{
    public Dictionary<string, decimal> MinutesPerSku { get; set; } = new();
    public Dictionary<string, decimal> PricePerRepository { get; set; } = new();
}