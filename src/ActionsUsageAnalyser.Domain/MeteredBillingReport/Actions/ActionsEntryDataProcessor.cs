namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;

public class ActionsEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Actions) return;

        enterprise.ActionsConsumptionPerOwner.TryAdd(item.Owner, new ());
        enterprise.ActionsConsumptionPerOwner[item.Owner].MinutesPerSku.TryAdd(item.SKU, 0);
        enterprise.ActionsConsumptionPerOwner[item.Owner].MinutesPerSku[item.SKU] += item.Quantity;
        enterprise.ActionsConsumptionPerOwner[item.Owner].CostPerRepository.TryAdd(item.RepositorySlug, 0);
        enterprise.ActionsConsumptionPerOwner[item.Owner].CostPerRepository[item.RepositorySlug] += item.Quantity * item.PricePerUnit;
    }
}