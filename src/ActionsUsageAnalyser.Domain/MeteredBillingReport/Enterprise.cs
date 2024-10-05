using ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Codespaces;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Copilot;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Packages;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.SharedStorage;

namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class Enterprise
{
    public Dictionary<string, ActionsConsumption> ActionsConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, CopilotConsumption> CopilotConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, StorageConsumption> StorageConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, PackagesConsumption> PackagesConsumptionPerOwner { get; set; } = new();
    public Dictionary<string, CodespacesConsumption> CodespacesConsumptionPerOwner { get; set; } = new();
}