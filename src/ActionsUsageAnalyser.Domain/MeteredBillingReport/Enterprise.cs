namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class Enterprise
{
    public Dictionary<string, ActionsConsumption> ActionsConsumptionPerOwner { get; set; } = new();
}