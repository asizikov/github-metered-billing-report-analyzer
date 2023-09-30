namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class CopilotEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Copilot) return;

        var owner = item.Owner;
        var price = item.PricePerUnit * item.Quantity * item.Multiplier;

        enterprise.CopilotConsumptionPerOwner.TryAdd(owner, new());

        var copilotConsumption = enterprise.CopilotConsumptionPerOwner[owner];
        copilotConsumption.AccumulatedPrise += price;
    }
}