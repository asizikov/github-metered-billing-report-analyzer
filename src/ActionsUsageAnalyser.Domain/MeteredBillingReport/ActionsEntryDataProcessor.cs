namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class ActionsEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Actions) return;

        enterprise.ActionsConsumptionPerOwner.TryAdd(item.Owner, new ActionsConsumption());
        enterprise.ActionsConsumptionPerOwner[item.Owner].MinutesPerSku.TryAdd(item.SKU, 0);
        enterprise.ActionsConsumptionPerOwner[item.Owner].MinutesPerSku[item.SKU] += item.Quantity;
        enterprise.ActionsConsumptionPerOwner[item.Owner].PricePerRepository.TryAdd(item.RepositorySlug, 0);
        enterprise.ActionsConsumptionPerOwner[item.Owner].PricePerRepository[item.RepositorySlug] += item.Quantity * item.PricePerUnit;
    }
}