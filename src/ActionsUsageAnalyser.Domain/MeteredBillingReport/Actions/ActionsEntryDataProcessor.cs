namespace ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;

public class ActionsEntryDataProcessor : IReportEntryDataProcessor
{
    public void ProcessForEnterprise(Enterprise enterprise, MeteredBillingReportItem item)
    {
        if (item.Product is not Product.Actions) return;

        enterprise.ActionsConsumptionPerOwner.TryAdd(item.Owner, new ());
        var owner = enterprise.ActionsConsumptionPerOwner[item.Owner];
        var currentCost = item.PricePerUnit * item.Quantity;
        
        owner.ConsumptionPerSku.TryAdd(item.SKU, (0,0));
        owner.ConsumptionPerSku[item.SKU] = (owner.ConsumptionPerSku[item.SKU].cost + currentCost, owner.ConsumptionPerSku[item.SKU].minutes + (int)item.Quantity);
        owner.CostPerRepository.TryAdd(item.RepositorySlug, 0);
        owner.CostPerRepository[item.RepositorySlug] += item.Quantity * item.PricePerUnit;
    }
}