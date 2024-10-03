namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class PackagesEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Packages) return;

        enterprise.PackagesConsumptionPerOwner.TryAdd(item.Owner, new PackagesConsumption());
        enterprise.PackagesConsumptionPerOwner[item.Owner].AccumulatedCost += item.Quantity * item.PricePerUnit;
        enterprise.PackagesConsumptionPerOwner[item.Owner].PricePerRepository.TryAdd(item.RepositorySlug, 0);
        enterprise.PackagesConsumptionPerOwner[item.Owner].PricePerRepository[item.RepositorySlug] += item.Quantity * item.PricePerUnit;
    }
}