namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Packages;

public class PackagesEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Packages) return;

        enterprise.PackagesConsumptionPerOwner.TryAdd(item.Owner, new PackagesConsumption());
        enterprise.PackagesConsumptionPerOwner[item.Owner].AccumulatedCost += item.Quantity * item.PricePerUnit;
        enterprise.PackagesConsumptionPerOwner[item.Owner].CostPerRepository.TryAdd(item.RepositorySlug, 0);
        enterprise.PackagesConsumptionPerOwner[item.Owner].CostPerRepository[item.RepositorySlug] += item.Quantity * item.PricePerUnit;
    }
}