using ActionsUsageAnalyser.Domain.MeteredBillingReport;
using ActionsUsageAnalyser.Domain.MeteredBillingReport.Actions;
using Shouldly;

namespace ActionsUsageAnalyser.Domain.Tests.MeteredBillingReport;

public class ActionsEntryDataProcessorTests
{
    private readonly ActionsEntryDataProcessor dataProcessor = new();

    [Fact]
    public void ProcessForEnterprise_WhenItemIsNotActionsProduct_ShouldNotAddToEnterprise()
    {
        var enterprise = new Enterprise();
        var item = new MeteredBillingReportItem
        {
            Product = Product.Copilot
        };

        dataProcessor.ProcessForEnterprise(enterprise, item);
        
        enterprise.ActionsConsumptionPerOwner.ShouldBeEmpty();
    }

    [Fact]
    public void ProcessForEnterprise_WhenItemIsActionsProduct_ShouldAddToEnterprise()
    {
        var enterprise = new Enterprise();
        var item = new MeteredBillingReportItem
        {
            Product = Product.Actions,
            Owner = "owner",
            SKU = "sku",
            Quantity = 1,
            Multiplier = 2,
            PricePerUnit = 1,
            RepositorySlug = "repo"
        };

        dataProcessor.ProcessForEnterprise(enterprise, item);

        enterprise.ShouldSatisfyAllConditions(
            () => enterprise.ActionsConsumptionPerOwner.ShouldContainKey(item.Owner),
            () => enterprise.ActionsConsumptionPerOwner[item.Owner].ConsumptionPerSku.ShouldContainKey(item.SKU),
            () => enterprise.ActionsConsumptionPerOwner[item.Owner].ConsumptionPerSku[item.SKU].minutes.ShouldBe((int)item.Quantity),
            () => enterprise.ActionsConsumptionPerOwner[item.Owner].ConsumptionPerSku[item.SKU].cost.ShouldBe(item.Quantity * item.PricePerUnit),
            () => enterprise.ActionsConsumptionPerOwner[item.Owner].CostPerRepository.ShouldContainKey(item.RepositorySlug),
            () => enterprise.ActionsConsumptionPerOwner[item.Owner].CostPerRepository[item.RepositorySlug].ShouldBe(item.Quantity * item.PricePerUnit)
        );
    }

    [Fact]
    public void ProcessForEnterprise_WhenCalledMultipleTimes_AggregatesData()
    {
        var enterprise = new Enterprise();

        var items = new List<MeteredBillingReportItem>
        {
            new()
            {
                Product = Product.Actions,
                Owner = "owner",
                SKU = "sku1",
                Quantity = 1,
                Multiplier = 1.1m,
                PricePerUnit = 1.3m,
                RepositorySlug = "repo-001"
            },
            new()
            {
                Product = Product.Actions,
                Owner = "owner",
                SKU = "sku2",
                Quantity = 2,
                Multiplier = 1.2m,
                PricePerUnit = 1.2m,
                RepositorySlug = "repo-002"
            },
            new()
            {
                Product = Product.Actions,
                Owner = "owner",
                SKU = "sku1",
                Quantity = 3,
                Multiplier = 2.0m,
                PricePerUnit = 1.1m,
                RepositorySlug = "repo-003"
            }
        };

        foreach (var item in items)
        {
            dataProcessor.ProcessForEnterprise(enterprise, item);
        }

        enterprise.ActionsConsumptionPerOwner.ShouldContainKey("owner");

        foreach (var sku in items.GroupBy(i => i.SKU))
        {
            enterprise.ActionsConsumptionPerOwner["owner"].ConsumptionPerSku.ShouldContainKey(sku.Key);
            enterprise.ActionsConsumptionPerOwner["owner"].ConsumptionPerSku[sku.Key].minutes.ShouldBe(sku.Sum(i => (int)i.Quantity));
            enterprise.ActionsConsumptionPerOwner["owner"].ConsumptionPerSku[sku.Key].cost.ShouldBe(sku.Sum(i => i.Quantity * i.PricePerUnit));
        }

        foreach (var group in items.GroupBy(i => i.RepositorySlug))
        {
            enterprise.ActionsConsumptionPerOwner["owner"].CostPerRepository.ShouldContainKey(group.Key);
            enterprise.ActionsConsumptionPerOwner["owner"].CostPerRepository[group.Key].ShouldBe(group.Sum(i => i.Quantity * i.PricePerUnit));
        }
    }
}