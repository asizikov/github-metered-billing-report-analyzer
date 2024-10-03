namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class Enterprise
{
    public Dictionary<string, ActionsConsumption> ActionsConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, CopilotConsumption> CopilotConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, StorageConsumption> StorageConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, PackagesConsumption> PackagesConsumptionPerOwner { get; set; } = new();
}