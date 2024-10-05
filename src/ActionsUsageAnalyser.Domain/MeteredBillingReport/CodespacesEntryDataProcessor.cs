namespace ActionsUsageAnalyser.Domain.MeteredBillingReport;

public class CodespacesEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.CodespacesLinux) return;

        enterprise.CodespacesConsumptionPerOwner.TryAdd(item.Owner, new CodespacesConsumption());
        var consumptionPerOwner = enterprise.CodespacesConsumptionPerOwner[item.Owner];
        var currentCost = item.Quantity * item.PricePerUnit;
        
        consumptionPerOwner.AccumulatedCost += currentCost;
        consumptionPerOwner.PricePerRepository.TryAdd(item.RepositorySlug, 0);
        consumptionPerOwner.PricePerRepository[item.RepositorySlug] += currentCost;
        
        consumptionPerOwner.ConsumptionPerSku.TryAdd(item.SKU, (item.UnitType, 0));
        var consumptionForSkuSoFar = consumptionPerOwner.ConsumptionPerSku[item.SKU];
        consumptionPerOwner.ConsumptionPerSku[item.SKU] = (consumptionForSkuSoFar.unit, consumptionForSkuSoFar.cost + item.Quantity * item.PricePerUnit);
    }
}