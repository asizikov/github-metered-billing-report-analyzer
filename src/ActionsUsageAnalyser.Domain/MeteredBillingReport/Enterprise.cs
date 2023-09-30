namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class Enterprise
{
    public Dictionary<string, ActionsConsumption> ActionsConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, CopilotConsumption> CopilotConsumptionPerOwner { get; set; } = new();
}